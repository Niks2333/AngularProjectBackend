using InventoryManagementLibrary.DAL;
using InventoryManagementLibrary.Helpers;
using InventoryManagementLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventoryManagementBackend.Controllers
{
    [RoutePrefix("api/stock")]
    public class StockController : ApiController
    {
        private readonly StoreStockRepository repository = new StoreStockRepository();

        [HttpGet]
        [Route("bystore")]
        public IHttpActionResult GetStockByStore(
            string storeName,
            string search = null,
            string category = null,
            int page = 1,
            int pageSize = 5,
            string sortColumn = "ProductName",
            string sortOrder = "ASC")
        {
            try
            {
                var products = repository.GetStoreProducts(storeName, search, category, page, pageSize, sortColumn, sortOrder);
                var allCategories = repository.GetStoreProducts(storeName)
                                              .Select(p => p.CategoryName)
                                              .Distinct()
                                              .ToList();
                var totalProductsCount = repository.GetStoreProducts(storeName, search, category, 1, int.MaxValue).Count;

                var viewModel = new StoreProductViewModel
                {
                    Products = products,
                    Categories = allCategories,
                    SelectedCategory = category,
                    Search = search,
                    StoreName = storeName,
                    Page = page,
                    PageSize = pageSize,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    TotalCount = totalProductsCount
                };

                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }
    }
}
