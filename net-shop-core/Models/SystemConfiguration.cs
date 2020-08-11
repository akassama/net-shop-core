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
        public string textWaterMark { get; set; }
        public string imageWaterMark { get; set; }
    }
}
