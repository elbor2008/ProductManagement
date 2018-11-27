using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Data;
using ProductManagement.Extensions;
using ProductManagement.Models;
using ProductManagement.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProductManagement.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty] public ShoppingCartViewModel ShoppingCartVm { get; set; }

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCartVm = new ShoppingCartViewModel()
            {
                Products = new List<Products>()
            };
        }

        public async Task<IActionResult> Index()
        {
            List<int> idList = HttpContext.Session.Get<List<int>>("shopping_cart");
            if (idList != null && idList.Count > 0)
            {
                foreach (int id in idList)
                {
                    Products products = await _db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags)
                        .FirstOrDefaultAsync(p => p.Id == id);
                    ShoppingCartVm.Products.Add(products);
                }
            }

            return View(ShoppingCartVm);
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexPost()
        {
            if (!ModelState.IsValid)
            {
                List<int> ids = HttpContext.Session.Get<List<int>>("shopping_cart");
                if (ids != null && ids.Count > 0)
                {
                    ShoppingCartVm.Products = new List<Products>();
                    foreach (int id in ids)
                    {
                        Products products = await _db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags)
                            .FirstOrDefaultAsync(p => p.Id == id);
                        ShoppingCartVm.Products.Add(products);
                    }
                }

                return View(ShoppingCartVm);
            }

            Appointments appointments = ShoppingCartVm.Appointments;
            appointments.AppointmentDate = appointments.AppointmentDate.AddHours(appointments.AppointmentTime.Hour)
                .AddMinutes(appointments.AppointmentTime.Minute).AddSeconds(appointments.AppointmentTime.Second);
            _db.Appointments.Add(appointments);
            await _db.SaveChangesAsync();

            List<int> idList = HttpContext.Session.Get<List<int>>("shopping_cart");
            foreach (int id in idList)
            {
                ProductsAppointments productsAppointments = new ProductsAppointments()
                {
                    AppointmentId = appointments.Id,
                    ProductId = id
                };
                _db.ProductsAppointments.Add(productsAppointments);
            }

            await _db.SaveChangesAsync();
            HttpContext.Session.Set("shopping_cart", new List<int>());

            return RedirectToAction(nameof(AppointmentConfirmation), new {id = appointments.Id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            List<int> ids = HttpContext.Session.Get<List<int>>("shopping_cart");
            if (ids != null && ids.Count > 0)
            {
                if (ids.Contains(id))
                {
                    ids.Remove(id);
                    HttpContext.Session.Set("shopping_cart", ids);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AppointmentConfirmation(int id)
        {
            ShoppingCartVm.Appointments = _db.Appointments.Find(id);

            List<ProductsAppointments> paList = _db.ProductsAppointments.Where(m => m.AppointmentId == id).ToList();
            foreach (ProductsAppointments pa in paList)
            {
                ShoppingCartVm.Products.Add(_db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags)
                    .FirstOrDefault(p => p.Id == pa.ProductId));
            }

            return View(ShoppingCartVm);
        }
    }
}