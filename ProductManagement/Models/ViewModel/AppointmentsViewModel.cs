using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Models.ViewModel
{
    public class AppointmentsViewModel
    {
        public List<Appointments> AppointmentsList { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}