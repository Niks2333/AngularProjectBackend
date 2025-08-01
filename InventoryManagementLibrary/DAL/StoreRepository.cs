using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InventoryManagementLibrary.Models;
using InventoryManagementLibrary.Helpers;

namespace InventoryManagementLibrary.DAL
{
    public class StoreRepository
    {
        private readonly string connectionString;

        public StoreRepository()
        {
            connectionString = DatabaseHelper.GetConnectionString();
        }

        public List<StoreModel> GetStores(int pageNumber = 1, int pageSize = 10, string sortColumn = "StoreName", string sortOrder = "ASC")
        {
            var stores = new List<StoreModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("StoresGetList", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PageNumber", pageNumber);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    command.Parameters.AddWithValue("@SortColumn", sortColumn);
                    command.Parameters.AddWithValue("@SortOrder", sortOrder);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        stores.Add(new StoreModel
                        {
                            StoreId = Convert.ToInt32(reader["StoreId"]),
                            StoreName = reader["StoreName"].ToString(),
                            CreatedBy = reader["CreatedBy"].ToString(),
                            CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                            StoreImage = reader["StoreImage"] != DBNull.Value ? (byte[])reader["StoreImage"] : null
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
            }

            return stores;
        }

        public void DeleteStore(int storeId, string modifiedBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("StoresDelete", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StoreId", storeId);
                    command.Parameters.AddWithValue("@LastModifiedBy", modifiedBy);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
            }
        }

        public bool InsertStore(string storeName, int storeTypeId, string createdBy, byte[] storeImage, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("StoresInsert", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StoreName", storeName);
                    command.Parameters.AddWithValue("@StoreTypeId", storeTypeId);
                    command.Parameters.AddWithValue("@CreatedBy", createdBy);
                    command.Parameters.Add("@StoreImage", SqlDbType.VarBinary).Value = (object)storeImage ?? DBNull.Value;

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

        public List<StoreTypeModel> GetStoreTypes()
        {
            var list = new List<StoreTypeModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("StoreTypesGetActiveList", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new StoreTypeModel
                            {
                                StoreTypeId = Convert.ToInt32(reader["StoreTypeId"]),
                                StoreTypeName = reader["StoreTypeName"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
            }

            return list;
        }

        public StoreModel GetStoreById(int storeId)
        {
            StoreModel store = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("StoresGetById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StoreId", storeId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            store = new StoreModel
                            {
                                StoreId = (int)reader["StoreId"],
                                StoreName = reader["StoreName"].ToString(),
                                CreatedBy = reader["CreatedBy"].ToString(),
                                CreatedOn = (DateTime)reader["CreatedOn"],
                                StoreImage = reader["StoreImage"] as byte[],
                                StoreTypeId = (int)reader["StoreTypeId"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
            }

            return store;
        }

        public bool UpdateStoreImage(int storeId, byte[] newImage, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("StoresUpdateImage", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StoreId", storeId);
                    command.Parameters.AddWithValue("@StoreImage", (object)newImage ?? DBNull.Value);

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
