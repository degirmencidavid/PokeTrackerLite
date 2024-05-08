using PokemonCollection.Utilities;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace YourProjectName.DataAccess
{
    public static class DataAccess
    {
        private static readonly string ConnectionString = "PUT CONNECTION STRING HERE";

        public static SqlConnection CreateConnection()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            return connection;
        }

        public static SqlConnection GetOpenConnection()
        {
            SqlConnection connection = CreateConnection();
            connection.Open();
            return connection;
        }

        public static SqlConnection GetOpenConnection(SqlConnection connection)
        {
            connection.Open();
            return connection;
        }

        public static SqlCommand CreateCommand(string query, SqlConnection connection)
        {
            SqlCommand command = new SqlCommand(query, connection);
            // Additional configurations can be added here if needed
            return command;
        }





        // Lazy get set ID
        public static string GetSetNameFromID(string setID)
        {
            using (SqlConnection connection = GetOpenConnection())
            {
                using (SqlCommand command = CreateCommand(SQLCommands.ReadSetBySetID, connection))
                {
                    try
                    {
                        command.Parameters.AddWithValue("@setID", setID);

                        string result = command.ExecuteScalar().ToString();

                        return result;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        return null;
                    }
                }
            }
        }
    }
}
