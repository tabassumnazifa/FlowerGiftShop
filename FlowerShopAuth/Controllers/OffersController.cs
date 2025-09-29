using FlowerShopAuth.Data;
using FlowerShopAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlowerShopAuth.Controllers
{
    [Authorize(Roles = "User")]
    public class UserOfferController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserOfferController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1️⃣ View all active offers
        public async Task<IActionResult> UserOffers()
        {
            var offers = await _context.Offers
                .Where(o => o.StartDate <= DateTime.Now && o.EndDate >= DateTime.Now)
                .OrderBy(o => o.StartDate)
                .ToListAsync();

            return View(offers);
        }

        // 2️⃣ Checkout page
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            decimal totalAmount = await _context.Carts
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Price);

            DateTime today = DateTime.Now;

            var activeOffer = await _context.Offers
                .Where(o => o.StartDate <= today && o.EndDate >= today && totalAmount >= o.MinimumAmount)
                .OrderByDescending(o => o.DiscountPercentage)
                .FirstOrDefaultAsync();

            decimal finalAmount = totalAmount;
            int? appliedOfferId = null;
            string appliedOfferMessage = "No Offers Available";

            if (activeOffer != null)
            {
                finalAmount = totalAmount - (totalAmount * activeOffer.DiscountPercentage / 100);
                appliedOfferId = activeOffer.Id;
                appliedOfferMessage = $"Offer Applied: {activeOffer.Name} ({activeOffer.DiscountPercentage}% Off)";
            }

            ViewBag.TotalAmount = totalAmount;
            ViewBag.FinalAmount = finalAmount;
            ViewBag.AppliedOfferMessage = appliedOfferMessage;
            ViewBag.AppliedOfferId = appliedOfferId;

            return View();
        }

        // 3️⃣ Place order
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(decimal finalAmount, int? appliedOfferId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = new Order
            {
                UserId = userId,
                TotalAmount = await _context.Carts
                    .Where(c => c.UserId == userId)
                    .SumAsync(c => c.Price),
                FinalAmount = finalAmount,
                AppliedOfferId = appliedOfferId,
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Clear cart
            var cartItems = _context.Carts.Where(c => c.UserId == userId);
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Order placed successfully!";
            return RedirectToAction("UserOrders", "Order"); // optional: redirect to user's order history
        }
    }
}
