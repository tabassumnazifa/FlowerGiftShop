using FlowerShopAuth.Data;
using FlowerShopAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShopAuth.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cartItems = _context.Carts.Where(c => c.UserId == user.Id).ToList();
            return View(cartItems);
        }

        // POST: Add item to cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int giftId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var existingItem = _context.Carts.FirstOrDefault(c => c.UserId == user.Id && c.GiftId == giftId);
            if (existingItem != null)
            {
                existingItem.Count += 1;
            }
            else
            {
                var gift = _context.Gifts.FirstOrDefault(g => g.Id == giftId);
                if (gift != null)
                {
                    _context.Carts.Add(new Cart
                    {
                        GiftId = gift.Id,
                        Name = gift.Name,
                        Price = gift.Price,
                        Count = 1,
                        UserId = user.Id
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["CartMessage"] = $"{_context.Gifts.FirstOrDefault(g => g.Id == giftId)?.Name} added to cart!";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // POST: Remove item from cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var item = _context.Carts.FirstOrDefault(c => c.Id == cartId && c.UserId == user.Id);
            if (item != null)
            {
                _context.Carts.Remove(item);
                await _context.SaveChangesAsync();
                TempData["CartMessage"] = $"{item.Name} removed from cart!";
            }

            return RedirectToAction("Index");
        }

        // POST: Update quantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartId, int count)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var item = _context.Carts.FirstOrDefault(c => c.Id == cartId && c.UserId == user.Id);
            if (item != null)
            {
                if (count <= 0)
                {
                    _context.Carts.Remove(item);
                    TempData["CartMessage"] = $"{item.Name} removed from cart!";
                }
                else
                {
                    item.Count = count;
                    TempData["CartMessage"] = $"{item.Name} quantity updated!";
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // POST: Place order (clear cart & show confirmation)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cartItems = _context.Carts.Where(c => c.UserId == user.Id).ToList();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index");
            }

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Order Confirmed!";
            return RedirectToAction("Index");
        }

        // POST: Proceed to checkout (calculate total + offers)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProceedToCheckout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cartItems = _context.Carts.Where(c => c.UserId == user.Id).ToList();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index");
            }

            decimal totalAmount = cartItems.Sum(c => c.Price * c.Count);

            // Check for active offers
            var activeOffers = _context.Offers
                .Where(o => o.StartDate <= DateTime.Now && o.EndDate >= DateTime.Now)
                .ToList();

            decimal finalAmount = totalAmount;
            string appliedOfferMessage = "";
            int? appliedOfferId = null;

            var eligibleOffer = activeOffers
                .Where(o => totalAmount >= o.MinimumAmount)
                .OrderByDescending(o => o.DiscountPercentage)
                .FirstOrDefault();

            if (eligibleOffer != null)
            {
                finalAmount = totalAmount - (totalAmount * eligibleOffer.DiscountPercentage / 100);
                appliedOfferMessage = $"Offer applied: {eligibleOffer.Name} - {eligibleOffer.DiscountPercentage}% off";
                appliedOfferId = eligibleOffer.Id;
            }

            ViewBag.TotalAmount = totalAmount;
            ViewBag.FinalAmount = finalAmount;
            ViewBag.AppliedOfferMessage = appliedOfferMessage;
            ViewBag.AppliedOfferId = appliedOfferId;

            return View("Checkout");
        }
    }
}
