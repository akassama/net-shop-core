using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModestLiving.Models
{
    [Table("Orders")]
    public class OrdersModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z 0-9]{10,250}$", ErrorMessage = "Minimum 10 characters required, and maximum of 250 characters.")]
        [Display(Name = "Account ID")]
        public string AccountID { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z 0-9]{2,150}$", ErrorMessage = "Minimum 2 characters required, and maximum of 150 characters.")]
        [Display(Name = "Delivery Location")]
        public string DeliveryLocation { get; set; }

        [RegularExpression(@"^[A-Za-z 0-9]{2,150}$", ErrorMessage = "Minimum 2 characters required, and maximum of 150 characters.")]
        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Only numbers allowed.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [RegularExpression(@"^[0-1]", ErrorMessage = "Only 0 or 1 allowed.")]
        [Display(Name = "Order Status")]
        public int OrderStatus { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Only numbers allowed.")]
        [Display(Name = "Failed Deliveries")]
        public int FailedDeliveries { get; set; }

        [Display(Name = "Date Added")]
        public DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}