using System.Web;
using InventoryManagementLibrary.Models;

namespace InventoryManagement.Models
{
    public class WebEditStockViewModel : StoreProductViewModel
    {
        public HttpPostedFileBase ImageFile { get; set; }
    }
}
