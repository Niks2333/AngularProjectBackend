using System.Collections.Generic;
namespace InventoryManagementLibrary.Models
{
    public class StoreProductViewModel
    {
        public int StockId { get; set; }
        public string StoreName { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public decimal StorePrice { get; set; }
        public int Stock { get; set; }
        public List<StoreProductViewModel> Products { get; set; }
        public List<string> SelectedCategories { get; set; }
        public List<string> Categories { get; set; }  
        public string SelectedCategory { get; set; }
        public string Search { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public int TotalCount { get; set; }
        public int StoreProductId { get; set; }
        public string ImagePath { get; set; }


    }
}
