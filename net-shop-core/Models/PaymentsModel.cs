using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModestLiving.Models
{
    [Table("Payments")]
    public class PaymentsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only numbers allowed.")]
        [Display(Name = "Order ID")]
        public int OrderID { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z 0-9]{10,250}$", ErrorMessage = "Minimum 10 characters required, and maximum of 250 characters.")]
        [Display(Name = "Customer Account ID")]
        public string CustomerAccountID { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z 0-9]{10,250}$", ErrorMessage = "Minimum 10 characters required, and maximum of 250 characters.")]
        [Display(Name = "Staff Account ID")]
        public string StaffAccountID { get; set; }

        [Required]
        [RegularExpression(@"[+-]?([0-9]*[.])?[0-9]+", ErrorMessage = "Only numbers allowed.")]
        [Display(Name = "Ammount")]
        public string Ammount { get; set; }

        [Display(Name = "Date Added")]
        public DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}