using System.Web;
using InventoryManagementLibrary.Models;

using System.Collections.Generic;

namespace InventoryManagement.Models
{
    public class WebStoreModel : StoreModel
    {
        public HttpPostedFileBase ImageFile { get; set; }
        public List<StoreTypeModel> StoreTypes { get; set; }

    }
}