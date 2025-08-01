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
        [HttpGet]
        [Route("list")]
        public IHttpActionResult GetStores()
        {
            try
            {   

                var repo = new StoreRepository();
                var stores = repo.GetStores();
                return Ok(stores);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.Message, ex.StackTrace, 0);
                return InternalServerError(ex);
            }
        }
    }

}
