using FlowerShopAuth.Data;
using FlowerShopAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShopAuth.Controllers
{
    [Authorize(Roles = "User")] // Only Users can access this page
    public class UserGiftController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserGiftController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /UserGift
        // Optional searchString parameter for product search
        public async Task<IActionResult> Index(string searchString)
        {
            // Start query for gifts
            IQueryable<Gift> giftsQuery = _context.Gifts.AsQueryable();

            // Apply search filter if search string is provided
            if (!string.IsNullOrEmpty(searchString))
            {
                giftsQuery = giftsQuery.Where(g => g.Name.Contains(searchString));
            }

            // Execute query
            var gifts = await giftsQuery.ToListAsync();

            // Fetch active offers for notifications
            var activeOffers = await _context.Offers
                .Where(o => o.StartDate <= DateTime.Now && o.EndDate >= DateTime.Now)
                .ToListAsync();

            // Pass active offers to the view using ViewBag
            ViewBag.ActiveOffers = activeOffers;

            return View(gifts);
        }
    }
}
