using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_shop_core.Models
{
    [Table("ProductTags")]
    public class ProductTagsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z 0-9]{2,50}$", ErrorMessage = "Minimum 2 characters required, and maximum of 50 characters.")]
        [Display(Name = "Tag Name")]
        public string TagName { get; set; }

        [RegularExpression(@"^[A-Za-z 0-9]{2,50}$", ErrorMessage = "Minimum 2 characters required, and maximum of 50 characters.")]
        [Display(Name = "Short Tag Name")]
        public string ShortTagName { get; set; }

        [Display(Name = "Date Added")]
        public DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}