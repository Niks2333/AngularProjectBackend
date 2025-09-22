using InventoryManagementLibrary.DAL;
using InventoryManagementLibrary.Helpers;
using InventoryManagementLibrary.Models;
using InventoryMangement.Middleware;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace InventoryManagementBackend.Controllers
{
    [JwtAuthorize]
    [RoutePrefix("api/store")]
    public class StoreController : ApiController
    {
        private readonly StoreRepository repository = new StoreRepository();

        [HttpGet]
        [Route("list")]
        public IHttpActionResult GetStores()
        {
            try
            {
                var stores = repository.GetStores();
                return Ok(stores);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("productlist")]
        public IHttpActionResult GetAllProducts()
        {
            try
            {
                var products = repository.GetAllProducts2();
                return Ok(products);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("storetypes")]
        public IHttpActionResult GetStoreTypes()
        {
            try
            {
                var types = repository.GetActiveStoreTypes();
                return Ok(types);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("add-with-products")]
        public IHttpActionResult AddStoreWithProducts()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;

          
                var modelJson = httpRequest.Form["model"];
                if (string.IsNullOrEmpty(modelJson))
                    return BadRequest("Model data is required.");

                var model = JsonConvert.DeserializeObject<AddStoreWithProductsModel>(modelJson);
                if (model == null || model.Products == null || !model.Products.Any())
                    return BadRequest("Invalid store or product data.");

             
                for (int i = 0; i < model.Products.Count; i++)
                {
                    var fileKey = $"productImage_{i}";
                    var file = httpRequest.Files[fileKey];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var savePath = HttpContext.Current.Server.MapPath("~/Content/images/");
                        if (!Directory.Exists(savePath))
                            Directory.CreateDirectory(savePath);

                        var fullPath = Path.Combine(savePath, fileName);
                        file.SaveAs(fullPath);

                        model.Products[i].ImagePath = fileName;
                    }
                    else
                    {
                        model.Products[i].ImagePath = null;
                    }
                }

               
                string errorMessage;
                bool result = repository.AddStoreWithProducts(
                    model.StoreName,
                    model.StoreTypeId,
                    model.CreatedBy,
                    null, 
                    model.Products.Select(p => new ProductInputModel
                    {
                        ProductId = p.ProductId,
                        StorePrice = p.StorePrice,
                        Stock = p.Stock,
                        ImagePath = p.ImagePath
                    }).ToList(),
                    out errorMessage
                );

                if (!result)
                    return Content(HttpStatusCode.BadRequest, new { message = errorMessage });

                return Ok(new { message = "Store and products added successfully." });
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("get-with-products/{storeName}")]
        public IHttpActionResult GetStoreWithProducts(string storeName)
        {
            try
            {
                if (string.IsNullOrEmpty(storeName))
                    return BadRequest("Store name is required.");

                var result = repository.GetStoreWithProducts(storeName);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("update-with-products")]
        public IHttpActionResult UpdateStoreWithProducts()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;

               
                var modelJson = httpRequest.Form["model"];
                if (string.IsNullOrEmpty(modelJson))
                    return BadRequest("Model data is required.");

                var model = JsonConvert.DeserializeObject<AddStoreWithProductsModel>(modelJson);
                if (model == null || model.Products == null)
                    return BadRequest("Invalid store or product data.");

               
                for (int i = 0; i < model.Products.Count; i++)
                {
                    var fileKey = $"productImage_{i}";
                    var file = httpRequest.Files[fileKey];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var savePath = HttpContext.Current.Server.MapPath("~/Content/images/");
                        if (!Directory.Exists(savePath))
                            Directory.CreateDirectory(savePath);

                        var fullPath = Path.Combine(savePath, fileName);
                        file.SaveAs(fullPath);

                        model.Products[i].ImagePath = fileName;
                    }
                    else
                    {
                        
                        model.Products[i].ImagePath = model.Products[i].ImagePath ?? "";
                    }
                }

               
                string errorMessage;
                bool result = repository.UpdateStoreWithProducts(
                    model.StoreName,
                    model.StoreTypeId,
                    model.CreatedBy,
                    model.Products.Select(p => new ProductInputModel
                    {
                        ProductId = p.ProductId,
                        StorePrice = p.StorePrice,
                        Stock = p.Stock,
                        ImagePath = p.ImagePath
                    }).ToList(),
                    out errorMessage
                );

                if (!result)
                    return Content(HttpStatusCode.BadRequest, new { message = errorMessage });

                return Ok(new { message = "Store and products updated successfully." });
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("delete/{storeName}")]
        public IHttpActionResult DeleteStore(string storeName)
        {
            try
            {
                if (string.IsNullOrEmpty(storeName))
                {
                    return BadRequest("Store Name is required");
                }

                bool result = repository.DeleteStore(storeName);

                if (!result)
                {
                    return Content(HttpStatusCode.NotFound, new { success = false, message = "Store not found" });
                }

          
                return Ok(new { success = true, message = "Store deleted successfully" });
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("exists")]
        public IHttpActionResult CheckStoreExists(string storeName)
        {
            try
            {
                bool exists = repository.CheckStoreExists(storeName);
                return Ok(exists); 
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }

    }
}
