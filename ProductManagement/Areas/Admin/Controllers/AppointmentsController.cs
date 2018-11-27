using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Models.ViewModel;
using ProductManagement.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProductManagement.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser + "," + SD.AdminEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly int _pageSize = 3;
        public AppointmentsViewModel AppointmentsVm { get; set; }

        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
            AppointmentsVm = new AppointmentsViewModel()
            {
                AppointmentsList = new List<Appointments>()
            };
        }

        public IActionResult Index(int currentPage = 1, string customerName = null, string email = null,
            string phoneNumber = null,
            string appointmentDate = null)
        {
            if (User.IsInRole(SD.SuperAdminEndUser))
            {
                AppointmentsVm.AppointmentsList = _db.Appointments.Include(a => a.ApplicationUser).ToList();
            }
            else
            {
                IIdentity userIdentity = User.Identity;
                ClaimsIdentity claimsIdentity = (ClaimsIdentity) userIdentity;
                Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                string userId = claim.Value;

                AppointmentsVm.AppointmentsList = _db.Appointments.Include(a => a.ApplicationUser)
                    .Where(a => a.ApplicationUserId == userId).ToList();
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("/Admin/Appointments?currentPage=:");

            if (!string.IsNullOrEmpty(customerName))
            {
                sb.Append("&customerName=" + customerName);

                AppointmentsVm.AppointmentsList = AppointmentsVm.AppointmentsList
                    .Where(a => a.CustomerName.ToLower().Contains(customerName.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(email))
            {
                sb.Append("&email=" + email);

                AppointmentsVm.AppointmentsList = AppointmentsVm.AppointmentsList
                    .Where(a => a.CustomerEmail.ToLower().Contains(email.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                sb.Append("&phoneNumber=" + phoneNumber);

                AppointmentsVm.AppointmentsList = AppointmentsVm.AppointmentsList
                    .Where(a => a.CustomerPhoneNumber.ToLower().Contains(phoneNumber.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(appointmentDate))
            {
                try
                {
                    sb.Append("&appointmentDate=" + appointmentDate);

                    DateTime appDate = Convert.ToDateTime(appointmentDate);
                    AppointmentsVm.AppointmentsList = AppointmentsVm.AppointmentsList
                        .Where(a => a.AppointmentDate.ToShortDateString().Equals(appDate.ToShortDateString())).ToList();
                }
                catch (Exception ex)
                {
                }
            }
            int count = AppointmentsVm.AppointmentsList.Count();
            AppointmentsVm.AppointmentsList = AppointmentsVm.AppointmentsList.OrderBy(a => a.AppointmentDate)
                .Skip((currentPage - 1) * _pageSize)
                .Take(_pageSize).ToList();

            AppointmentsVm.PagingInfo = new PagingInfo()
            {
                CurrentPage = currentPage,
                TotalItems = count,
                ItemsPerPage = _pageSize,
                UrlParam = sb.ToString()
            };

            return View(AppointmentsVm);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //            List<Products> productsList = (from p in _db.Products
            //                join pa in _db.ProductsAppointments on p.Id equals pa.ProductId
            //                where pa.AppointmentId == id
            //                select p).Include(p => p.ProductTypes).ToList();
            List<Products> productsList = (from pa in _db.ProductsAppointments
                where pa.AppointmentId == id
                select pa.Products).Include(p => p.ProductTypes).ToList();

            AppointmentsDetailViewModel appointmentsDetailDv = new AppointmentsDetailViewModel()
            {
                Appointments = _db.Appointments.Find(id),
                ApplicationUserList = _db.ApplicationUsers.ToList(),
                ProductsList = productsList
            };
            return View(appointmentsDetailDv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AppointmentsDetailViewModel appointmentsDetailDv)
        {
            if (id != appointmentsDetailDv.Appointments.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(appointmentsDetailDv);
            }

            Appointments appointments = _db.Appointments.Find(id);
            if (appointments == null)
            {
                return NotFound();
            }

            appointments.CustomerName = appointmentsDetailDv.Appointments.CustomerName;
            appointments.AppointmentDate = appointmentsDetailDv.Appointments.AppointmentDate
                .AddHours(appointmentsDetailDv.Appointments.AppointmentTime.Hour)
                .AddMinutes(appointmentsDetailDv.Appointments.AppointmentTime.Minute)
                .AddSeconds(appointmentsDetailDv.Appointments.AppointmentTime.Second);
            appointments.CustomerPhoneNumber = appointmentsDetailDv.Appointments.CustomerPhoneNumber;
            appointments.CustomerEmail = appointmentsDetailDv.Appointments.CustomerEmail;
            appointments.IsConfirmed = appointmentsDetailDv.Appointments.IsConfirmed;
            if (User.IsInRole(SD.SuperAdminEndUser))
            {
                appointments.ApplicationUserId = appointmentsDetailDv.Appointments.ApplicationUserId;
            }

            _db.Appointments.Update(appointments);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //            List<Products> productsList = (from p in _db.Products
            //                join pa in _db.ProductsAppointments on p.Id equals pa.ProductId
            //                where pa.AppointmentId == id
            //                select p).Include(p => p.ProductTypes).ToList();
            List<Products> productsList = (from pa in _db.ProductsAppointments
                where pa.AppointmentId == id
                select pa.Products).Include(p => p.ProductTypes).ToList();

            AppointmentsDetailViewModel appointmentsDetailDv = new AppointmentsDetailViewModel()
            {
                Appointments = _db.Appointments.Find(id),
                ApplicationUserList = _db.ApplicationUsers.ToList(),
                ProductsList = productsList
            };
            return View(appointmentsDetailDv);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //            List<Products> productsList = (from p in _db.Products
            //                join pa in _db.ProductsAppointments on p.Id equals pa.ProductId
            //                where pa.AppointmentId == id
            //                select p).Include(p => p.ProductTypes).ToList();
            List<Products> productsList = (from pa in _db.ProductsAppointments
                where pa.AppointmentId == id
                select pa.Products).Include(p => p.ProductTypes).ToList();

            AppointmentsDetailViewModel appointmentsDetailDv = new AppointmentsDetailViewModel()
            {
                Appointments = _db.Appointments.Find(id),
                ApplicationUserList = _db.ApplicationUsers.ToList(),
                ProductsList = productsList
            };
            return View(appointmentsDetailDv);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Appointments appointments = _db.Appointments.Find(id);
            _db.Appointments.Remove(appointments);
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}