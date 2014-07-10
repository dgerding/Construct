using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Timers;
using System.Collections.Concurrent;
using System.Threading;
using System.Data;
using System.Xml.Linq;
using System.IO;
using Construct.Sensors;
using Construct.MessageBrokering;

namespace Construct.Sensors.MSSqlSensor
{
    public class MSSqlSensor : Sensor
    {
        private string defaultCCFConnectionString;
        private ConcurrentQueue<List<Dictionary<string, object>>> queue;
        private string[] keys;
        private string defaultDatabaseTable;
        private string IDColumnName;
        private Thread fetchThread;
        private Thread sendThread;
        private readonly Dictionary<string, string> availableCommands;

        public MSSqlSensor(string[] args)
            : base(Protocol.HTTP, args, Guid.Empty, new Dictionary<string, Guid>() { { "HAS_NOT_INITIALIZED", Guid.Empty} })
        {
            defaultCCFConnectionString = "data source=daisy.colum.edu;initial catalog=CCF;User ID=CCF;Password=!!thewoodsarelovelydarkanddeep??";
            if (args.Length == 5)
            {
                defaultDatabaseTable = args[3];
                IDColumnName = args[4];
            }
            else
            {
                defaultDatabaseTable = "_DB_NOT_SET";
                IDColumnName = "ID_COLUMN_NOT_SET";
            }
            queue = new ConcurrentQueue<List<Dictionary<string, object>>>();
            availableCommands = new Dictionary<string,string>();

            fetchThread = new Thread(FetchDelegate);
            sendThread = new Thread(SendDelegate);

            InitializedDictKeyList();

            GatherAvailableCommands();
            SendAvailableCommandsTelemetry();

            broker.OnCommandReceived += broker_OnCommandReceived;
        }

        private void GatherAvailableCommands()
        {
            if (availableCommands.Keys.Contains("SetDatabaseAndTable") == false)
            {
                availableCommands.Add("SetDatabaseAndTable", "DatabaseTableString(<database>.[dbo].<table>)");
            }
            if (availableCommands.Keys.Contains("SetIDColumn") == false)
            {
                availableCommands.Add("SetIDColumn", "IDColumn");
            }
        }

        private void SendAvailableCommandsTelemetry()
        {
            broker.Publish(new Telemetry("AvailableSensorCommands", availableCommands));
        }

        private void InitializedDictKeyList()
        {
            string xmlText = new StreamReader(File.OpenRead("construct.xml")).ReadToEnd();
            XDocument document = XDocument.Parse(xmlText);

            Guid sensorTypeSourceID = Guid.Parse(document.Root.Attribute("ID").Value);

            DataTypes.Remove("HAS_NOT_INITIALIZED");

            string dataTypeName = ((XElement)document.Root.FirstNode).Attribute("Name").Value;
            Guid dataTypeID = Guid.Parse(((XElement)document.Root.FirstNode).Attribute("ID").Value);
            DataTypes.Add(dataTypeName, dataTypeID);

            int nodeCount = ((XElement)document.Root.FirstNode).Nodes().Count();
            keys = new string[nodeCount];

            XElement currentNode = ((XElement)((XElement)document.Root.FirstNode).Nodes().First());
            for (int i = 0; i < nodeCount; ++i)
            {
                keys[i] = currentNode.Attribute("Name").Value;
                currentNode = (XElement)currentNode.NextNode;
            }
        }

        private void broker_OnCommandReceived(object sender, string commandString)
        {
            Command command = JsonConvert.DeserializeObject<Command>(commandString);

            switch (command.Name)
            {
                case "AddItemOutbox":
                    Rendezvous<Data> itemRendezvous = new Rendezvous<Data>(command.Args["Uri"]);
                    broker.AddPeer(new Outbox<Data>(itemRendezvous));
                    break;
                default:
                    break;
            }
        }

        private void FetchDelegate()
        {
            int lastListCount = 0;
            int totalReads = 0;
            do
            {
                string queryString = String.Format(@"SELECT * FROM {0} WHERE {1} IN (SELECT {1} 
              FROM   (select {1},
                             ROW_NUMBER() over (order by {1} asc) as RowId
                      from   {0}) dt
              WHERE  RowId between {2} and {3});", defaultDatabaseTable, IDColumnName, (totalReads + 1).ToString(), (totalReads + 1000).ToString());
                using (SqlConnection conn = new SqlConnection(defaultCCFConnectionString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlCommand readCommand = null;
                    SqlDataReader reader = null;
                    try
                    {
                        readCommand = new SqlCommand(queryString, conn);
                        reader = readCommand.ExecuteReader();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                    while (reader.Read())
                    {
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; ++i)
                        {
                            row.Add(keys[i], reader[i]);
                        }
                        list.Add(row);
                    } 
                    //When we reach here, it means the block of 1000 records has been read

                    if (list.Count > 0)
                    {
                        queue.Enqueue(list);
                    }
                    lastListCount = list.Count;
                    totalReads += list.Count;
                    reader.Close();
                }
            }
            while (lastListCount > 0);
        }

        private void SendDelegate()
        {
            while (true)
            {
                if (!queue.IsEmpty)
                {
                    List<Dictionary<string, object>> tempList;
                    if (queue.TryDequeue(out tempList))
                    {
                        foreach (Dictionary<string, object> row in tempList)
                        {
                            SendItem(row, DataTypes.Keys.First());
                        }
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        protected override string Start()
        {
            fetchThread.Start();
            sendThread.Start();

            return base.Start();
        }

        protected override string Stop()
        {
            fetchThread.Abort(); // correct?
            sendThread.Abort();  // correct?

            return base.Stop();
        }
        # region old ORM class
        //private class TempDBRow
        //{
        //    public TempDBRow(object[] values)
        //    {
        //        this.LogEntryID = DBSafeAssign(values[0]);
        //        this.GameID = DBSafeAssign(values[1]);
        //        this.LogTimeStamp = DBSafeAssign(values[2]);
        //        this.ActionDefID = DBSafeAssign(values[3]);
        //        this.ActorObjectID = DBSafeAssign(values[4]);
        //        this.AffectedObjectID = DBSafeAssign(values[5]);
        //        this.RootIntervalIndex = DBSafeAssign(values[6]);
        //        this.IsGenerated = DBSafeAssign(values[7]);
        //    }

        //    public TempDBRow(int logEntryID,
        //                     int gameID, 
        //                     DateTime logTimeStamp,
        //                     int actionDefID,
        //                     int actorObjectID,
        //                     int affectedObjectID,
        //                     int rootIntervalIndex,
        //                     bool isGenerated)
        //    {
        //        this.LogEntryID = logEntryID;
        //        this.GameID = gameID;
        //        this.LogTimeStamp = logTimeStamp;
        //        this.ActionDefID = actionDefID;
        //        this.ActorObjectID = actorObjectID;
        //        this.AffectedObjectID = affectedObjectID;
        //        this.RootIntervalIndex = rootIntervalIndex;
        //        this.IsGenerated = isGenerated;
        //    }

        //    public int LogEntryID { get; set; }
        //    public int GameID { get; set; }
        //    public DateTime LogTimeStamp { get; set; }
        //    public int ActionDefID { get; set; }
        //    public int ActorObjectID { get; set; }
        //    public int AffectedObjectID { get; set; }
        //    public int RootIntervalIndex { get; set; }
        //    public bool IsGenerated { get; set; }
        //}
        #endregion
    }
}
