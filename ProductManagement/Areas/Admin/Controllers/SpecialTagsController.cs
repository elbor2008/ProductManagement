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
    public class SpecialTagsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SpecialTagsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.SpecialTags.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTags specialTags)
        {
            if (ModelState.IsValid)
            {
                _db.SpecialTags.Add(specialTags);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(specialTags);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTags = await _db.SpecialTags.FindAsync(id);
            if (specialTags == null)
            {
                return NotFound();
            }

            return View(specialTags);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTags specialTags)
        {
            if (id != specialTags.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.SpecialTags.Update(specialTags);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(specialTags);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTags = await _db.SpecialTags.FindAsync(id);
            if (specialTags == null)
            {
                return NotFound();
            }

            return View(specialTags);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTags = await _db.SpecialTags.FindAsync(id);
            if (specialTags == null)
            {
                return NotFound();
            }

            return View(specialTags);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSpecialTags(int id)
        {
            SpecialTags specialTags = await _db.SpecialTags.FindAsync(id);
            _db.SpecialTags.Remove(specialTags);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}