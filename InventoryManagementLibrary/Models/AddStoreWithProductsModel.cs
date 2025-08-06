using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementLibrary.Models
{
    public class ProductInsertModel
    {
        public int ProductId { get; set; }
        public decimal StorePrice { get; set; }
        public int Stock { get; set; }
        public string ImagePath { get; set; }
    }

    public class StoreWithProductsModel
    {
        public string StoreName { get; set; }
        public int StoreTypeId { get; set; }
        public string CreatedBy { get; set; }
        public byte[] StoreImage { get; set; }  
        public List<ProductInsertModel> Products { get; set; }
    }
}
