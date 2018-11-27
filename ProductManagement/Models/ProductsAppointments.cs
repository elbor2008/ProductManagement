using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Models
{
    public class ProductsAppointments
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int AppointmentId { get; set; }

        [ForeignKey("ProductId")] public virtual Products Products { get; set; }
        [ForeignKey("AppointmentId")] public virtual Appointments Appointments { get; set; }
    }
}