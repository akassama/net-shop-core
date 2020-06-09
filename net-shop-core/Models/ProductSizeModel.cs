﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModestLiving.Models
{
    [Table("ProductSize")]
    public class ProductSizeModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Product ID")]
        public int? ProductID { get; set; }

        [Display(Name = "Size")]
        public string Size { get; set; }

        [Display(Name = "Date Added")]
        public DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}