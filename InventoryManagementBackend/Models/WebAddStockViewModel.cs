using InventoryManagementLibrary.Models;
using System.Web;

namespace InventoryManagement.Models
{
    public class WebAddStockViewModel : AddStockViewModel
    {
        public HttpPostedFileBase ImageFile { get; set; }
    }
}
