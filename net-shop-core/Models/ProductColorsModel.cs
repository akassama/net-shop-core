using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_shop_core.Models
{
    [Table("ProductColors")]
    public class ProductColorsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Product ID")]
        public int? ProductID { get; set; }

        [Display(Name = "Color Name")]
        public string ColorName { get; set; }

        [Display(Name = "Color Code")]
        public string ColorCode { get; set; }

        [Display(Name = "Date Added")]
        public DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}
