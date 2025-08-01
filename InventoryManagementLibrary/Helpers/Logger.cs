

using System;
using System.Data;
using System.Data.SqlClient;

namespace InventoryManagementLibrary.Helpers
{
    public static class Logger
    {
        private static readonly string connectionString = DatabaseHelper.GetConnectionString();

        public static void LogException(string errorMessage, string stackTrace, int userId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("InsertErrorLog", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ErrorMessage", string.IsNullOrEmpty(errorMessage) ? (object)DBNull.Value : errorMessage);
                    cmd.Parameters.AddWithValue("@StackTrace", string.IsNullOrEmpty(stackTrace) ? (object)DBNull.Value : stackTrace);
                    cmd.Parameters.AddWithValue("@CreatedBy", userId > 0 ? (object)userId : DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Logging failed: " + ex.Message);
            }
        }
    }
}
