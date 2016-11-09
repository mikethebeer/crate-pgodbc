using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace CratePsqlODBC
{
    public class OdbcTransportExecutor
    {
        private const string connectionString = "Dsn=PsqlODBC 32-bit";

        /// <summary>
        /// Executes a statement against Crate containing bulk arguments
        /// </summary>
        /// <param name="statement">the sql statement</param>
        /// <param name="parameters">the list of parameters</param>
        /// <param name="bulkArgs">the bulk arguments</param>
        public void ExecBulkArgs(string statement, string[] parameters, object[][] bulkArgs)
        {
            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    OdbcCommand cmd = conn.CreateCommand();
                    cmd.CommandText = statement;

                    foreach (object[] objects in bulkArgs)
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            cmd.Parameters.AddWithValue(parameters[i], objects[i]);
                        }
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during bulk statement execution: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Executes the statement against Crate
        /// </summary>
        /// <param name="statement">the sql statement</param>
        /// <returns>the response of the execution</returns>
        public OdbcResponse Exec(string statement)
        {
            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    OdbcCommand cmd = conn.CreateCommand();
                    cmd.CommandText = statement;
                    OdbcDataReader dbReader = cmd.ExecuteReader();

                    List<string> cols = Enumerable.Range(0, dbReader.FieldCount).Select(dbReader.GetName).ToList();
                    List<Type> types = Enumerable.Range(0, dbReader.FieldCount).Select(dbReader.GetFieldType).ToList();
                    object[] row = new object[dbReader.FieldCount];
                    List<object[]> response = new List<object[]>();

                    while (dbReader.Read())
                    {
                        dbReader.GetValues(row);
                        response.Add(row);
                    }

                    return new OdbcResponse(cols.ToArray(), response.ToArray(), types);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during statement execution: " + ex.Message);
                    return new OdbcResponse(null, null, null);
                }
            }
        }
    }
}
