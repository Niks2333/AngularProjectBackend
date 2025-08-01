using System.Collections.Generic;

namespace InventoryManagementLibrary.Models
{
    public class AddStockViewModel
    {
        public string StoreName { get; set; }
        public string ProductName { get; set; }
        public decimal StorePrice { get; set; }
        public int Stock { get; set; }
        public int StoreProductId { get; set; }
        public List<string> AvailableProducts { get; set; }
        public string ImagePath { get; set; }
        
    }
}
