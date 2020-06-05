using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModestLiving.Models
{
    [Table("Accounts")]
    public class AccountsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z 0-9]{10,20}$", ErrorMessage = "Minimum 10 characters required, and maximum of 20 characters.")]
        [Display(Name = "Account ID")]
        public string AccountID { get; set; }

        [Required]
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
       //[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid.")]
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

        [RegularExpression(@"^[A-Za-z 0-9]{4,20}$", ErrorMessage = "Minimum 4 characters required, and maximum of 20 characters.")]
        [Display(Name = "Salt")]
        public string Salt { get; set; }

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


    [Table("LoginInfo")]
    public class LoginInfoModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Last Login")]
        public DateTime? LastLogin { get; set; }

        [Required]
        [Display(Name = "Failed Login Count")]
        public int FailedLoginCount { get; set; }

        [Required]
        [RegularExpression(@"^[0-1]", ErrorMessage = "Only 0 or 1 allowed.")]
        [Display(Name = "Locked Status")]
        public int LockedStatus { get; set; }

        [Display(Name = "LockPeriod")]
        public DateTime? LockPeriod { get; set; }

        [Required]
        [Display(Name = "Total Logins")]
        public int TotalLogins { get; set; }

        [Required]
        [Display(Name = "Login Session ID")]
        public string LoginSessionID { get; set; }

        [Required]
        [Display(Name = "First Login")]
        public DateTime FirstLogin { get; set; } = DateTime.Now;
    }

    [Table("ActivityLog")]
    public class ActivityLogModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "ActivityType")]
        public string ActivityType { get; set; }

        [Display(Name = "Activity Description")]
        public string ActivityDescription { get; set; }

        [Display(Name = "Link")]
        public string Link { get; set; }

        [Display(Name = "ActionID")]
        public string ActionID { get; set; }

        [Required]
        [Display(Name = "ActivityDate")]
        public DateTime ActivityDate { get; set; } = DateTime.Now;
    }

}