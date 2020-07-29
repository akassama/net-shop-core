using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_shop_core.Models
{
    [Table("Stores")]
    public class StoresModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Account ID")]
        public string AccountID { get; set; }

        [Required]
        [Display(Name = "Store Name")]
        public string StoreName { get; set; }

        [Required]
        [Display(Name = "Store Description")]
        public string StoreDescription { get; set; }

        [Required]
        [Display(Name = "Store Location")]
        public string StoreLocation { get; set; }

        [Display(Name = "Store Location Map")]
        public string StoreLocationMap { get; set; }

        [Display(Name = "Date Added")]
        public DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}
