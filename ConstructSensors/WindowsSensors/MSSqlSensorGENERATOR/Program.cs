using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Xml.Linq;
using Telerik.Windows.Zip;

namespace MSSqlSensorGENERATOR
{
    class Program
    {
        private static int version = 1000;
        private static string databaseName;
        private static string tableName;
        private static string serverName;
        private static string connectionString;
        //private static string defaultCCFConnectionString = "data source=daisy.colum.edu;initial catalog=CCF;User ID=CCF;Password=!!thewoodsarelovelydarkanddeep??";
        private const string xmlFileName = "construct.xml";
        private static Guid sensorTypeSourceID = Guid.NewGuid();
        private static Guid sensorTypeSourceParentID = Guid.Parse("5C11FBBD-9E36-4BEA-A8BE-06E225250EF8");
        private static Guid sensorHostTypeID = Guid.Parse("EDA0FF3E-108B-45D5-BF58-F362FABF2EFE");
        
        private static Dictionary<string, string> tableColumns = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            ReadUserInput();

            ReadTargetTableSchema();

            WriteXmlDefDoc();

            ZipSensorPackage();
        }

        private static void ReadUserInput()
        {
            #region I/O code for reading in connection string info in peices (prevent injection?)
            //Console.WriteLine("Please enter the name of the server to target.");
            //serverName = Console.ReadLine();

            //while (!IsValidServerName(serverName))
            //{
            //    Console.WriteLine("Server name must only contain alpha-numeric characters, and \".\" or \"_\".");
            //    Console.WriteLine("Please re-enter the name of the database you wish to copy from.");
            //    serverName = Console.ReadLine();
            //}

            //Console.WriteLine("Please enter the name of the database you wish to copy from.");
            //database = Console.ReadLine();

            //while (!IsValidDBName(database))
            //{
            //    Console.WriteLine("Database name must only contain alpha-numeric characters and \"_\".");
            //    Console.WriteLine("Please re-enter the name of the database you wish to copy from.");
            //    database = Console.ReadLine();
            //}

            //Console.WriteLine("Please enter the name of the table in Database:" + database + " to copy from");
            //tableName = Console.ReadLine();

            //while (!IsValidDBName(tableName))
            //{
            //    Console.WriteLine("Table name must only contain alpha-numeric characters and \"_\".");
            //    Console.WriteLine("Please re-enter the name of the table in Database:" + database + " to copy from");
            //    tableName = Console.ReadLine();
            //}
            
            ////Format connection string together
            //connectionString = String.Format("data source={0};initial catalog={1};User ID={2};Password={0}", blah, blah, blah, blah);
            #endregion

            Console.WriteLine("Please enter the connection string for the target database.");
            connectionString = Console.ReadLine();
            bool couldConnect = false;
            bool goodDBName = false;
            while(!couldConnect || !goodDBName)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                        {
                            conn.Open();
                        }
                        couldConnect = true;
                    }
                }
                catch (Exception e)
                {
                    couldConnect = false;
                }

                if (couldConnect)
                {
                    try
                    {
                        string[] connectionStringParts = connectionString.Split(';');
                        string srvTmpNm = connectionStringParts[0];
                        serverName = srvTmpNm.Substring(srvTmpNm.IndexOf('=') + 1);
                        serverName = serverName.Replace('.', '_');
                        string dbTmpNm = connectionStringParts[1];
                        databaseName = dbTmpNm.Substring(dbTmpNm.IndexOf('=') + 1);
                        goodDBName = true;
                    }
                    catch (Exception e)
                    {
                        databaseName = "<BAD_LAST_READ>";
                        goodDBName = false;
                    }
                }
            }

            Console.WriteLine("Please enter the name of the table in Database:" + databaseName + " to copy from");
            tableName = Console.ReadLine();

            while (!IsValidDBName(tableName))
            {
                Console.WriteLine("Table name must only contain alpha-numeric characters and \"_\".");
                Console.WriteLine("Please re-enter the name of the table in Database:" + databaseName + " to copy from");
                tableName = Console.ReadLine();
            }
        }

        private static void ReadTargetTableSchema()
        {
            string queryString = String.Format(@"SELECT TOP 1 * FROM [{0}].[dbo].[{1}];", databaseName, tableName);
            using (SqlConnection conn = new SqlConnection(connectionString))
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

                DataTable schemaTable = reader.GetSchemaTable();
                foreach (DataRow row in schemaTable.Rows)
                {
                    tableColumns.Add((string)(row["ColumnName"]), ((Type)(row["DataType"])).FullName);
                }

                reader.Close();
            }
        }

        private static void WriteXmlDefDoc()
        {
            if (File.Exists(xmlFileName))
            {
                File.Delete(xmlFileName);
            }
            #region readable output-xml, commented
            /*
             *  THIS CODE IS TO TRY TO MAKE THE OUTPUT XML LOOK READABLE.
             *  It isnt finished, needs to add datatype properties to the datatype node
             */
            //XDocument doc = new XDocument();
            //doc.Add(new XElement("SensorTypeSource",
            //            new XAttribute("Name", String.Format("MSSql{0}TableSensor", tableName)),
            //            new XAttribute("ID", Guid.NewGuid().ToString()),
            //            new XAttribute("ParentID", Guid.NewGuid().ToString()),
            //            new XAttribute("SensorHostTypeID", Guid.NewGuid().ToString()),
            //            new XAttribute("Version", version.ToString()),
            //            new XElement("DataType",
            //                new XAttribute("Name", String.Format("MSSql{0}Table", tableName)),
            //                new XAttribute("ID", Guid.NewGuid().ToString()))));
            //// add data type properties to the datatype node

            //string xmlText = doc.ToString();
            //StreamWriter xmlWriter = File.CreateText(xmlDefFileName);
            //xmlWriter.Write(xmlText);
            //xmlWriter.Flush();
            //xmlWriter.Close();
            #endregion

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.Unicode;

            using (XmlWriter writer = XmlWriter.Create(xmlFileName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("SensorTypeSource");
                writer.WriteAttributeString("Name", String.Format("MSSql_{0}_{1}_{2}Sensor", serverName, databaseName, tableName));
                                                    
                writer.WriteAttributeString("ID", sensorTypeSourceID.ToString());
                writer.WriteAttributeString("ParentID", sensorTypeSourceParentID.ToString());
                writer.WriteAttributeString("SensorHostTypeID", sensorHostTypeID.ToString());
                writer.WriteAttributeString("Version", version.ToString());

                writer.WriteStartElement("DataType");
                writer.WriteAttributeString("Name", String.Format("MSSql_{0}_{1}_{2}", serverName, databaseName, tableName));
                writer.WriteAttributeString("ID", Guid.NewGuid().ToString());

                foreach (string columnName in tableColumns.Keys)
                {
                    writer.WriteStartElement("DataTypeProperty");
                    writer.WriteAttributeString("Name", columnName);
                    writer.WriteAttributeString("Type", tableColumns[columnName]);
                    writer.WriteAttributeString("ID", Guid.NewGuid().ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private static void ZipSensorPackage()
        {
            // Check MSSQL table sensor files exist
            DirectoryInfo sourceDirInfo = new DirectoryInfo(".\\..\\..\\..\\MSSqlSensor\\bin\\Debug");
            FileInfo[] sensorFiles = sourceDirInfo.GetFiles();
            if (sensorFiles.Where(f => f.Name == "Construct.MessageBrokering.dll").Count() == 0 ||
                sensorFiles.Where(f => f.Name == "Construct.Sensors.dll").Count() == 0 ||
                sensorFiles.Where(f => f.Name == "Newtonsoft.Json.dll").Count() == 0||
                sensorFiles.Where(f => f.Name == "MSSqlSensor.REPLACE_ME.1000.exe").Count() == 0)
            {
                // error out and break;
                Console.WriteLine("Did not find one of the files:");
                Console.WriteLine("\t\"Construct.MessageBrokering.dll\"");
                Console.WriteLine("\t\"Construct.Sensors.dll\"");
                Console.WriteLine("\t\"Newtonsoft.Json.dll\"");
                Console.WriteLine("\t\"MSSqlSensor.REPLACE_ME.1000.exe\"");
                if (File.Exists(xmlFileName))
                {
                    File.Delete(xmlFileName);
                }
                return;
            }

            string nameReplaceBase = "MSSqlSensor.REPLACE_ME.";
            string generatedNameBase = String.Format("MSSql_{0}_{1}_{2}Sensor.{3}.{4}", serverName, databaseName, tableName, sensorTypeSourceID, version);
            List<string> replacedZipFileNames = new List<string>();

            //copy MSSQL table sensor files and rename to table specific version
            foreach (FileInfo file in sensorFiles.Where(f => f.Name.Contains(nameReplaceBase)))
            {
                string newName = String.Format("{0}{1}", generatedNameBase, file.Name.Substring(nameReplaceBase.Length + version.ToString().Length));
                File.Copy(file.FullName, newName);
                replacedZipFileNames.Add(newName);
            }

            string zipPackageFileName = String.Format("{0}.zip", generatedNameBase);
            if (File.Exists(zipPackageFileName))
            {
                File.Delete(zipPackageFileName);
            }

            //write zip with new files
            using (FileStream fileStream = new FileStream(zipPackageFileName, FileMode.OpenOrCreate))
            {
                using (ZipPackage package = ZipPackage.Create(fileStream))
                {
                    foreach (FileInfo file in sensorFiles.Where(f => f.Name.Contains("MSSqlSensor.REPLACE_ME.") == false))
                    {
                        package.Add(file.FullName);
                    }

                    package.Add(xmlFileName);

                    foreach (string fileName in replacedZipFileNames)
                    {
                        package.Add(fileName);
                    }
                }
            }

            // clean up copied files
            File.Delete(xmlFileName);
            foreach (string fileName in replacedZipFileNames)
            {
                File.Delete(fileName);
            }
        }

        public static bool IsValidDBName(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            for (int i = 0; i < str.Length; i++)
            {
                if (!(char.IsLetter(str[i])) && (!(char.IsNumber(str[i]))) && (!(IsLegalDBFormatChar(str[i]))))
                    return false;
            }

            return true;
        }

        public static bool IsValidServerName(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            for (int i = 0; i < str.Length; i++)
            {
                if (!(char.IsLetter(str[i])) && (!(char.IsNumber(str[i]))) && (!(IsLegalServerFormatChar(str[i]))))
                    return false;
            }

            return true;
        }

        private static bool IsLegalDBFormatChar(char ch)
        {
            return (ch == '_');
        }

        private static bool IsLegalServerFormatChar(char ch)
        {
            return (ch == '_');
        }
    }
}
