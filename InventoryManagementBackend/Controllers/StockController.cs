using InventoryManagement.Models;
using InventoryManagementLibrary.DAL;
using InventoryManagementLibrary.Helpers;
using InventoryManagementLibrary.Models;
using Newtonsoft.Json;
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
                var availableProducts = allProducts.Except(addedProducts, StringComparer.OrdinalIgnoreCase).ToList();

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
                var httpRequest = HttpContext.Current.Request;

                var json = httpRequest["Data"];
                var model = JsonConvert.DeserializeObject<AddStockRequestModel>(json);

                var uploadedFile = httpRequest.Files["ImageFile"];
                string fileName = null;

                if (uploadedFile != null && uploadedFile.ContentLength > 0)
                {
                    fileName = Path.GetFileName(uploadedFile.FileName);
                    var path = HttpContext.Current.Server.MapPath("~/Content/images/" + fileName);
                    uploadedFile.SaveAs(path);
                }

                string errorMessage;
                var result = stockInsertRepository.AddStoreProduct(model.StoreName, model.ProductName,model.StorePrice,model.Stock,"System",fileName,out errorMessage);

                return Ok(new { success = result, message = errorMessage });
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }



        [HttpGet]
        [Route("edit-form-data/{id}")]
        public IHttpActionResult GetEditStockFormData(int id)
        {
            try
            {
                var data = repository.GetStockById(id);

                var model = new WebEditStockViewModel
                {
                    StoreProductId = data.StoreProductId,
                    StorePrice = data.StorePrice,
                    Stock = data.Stock,
                    ImagePath = data.ImagePath,
                    StoreName = data.StoreName,
                    ProductName = data.ProductName
                };

                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        [Route("update")]
        public IHttpActionResult UpdateStock()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;

           
                var json = httpRequest["model"];

                var model = JsonConvert.DeserializeObject<WebEditStockViewModel>(json);

                var file = httpRequest.Files["ImageFile"];
                string fileName = null;

                if (file != null && file.ContentLength > 0)
                {
                    fileName = Path.GetFileName(file.FileName);
                    var savePath = HttpContext.Current.Server.MapPath("~/Content/images/" + fileName);
                    file.SaveAs(savePath);
                }

                string error;
                var success = repository.UpdateStoreProduct( model.StoreProductId,model.StorePrice, model.Stock,"admin", fileName, out error);

                return Ok(new { success, message = error });
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IHttpActionResult DeleteStock(int id)
        {
            try
            {
                string modifiedBy = User?.Identity?.Name ?? "System";

                string error;
                bool success = repository.DeleteStoreProduct(id, modifiedBy, out error);

                if (success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Deleted successfully"
                    });
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, new
                    {
                        success = false,
                        message = error
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);

                return InternalServerError(new Exception("Unexpected error while deleting."));
            }


        }
    }
}