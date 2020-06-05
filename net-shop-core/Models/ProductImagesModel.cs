using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModestLiving.Models
{
    [Table("ProductImages")]
    public class ProductImagesModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Product ID")]
        public int? ProductID { get; set; }

        [Display(Name = "Image Type")]
        public string ImageType { get; set; }

        [Display(Name = "Image Link")]
        public string ImageLink { get; set; }

        [Display(Name = "ImageDescription")]
        public string ImageDescription { get; set; }

        [Display(Name = "Date Added")]
        public DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}
