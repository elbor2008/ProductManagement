using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Models
{
    public class Appointments
    {
        public int Id { get; set; }

        public DateTime AppointmentDate { get; set; }

        [NotMapped] public DateTime AppointmentTime { get; set; }

        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }

        public bool IsConfirmed { get; set; }

        [Display(Name = "Sales Person")] public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")] public virtual ApplicationUser ApplicationUser { get; set; }
    }
}