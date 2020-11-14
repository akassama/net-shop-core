using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_shop_core.Models
{
    /* ACTIVITY LOGS MODEL */
    [Table("ActivityLogs")]
    public class ActivityLogsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Activity User")]
        public string ActivityUser { get; set; }

        [Display(Name = "Action By")]
        public string ActionBy { get; set; }

        [Display(Name = "Log Type")]
        public string LogType { get; set; }

        [Required]
        [Display(Name = "Action")]
        public string Action { get; set; }

        [Display(Name = "Activity Date")]
        public DateTime? ActivityDate { get; set; } = DateTime.Now;
    }
}
