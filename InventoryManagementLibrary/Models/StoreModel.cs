using System;

namespace InventoryManagementLibrary.Models
{
    public class StoreModel
    {

        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public byte[] StoreImage { get; set; }
        public int StoreTypeId { get; set; }
    }


}
