using FlowerShopAuth.Data;
using FlowerShopAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShopAuth.Controllers
{
    [Authorize(Roles = "Admin")] // Only Admin can access
    public class AdminGiftController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminGiftController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminGift
        public async Task<IActionResult> Index()
        {
            var gifts = await _context.Gifts.ToListAsync();
            return View(gifts);
        }

        // GET: AdminGift/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminGift/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Gift gift)
        {
            if (ModelState.IsValid)
            {
                _context.Gifts.Add(gift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gift);
        }

        // GET: AdminGift/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var gift = await _context.Gifts.FindAsync(id);
            if (gift == null) return NotFound();

            return View(gift);
        }

        // POST: AdminGift/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Gift gift)
        {
            if (id != gift.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gift);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiftExists(gift.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gift);
        }

        // GET: AdminGift/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var gift = await _context.Gifts.FirstOrDefaultAsync(m => m.Id == id);
            if (gift == null) return NotFound();

            return View(gift);
        }

        // POST: AdminGift/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gift = await _context.Gifts.FindAsync(id);
            if (gift == null) return NotFound();

            // Remove all cart items that reference this gift
            var cartItems = _context.Carts.Where(c => c.GiftId == id);
            _context.Carts.RemoveRange(cartItems);

            // Remove the gift itself
            _context.Gifts.Remove(gift);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Gift and related cart items deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool GiftExists(int id)
        {
            return _context.Gifts.Any(e => e.Id == id);
        }
    }
}
