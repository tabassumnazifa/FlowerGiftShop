using FlowerShopAuth.Data;
using FlowerShopAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShopAuth.Controllers
{
    public class RatingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RatingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Show all reviews
        public async Task<IActionResult> Index()
        {
            var ratings = _context.Ratings.ToList();

            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = false;

            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                isAdmin = roles.Contains("Admin");
            }

            ViewBag.IsAdmin = isAdmin; // send flag to view
            return View(ratings);
        }

        // Show Create page (only for non-admins)
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                    return RedirectToAction(nameof(Index)); // admin cannot create reviews
            }

            return View();
        }

        // Save review (only for non-admins)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rating rating)
        {
            if (!ModelState.IsValid)
                return View(rating);

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                    return RedirectToAction(nameof(Index)); // admin cannot post reviews

                rating.UserId = user.Id;
                rating.UserEmail = user.Email;
            }

            rating.DateAdded = DateTime.Now;
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Delete review (only for admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var review = await _context.Ratings.FindAsync(id);
            if (review == null)
                return NotFound();

            _context.Ratings.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
