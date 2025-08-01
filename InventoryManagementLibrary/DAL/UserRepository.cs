using System;
using System.Data;
using System.Data.SqlClient;
using InventoryManagementLibrary.Models;
using InventoryManagementLibrary.Helpers;

namespace InventoryManagementLibrary.DAL
{
    public class UserRepository
    {
        private readonly string connectionString;

        public UserRepository()
        {
            connectionString = DatabaseHelper.GetConnectionString();
        }

        public string ValidateUserAndGetEmail(string username, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("UserValidateLogin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        byte[] storedHash = (byte[])reader["PasswordHash"];
                        byte[] storedSalt = (byte[])reader["PasswordSalt"];

                        bool isValid = PasswordHelper.VerifyPassword(password, storedHash, storedSalt);
                        if (isValid)
                        {
                            return reader["Email"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
            }

            return null;
        }

        public bool RegisterUser(UserModel user, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand checkCommand = new SqlCommand("UserCheckExists", con))
                    {
                        checkCommand.CommandType = CommandType.StoredProcedure;
                        checkCommand.Parameters.AddWithValue("@Username", user.Username);
                        int count = (int)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            errorMessage = "Username already exists.";
                            return false;
                        }
                    }

                    using (SqlCommand insertCommand = new SqlCommand("UsersInsert", con))
                    {
                        insertCommand.CommandType = CommandType.StoredProcedure;
                        insertCommand.Parameters.AddWithValue("@Username", user.Username);
                        insertCommand.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                        insertCommand.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt);
                        insertCommand.Parameters.AddWithValue("@Email", user.Email);

                        int rows = insertCommand.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
