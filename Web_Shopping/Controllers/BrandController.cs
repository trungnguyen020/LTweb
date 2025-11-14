using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Shopping.Models;
using Web_Shopping.Repository;

namespace Web_Shopping.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string Slug = " ")
        {
            BrandModel brand = _dataContext.Brands.Where(c => c.Slug == Slug).FirstOrDefault();

            if (brand == null) return RedirectToAction("Index");

            var productByBrand = _dataContext.Products.Where(c => c.BrandId == brand.Id);

            return View(await productByBrand.OrderByDescending(c => c.Id).ToListAsync());
        }
    }
}
