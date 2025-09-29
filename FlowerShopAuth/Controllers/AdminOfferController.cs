using FlowerShopAuth.Data;
using FlowerShopAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShopAuth.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOfferController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminOfferController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminOffer (all offers, CRUD)
        public async Task<IActionResult> Index()
        {
            var offers = await _context.Offers
                .OrderByDescending(o => o.StartDate)
                .ToListAsync();
            return View(offers);
        }

        // GET: AdminOffer/ActiveOffers (only available offers)
        public async Task<IActionResult> ActiveOffers()
        {
            var activeOffers = await _context.Offers
                .Where(o => o.StartDate <= DateTime.Now && o.EndDate >= DateTime.Now)
                .OrderBy(o => o.EndDate)
                .ToListAsync();

            return View(activeOffers);
        }

        // GET: AdminOffer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminOffer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Offer offer)
        {
            if (ModelState.IsValid)
            {
                _context.Offers.Add(offer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(offer);
        }

        // GET: AdminOffer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var offer = await _context.Offers.FindAsync(id);
            if (offer == null) return NotFound();

            return View(offer);
        }

        // POST: AdminOffer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Offer offer)
        {
            if (id != offer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(offer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Offers.Any(e => e.Id == offer.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(offer);
        }

        // GET: AdminOffer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var offer = await _context.Offers.FirstOrDefaultAsync(o => o.Id == id);
            if (offer == null) return NotFound();

            return View(offer);
        }

        // POST: AdminOffer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer != null)
            {
                _context.Offers.Remove(offer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
