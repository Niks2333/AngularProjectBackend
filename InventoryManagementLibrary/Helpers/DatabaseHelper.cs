using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace InventoryManagementLibrary.Helpers
{
    public static class DatabaseHelper
    {
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
    }
}
