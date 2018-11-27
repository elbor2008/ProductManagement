using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductManagement.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminUsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.ApplicationUsers.ToList());
        }

        public IActionResult Edit(string id)
        {
            return View(_db.ApplicationUsers.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser applicationUser)
        {
            if (!ModelState.IsValid)
            {
                return View(applicationUser);
            }

            if (id != applicationUser.Id)
            {
                return NotFound();
            }

            ApplicationUser userFromDb = await _db.ApplicationUsers.FindAsync(id);
            if (userFromDb == null)
            {
                return NotFound();
            }

            userFromDb.Name = applicationUser.Name;
            userFromDb.PhoneNumber = applicationUser.PhoneNumber;
            _db.ApplicationUsers.Update(userFromDb);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(string id)
        {
            return View(_db.ApplicationUsers.Find(id));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(string id)
        {
            ApplicationUser userFromDb = await _db.ApplicationUsers.FindAsync(id);
            if (userFromDb == null)
            {
                return NotFound();
            }

            userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            _db.ApplicationUsers.Update(userFromDb);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}