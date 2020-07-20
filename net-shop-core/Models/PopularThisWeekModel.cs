using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_shop_core.Models
{
    [Table("vwPopularThisWeek")]
    public class PopularThisWeekModel
    {
        [Key]
        [Display(Name = "Product ID")]
        public string ProductID { get; set; }

        [Display(Name = "Value Occurrence")]
        public int ValueOccurrence { get; set; }

    }
}
