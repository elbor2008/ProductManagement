using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ProductManagement.Data;
using ProductManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace ProductManagement.ViewComponents
{
    public class UserNameViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public UserNameViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity) this.User.Identity;
            Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string id = claim.Value;

            ApplicationUser applicationUser = await _db.ApplicationUsers.FindAsync(id);
            return View(applicationUser);
        }
    }
}