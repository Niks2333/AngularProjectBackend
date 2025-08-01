using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InventoryManagementLibrary.Models;
using InventoryManagementLibrary.Helpers;
using System.Linq;

namespace InventoryManagementLibrary.DAL
{
    public class StoreStockRepository
    {
        private readonly string connectionString;

        public StoreStockRepository()
        {
            connectionString = DatabaseHelper.GetConnectionString();
        }

        //public List<StoreProductViewModel> GetStoreProducts(string storeName, string search = null, string category = null, int page = 1, int pageSize = 10, string sortColumn = "ProductName", string sortOrder = "ASC")
        //{
        //    var list = new List<StoreProductViewModel>();

        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        using (SqlCommand command = new SqlCommand("StoreProductsGetList", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.AddWithValue("@StoreName", string.IsNullOrEmpty(storeName) ? (object)DBNull.Value : storeName);
        //            command.Parameters.AddWithValue("@Search", string.IsNullOrEmpty(search) ? (object)DBNull.Value : search);
        //            command.Parameters.AddWithValue("@CategoryName", string.IsNullOrEmpty(category) ? (object)DBNull.Value : category);
        //            command.Parameters.AddWithValue("@PageNumber", page);
        //            command.Parameters.AddWithValue("@PageSize", pageSize);
        //            command.Parameters.AddWithValue("@SortColumn", sortColumn);
        //            command.Parameters.AddWithValue("@SortOrder", sortOrder);

        //            connection.Open();
        //            SqlDataReader reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                list.Add(new StoreProductViewModel
        //                {
        //                    StoreProductId = Convert.ToInt32(reader["StoreProductId"]),
        //                    StoreName = reader["StoreName"].ToString(),
        //                    ProductName = reader["ProductName"].ToString(),
        //                    CategoryName = reader["CategoryName"].ToString(),
        //                    StorePrice = Convert.ToDecimal(reader["StorePrice"]),
        //                    Stock = Convert.ToInt32(reader["Stock"]),
        //                    ImagePath = reader["ImagePath"].ToString()
        //                });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogException(ex.Message, ex.StackTrace, 0);
        //    }

        //    return list;
        //}


        public List<StoreProductViewModel> GetStoreProducts(string storeName, string search = null, List<string> selectedCategories = null, int page = 1, int pageSize = 10, string sortColumn = "ProductName", string sortOrder = "ASC")
        {
            var list = new List<StoreProductViewModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("StoreProductsGetList", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StoreName", string.IsNullOrEmpty(storeName) ? (object)DBNull.Value : storeName);
                    command.Parameters.AddWithValue("@Search", string.IsNullOrEmpty(search) ? (object)DBNull.Value : search);

                    string categoriesCsv = (selectedCategories != null && selectedCategories.Any())
                        ? string.Join(",", selectedCategories)
                        : null;

                    command.Parameters.AddWithValue("@CategoryNames", string.IsNullOrEmpty(categoriesCsv) ? (object)DBNull.Value : categoriesCsv);

                    command.Parameters.AddWithValue("@PageNumber", page);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    command.Parameters.AddWithValue("@SortColumn", sortColumn);
                    command.Parameters.AddWithValue("@SortOrder", sortOrder);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new StoreProductViewModel
                        {
                            StoreProductId = Convert.ToInt32(reader["StoreProductId"]),
                            StoreName = reader["StoreName"].ToString(),
                            ProductName = reader["ProductName"].ToString(),
                            CategoryName = reader["CategoryName"].ToString(),
                            StorePrice = Convert.ToDecimal(reader["StorePrice"]),
                            Stock = Convert.ToInt32(reader["Stock"]),
                            ImagePath = reader["ImagePath"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
            }

            return list;
        }

        public StoreProductViewModel GetStockById(int storeProductId)
        {
            StoreProductViewModel model = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("StoreProductGetById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StoreProductId", storeProductId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new StoreProductViewModel
                            {
                                StoreProductId = Convert.ToInt32(reader["StoreProductId"]),
                                StoreName = reader["StoreName"].ToString(),
                                ProductName = reader["ProductName"].ToString(),
                                StorePrice = Convert.ToDecimal(reader["StorePrice"]),
                                Stock = Convert.ToInt32(reader["Stock"]),
                                ImagePath = reader["ImagePath"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
            }

            return model;
        }

        public bool UpdateStoreProduct(int storeProductId, decimal newPrice, int newStock, string modifiedBy, string imagePath, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("StoreProductsUpdate", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StoreProductId", storeProductId);
                    command.Parameters.AddWithValue("@NewPrice", newPrice);
                    command.Parameters.AddWithValue("@NewStock", newStock);
                    command.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
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

        public bool DeleteStoreProduct(int storeProductId, string modifiedBy, out string error)
        {
            error = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("StoreProductsDelete", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StoreProductId", storeProductId);
                    command.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                    connection.Open();
                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                error = ex.Message;
                return false;
            }
        }

        public List<string> GetAllProductNames()
        {
            var list = new List<string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("ProductsGetAllNames", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(reader.GetString(0));
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

        public List<string> GetAddedProductNamesForStore(string storeName)
        {
            var list = new List<string>();

            try
            {
                using (SqlConnection connection  = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("StoreProductsGetAddedNamesByStore", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StoreName", storeName);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(reader.GetString(0));
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
    }
}
