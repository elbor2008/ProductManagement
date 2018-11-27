using System.Collections.Generic;

namespace ProductManagement.Models.ViewModel
{
    public class AppointmentsDetailViewModel
    {
        public Appointments Appointments { get; set; }
        public List<ApplicationUser> ApplicationUserList { get; set; }
        public List<Products> ProductsList { get; set; }
    }
}