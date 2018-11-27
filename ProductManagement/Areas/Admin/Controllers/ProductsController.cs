using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Models.ViewModel;
using ProductManagement.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProductManagement.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;

        [BindProperty] public ProductsViewModel ProductsVm { get; set; }

        public ProductsController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            ProductsVm = new ProductsViewModel()
            {
                ProductTypes = _db.ProductTypes.ToList(),
                SpecialTags = _db.SpecialTags.ToList(),
                Products = new Products()
            };
        }

        public async Task<IActionResult> Index()
        {
            //            List<Products> products =
            //                await _db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags).ToListAsync();
            List<Products> products = await _db.Products.ToListAsync();
            return View(products);
        }

        public IActionResult Create()
        {
            return View(ProductsVm);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            if (!ModelState.IsValid)
            {
                return View(ProductsVm);
            }

            _db.Products.Add(ProductsVm.Products);
            await _db.SaveChangesAsync();

            Products productsFromDb = _db.Products.Find(ProductsVm.Products.Id);

            string rootPath = _hostingEnvironment.WebRootPath;
            IFormFileCollection files = HttpContext.Request.Form.Files;
            string upload = Path.Combine(rootPath, SD.ImageFolder);
            if (files.Count != 0)
            {
                string extension = Path.GetExtension(files[0].FileName);
                using (FileStream fileStream =
                    new FileStream(upload + @"\" + productsFromDb.Id + extension, FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + productsFromDb.Id + extension;
            }
            else
            {
                System.IO.File.Copy(upload + @"\" + SD.DefaultProductImage, upload + @"\" + productsFromDb.Id + ".jpg");

                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + productsFromDb.Id + ".jpg";
            }

            _db.Products.Update(productsFromDb);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductsVm.Products = await _db.Products.FindAsync(id);
            if (ProductsVm.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit()
        {
            if (!ModelState.IsValid)
            {
                return View(ProductsVm);
            }

            Products productsFromDb = await _db.Products.FindAsync(ProductsVm.Products.Id);

            IFormFileCollection files = HttpContext.Request.Form.Files;
            if (files.Count != 0)
            {
                string rootPath = _hostingEnvironment.WebRootPath;
                string upload = Path.Combine(rootPath, SD.ImageFolder);
                string oldExtension = Path.GetExtension(productsFromDb.Image);
                string newExtension = Path.GetExtension(files[0].FileName);

                string oldPic = upload + @"\" + productsFromDb.Id + oldExtension;
                if (System.IO.File.Exists(oldPic))
                {
                    System.IO.File.Delete(oldPic);
                }

                using (FileStream fileStream =
                    new FileStream(upload + @"\" + productsFromDb.Id + newExtension, FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + productsFromDb.Id + newExtension;
            }

            productsFromDb.Name = ProductsVm.Products.Name;
            productsFromDb.Price = ProductsVm.Products.Price;
            productsFromDb.Available = ProductsVm.Products.Available;
            productsFromDb.ShadeColor = ProductsVm.Products.ShadeColor;
            productsFromDb.ProductTypeId = ProductsVm.Products.ProductTypeId;
            productsFromDb.SpecialTagId = ProductsVm.Products.SpecialTagId;

            _db.Products.Update(productsFromDb);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductsVm.Products = await _db.Products.FindAsync(id);
            return View(ProductsVm);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductsVm.Products = await _db.Products.FindAsync(id);
            return View(ProductsVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Products productsFromDb = await _db.Products.FindAsync(id);
            if (productsFromDb == null)
            {
                return NotFound();
            }

            string rootPath = _hostingEnvironment.WebRootPath;
            string upload = Path.Combine(rootPath, SD.ImageFolder);
            string extension = Path.GetExtension(productsFromDb.Image);
            string pic = upload + @"\" + productsFromDb.Id + extension;
            if (System.IO.File.Exists(pic))
            {
                System.IO.File.Delete(pic);
            }

            _db.Products.Remove(productsFromDb);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}