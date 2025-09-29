using FlowerShopAuth.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FlowerShopAuth.Controllers
{
    [Authorize(Roles = "User")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var gifts = await _db.Gifts.AsNoTracking().ToListAsync();
            return View(gifts); // will use Views/Product/Index.cshtml
        }
    }
}
