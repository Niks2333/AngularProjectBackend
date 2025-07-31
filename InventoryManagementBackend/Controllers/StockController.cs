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
        [HttpPost]
        [Route("bystore")]
        public IHttpActionResult GetStockByStore([FromBody] StoreProductViewModel model)
        {
            try
            {
                var products = repository.GetStoreProducts(
                    model.StoreName,
                    model.Search,
                    model.SelectedCategories,
                    model.Page,
                    model.PageSize,
                    model.SortColumn,
                    model.SortOrder
                );

                var allCategories = repository.GetStoreProducts(model.StoreName)
                                              .Select(p => p.CategoryName)
                                              .Distinct()
                                              .ToList();

                var totalProductsCount = repository.GetStoreProducts(
                    model.StoreName,
                    model.Search,
                    model.SelectedCategories,
                    1,
                    int.MaxValue
                ).Count;

                model.Products = products;
                model.Categories = allCategories;
                model.TotalCount = totalProductsCount;

                return Ok(model);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }

    }
}