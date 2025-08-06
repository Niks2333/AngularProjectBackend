using InventoryManagementLibrary.DAL;
using InventoryManagementLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InventoryManagementBackend.Controllers
{
    [RoutePrefix("api/store")]
    public class StoreController : ApiController
    {
        private readonly StoreRepository repo = new StoreRepository();

       
        [HttpGet]
        [Route("list")]
        public IHttpActionResult GetStores()
        {
            try
            {
                var stores = repo.GetStores();
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
                var products = repo.GetAllProducts2();
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
                var types = repo.GetActiveStoreTypes();
                return Ok(types);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }
    }
}
