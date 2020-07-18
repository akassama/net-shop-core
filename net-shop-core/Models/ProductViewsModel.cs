using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_shop_core.Models
{
    [Table("ProductViews")]
    public class ProductViewsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Product ID")]
        public int? ProductID { get; set; }

        [Display(Name = "Visitor ID")]
        public string VisitorID { get; set; }

        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Browser")]
        public string Browser { get; set; }

        [Display(Name = "Device")]
        public string Device { get; set; }

        [Display(Name = "Visit Date")]
        public DateTime? VisitDate { get; set; } = DateTime.Now;

        [Display(Name = "OtherDetails")]
        public string OtherDetails { get; set; }
    }
}

