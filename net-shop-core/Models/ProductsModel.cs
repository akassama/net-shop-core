using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_shop_core.Models
{
    [Table("Products")]
    public class ProductsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Product ID")]
        public string ProductID { get; set; }

        [Display(Name = "Account ID")]
        [RegularExpression(@"^[A-Za-z 0-9]{10,250}$", ErrorMessage = "Minimum 10 characters required, and maximum of 250 characters.")]
        public string AccountID { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Required]
        [Display(Name = "Product Description")]
        public string ProductDescription { get; set; }

        [Display(Name = "Unique Product Name")]
        public string UniqueProductName { get; set; }

        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only product id (integer) allowed.")]
        [Display(Name = "Product Type")]
        public int ProductType { get; set; }

        [Display(Name = "Wholesale Quantity")]
        public int? WholeSaleQuantity { get; set; }

        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only store id (integer) allowed.")]
        [Display(Name = "Store")]
        public int StoreID { get; set; }

        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only category id (integer) allowed.")]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        [Display(Name = "Currency")]
        public string Currency { get; set; }

        [Required]
        [RegularExpression(@"[+-]?([0-9]*[.])?[0-9]+", ErrorMessage = "Only numbers allowed.")]
        [Display(Name = "Product Price")]
        public string ProductPrice { get; set; }

        [RegularExpression(@"[+-]?([0-9]*[.])?[0-9]+", ErrorMessage = "Only numbers allowed.")]
        [Display(Name = "Product Previous Price")]
        public string ProductPreviousPrice { get; set; }

        [Display(Name = "Is Featured?")]
        [RegularExpression(@"^[0-1]", ErrorMessage = "Only 0 or 1 allowed.")]
        public int? FeaturedPost { get; set; }

        [Required]
        [RegularExpression(@"^.{2,250}$", ErrorMessage = "Minimum 2 characters required, and maximum of 250 characters.")]
        [Display(Name = "Product Tags")]
        public string ProductTags { get; set; }

        [Display(Name = "Approve Status")]
        [RegularExpression(@"^[0-1]", ErrorMessage = "Only 0 or 1 allowed.")]
        public int? ApproveStatus { get; set; }

        [Display(Name = "Updated By")]
        [RegularExpression(@"^[A-Za-z 0-9]{2,150}$", ErrorMessage = "Minimum 2 characters required, and maximum of 150 characters.")]
        public string UpdatedBy { get; set; }

        [Display(Name = "Update Date")]
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Date Added")]
        public DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}