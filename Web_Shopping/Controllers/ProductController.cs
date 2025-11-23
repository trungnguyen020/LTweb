using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Shopping.Models;
using Web_Shopping.Repository;

namespace Web_Shopping.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<AppUserModel> _userManager;
        public ProductController(DataContext context, UserManager<AppUserModel> userManager)
        {
            _dataContext = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int Id)
        {
            if(Id <= 0) RedirectToAction("Index");
            var productById = await _dataContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(c => c.Id == Id);
            if (productById == null)
            {
                return NotFound();
            }
            var relatedProducts = await _dataContext.Products
            .Where(p=>(p.CategoryId == productById.CategoryId || p.BrandId == productById.BrandId) && p.Id != productById.Id)
            .Take(4)
            .ToListAsync();
            ViewBag.RelatedProducts = relatedProducts;
            var reviews = await _dataContext.Ratings
                .Where(r => r.ProductId == Id)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
            ViewBag.Reviews = reviews;
            return View(productById);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(RatingModel rating)
        {
            var user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(rating.Name))
            {
                rating.Name = user.UserName;
            }
            else
            {

            }
            rating.CreatedDate = DateTime.Now;
            ModelState.Remove("Product");
            if (ModelState.IsValid)
            {
                _dataContext.Ratings.Add(rating);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Cảm ơn bạn đã đánh giá!";
                return Redirect(Request.Headers["Referer"]);
            }
            else
            {
                TempData["error"] = "Vui lòng nhập đầy đủ thông tin.";
                return Redirect(Request.Headers["Referer"]);
            }
        }
        public async Task<IActionResult> Search(string searchTerm)
        {
            var products = await _dataContext.Products
                .Where(p=>p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
                .ToListAsync();
            ViewBag.Keyword = searchTerm;

            return View(products);
        }
    }
}
