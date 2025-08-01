using System;
using System.Data;
using System.Data.SqlClient;
using InventoryManagementLibrary.Helpers;

namespace InventoryManagementLibrary.DAL
{
    public class StockInsertRepository
    {
        private readonly string connectionString;

        public StockInsertRepository()
        {
            connectionString = DatabaseHelper.GetConnectionString();
        }

        public bool AddStoreProduct(string storeName, string productName, decimal storePrice, int stock, string createdBy, string imagePath, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("StoreProductsInsert", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StoreName", storeName);
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@StorePrice", storePrice);
                    command.Parameters.AddWithValue("@Stock", stock);
                    command.Parameters.AddWithValue("@CreatedBy", createdBy);
                    command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(imagePath) ? (object)DBNull.Value : imagePath);

                    connection.Open();
                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (SqlException ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0); 
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
