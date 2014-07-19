using System;
using System.Data;
using System.Data.Sql;
using Microsoft.SqlServer.Server;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml;
using System.Text.RegularExpressions;
using System.Text;

public partial class TABLENAMETrigger
{
    [SqlTrigger(Name = @"PROPERTYVALUENAMETrigger", Target = "[dbo].[TABLENAME]", Event = "FOR INSERT, UPDATE, DELETE")]
    public static void PROPERTYVALUENAMETrigger()
    {
        SqlCommand command;
        SqlTriggerContext triggContext = SqlContext.TriggerContext;
        SqlPipe pipe = SqlContext.Pipe;
        SqlDataReader reader;

        switch (triggContext.TriggerAction)
        {
            case TriggerAction.Insert:
                // Retrieve the connection that the trigger is using
                using (SqlConnection connection = new SqlConnection(@"context connection=true"))
                {
                    connection.Open();
                    command = new SqlCommand(@"SELECT * FROM INSERTED;",
                        connection);
                    reader = command.ExecuteReader();
                    reader.Read();
                    reader.Close();

                    pipe.Send("You inserted " + reader.RecordsAffected + "rows.");
                    
                }

                break;
                
                //return;
                //break;

        }
    }
}