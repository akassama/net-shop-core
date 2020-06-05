using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModestLiving.Models
{
    [Table("OrderDetails")]
    public class OrderDetailsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only product id (integer) allowed.")]
        [Display(Name = "Product ID")]
        public string ProductID { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z 0-9]{2,150}$", ErrorMessage = "Minimum 2 characters required, and maximum of 150 characters.")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Required]
        [RegularExpression(@"[+-]?([0-9]*[.])?[0-9]+", ErrorMessage = "Only numbers allowed.")]
        [Display(Name = "Product Price")]
        public string ProductPrice { get; set; }

        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only numbers(integers) allowed.")]
        [Display(Name = "Product Quantity")]
        public string ProductQuantity { get; set; }

        [Display(Name = "Date Added")]
        public DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}