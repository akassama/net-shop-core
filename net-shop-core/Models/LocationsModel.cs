using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_shop_core.Models
{
    [Table("Locations")]
    public class LocationsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z 0-9]{2,150}$", ErrorMessage = "Minimum 2 characters required, and maximum of 150 characters.")]
        [Display(Name = "Location Name")]
        public string LocationName { get; set; }

        [Display(Name = "Google Map Link")]
        public string GoogleMapLink { get; set; }

        [Display(Name = "Latitude")]
        public string Latitude { get; set; }

        [Display(Name = "Longitude")]
        public string Longitude { get; set; }

        [Display(Name = "Date Added")]
        public DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}