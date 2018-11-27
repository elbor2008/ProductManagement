using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Data;
using ProductManagement.Extensions;
using ProductManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProductManagement.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            List<Products> productsList = await _db.Products.ToListAsync();
            return View(productsList);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Products products = await _db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags)
                .Where(p => p.Id == id).FirstOrDefaultAsync();
            return View(products);
        }

        [HttpPost, ActionName("Detail")]
        [ValidateAntiForgeryToken]
        public IActionResult DetailPost(int id)
        {
            List<int> idList = HttpContext.Session.Get<List<int>>("shopping_cart") ?? new List<int>();
            idList.Add(id);
            HttpContext.Session.Set("shopping_cart", idList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            List<int> idList = HttpContext.Session.Get<List<int>>("shopping_cart");
            if (idList != null && idList.Count > 0)
            {
                if (idList.Contains(id))
                {
                    idList.Remove(id);
                    HttpContext.Session.Set("shopping_cart", idList);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}