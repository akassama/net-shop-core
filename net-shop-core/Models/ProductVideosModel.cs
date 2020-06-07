using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace net_shop_core.Models
{
    [Table("ProductVideos")]
    public class ProductVideosModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Product ID")]
        public int? ProductID { get; set; }

        [Display(Name = "Video Type")]
        public string VideoType { get; set; }

        [Display(Name = "Video Link")]
        public string VideoLink { get; set; }

        [Display(Name = "Video Description")]
        public string VideoDescription { get; set; }

        [Display(Name = "Date Added")]
        public DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}
