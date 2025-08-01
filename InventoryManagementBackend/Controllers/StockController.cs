using InventoryManagement.Models;
using InventoryManagementLibrary.DAL;
using InventoryManagementLibrary.Helpers;
using InventoryManagementLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace InventoryManagementBackend.Controllers
{
    [RoutePrefix("api/stock")]
    public class StockController : ApiController
    {
        private readonly StoreStockRepository repository = new StoreStockRepository();
        private readonly StockInsertRepository stockInsertRepository = new StockInsertRepository();

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




        [HttpGet]
        [Route("add-form-data")]
        public IHttpActionResult GetAddStockFormData(string storeName)
        {
            try
            {
                var allProducts = repository.GetAllProductNames();
                var addedProducts = repository.GetAddedProductNamesForStore(storeName);
                var availableProducts = allProducts.Except(addedProducts).ToList();

                var model = new WebAddStockViewModel
                {
                    StoreName = storeName,
                    AvailableProducts = availableProducts
                };

                return Ok(model);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> AddStock()
        {
            try
            {
                var httpRequest =HttpContext.Current.Request;

                var storeName = httpRequest["StoreName"];
                var productName = httpRequest["ProductName"];
                var storePrice = Convert.ToDecimal(httpRequest["StorePrice"]);
                var stock = Convert.ToInt32(httpRequest["Stock"]);
                var uploadedFile = httpRequest.Files["ImageFile"];
                string fileName = null;

                if (uploadedFile != null && uploadedFile.ContentLength > 0)
                {
                    fileName = Path.GetFileName(uploadedFile.FileName);
                    var path = HttpContext.Current.Server.MapPath("~/Content/images/" + fileName);
                    uploadedFile.SaveAs(path);
                }

                string errorMessage;
                var result = stockInsertRepository.AddStoreProduct(
                    storeName,
                    productName,
                    storePrice,
                    stock,
                    "System", 
                    fileName,
                    out errorMessage
                );

                return Ok(new { success = result, message = errorMessage });
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return Content(HttpStatusCode.InternalServerError, new
                {
                    success = false,
                    message = "Exception: " + ex.Message, 
                    stackTrace = ex.StackTrace
                });
            }

        }

    }
}