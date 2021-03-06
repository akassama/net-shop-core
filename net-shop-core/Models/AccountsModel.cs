﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_shop_core.Models
{
    [Table("Accounts")]
    public class AccountsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [RegularExpression(@"^[A-Za-z 0-9]{10,20}$", ErrorMessage = "Minimum 10 characters required, and maximum of 20 characters.")]
        [Display(Name = "Account ID")]
        public string AccountID { get; set; }

        [RegularExpression(@"^[A-Za-z 0-9]{2,50}$", ErrorMessage = "Minimum 2 characters required, and maximum of 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [RegularExpression(@"^[A-Za-z 0-9]{2,50}$", ErrorMessage = "Minimum 2 characters required, and maximum of 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [RegularExpression(@"^[A-Za-z 0-9]{2,50}$", ErrorMessage = "Minimum 2 characters required, and maximum of 50 characters.")]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [RegularExpression(@"^[\d]{1,4}$", ErrorMessage = "Only numbers allowed. 4 digits max")]
        [Display(Name = "Country Code")]
        public int? CountryCode { get; set; }

        [RegularExpression(@"^[\d]{7,7}$", ErrorMessage = "Only phone numbers allowed. 7 digit numbers")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression("(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{6,}", ErrorMessage = "Must contain at least one number and one uppercase and lowercase letter, and at least 6 or more characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [RegularExpression(@"^[A-Za-z 0-9]{5,250}$", ErrorMessage = "Minimum 5 characters required, and maximum of 250 characters.")]
        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }


        [RegularExpression(@"^[A-Za-z 0-9]{5,250}$", ErrorMessage = "Minimum 5 characters required, and maximum of 250 characters.")]
        [Display(Name = "Directory Name")]
        public string DirectoryName { get; set; }

        [RegularExpression(@"^[\d]{0,1}$", ErrorMessage = "Only numbers allowed. 0 or 1")]
        [Display(Name = "Status")]
        public int? Status { get; set; }

        [RegularExpression(@"^[0-1]$", ErrorMessage = "Only numbers allowed. 0 or 1")]
        [Display(Name = "Oauth")]
        public int? Oauth { get; set; }

        [RegularExpression(@"^[0-1]$", ErrorMessage = "Only numbers allowed. 0 or 1")]
        [Display(Name = "Account Verification")]
        public int? AccountVerification { get; set; }

        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }

        [Display(Name = "Update Date")]
        public DateTime UpdateDate { get; set; }

        [Display(Name = "Date Added")]
        public DateTime? DateAdded { get; set; } = DateTime.Now;
    }


    public class LoginViewModel
    {
        [Key]
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression("(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{6,}", ErrorMessage = "Must contain at least one number and one uppercase and lowercase letter, and at least 6 or more characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

}