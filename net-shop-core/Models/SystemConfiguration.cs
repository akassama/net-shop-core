using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net_shop_core.Models
{
    public class SystemConfiguration
    {
        public int shopPageSize { get; set; }
        public int newArrivalPageSize { get; set; }
        public int categoryPageSize { get; set; }
        public int totalHomeProducts { get; set; }
        public int searchPageSize { get; set; }
        public int defaultProductApproveStatus { get; set; }
        public int uploadImageDefaultHeight { get; set; }
        public int uploadImageDefaultWidth { get; set; }
        public int defaultProductStock { get; set; }
        public string textWaterMark { get; set; }
        public string imageWatermark { get; set; }
        public string connectionString { get; set; }
        public bool logSearches { get; set; }
        public bool logTagClicks { get; set; }
        public bool logCategoryClicks { get; set; }
        public bool logActivity { get; set; }
    }
}
