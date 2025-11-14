using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_Shopping.Models;
using Web_Shopping.Repository;

namespace Web_Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Publisher,Saler,Admin")]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<CategoryModel> categories = await _dataContext.Categories.AsNoTracking().ToListAsync();

            const int pageSize = 10;

            if (pg < 1)
            {
                pg = 1;
            }
            int recsCount = categories.Count();

            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = categories.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            

            if (ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-");
                var Slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (Slug != null)
                {
                    ModelState.AddModelError("", "Danh mục đã có");
                    return View(category);
                }
                _dataContext.Add(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm danh mục thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Có một vài thứ đang bị lỗi";
            }
            return View(category);
        }
        public async Task<IActionResult> Edit(int id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(id);
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryModel category)
        {


            if (ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-");
                var Slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug && p.Id != category.Id);
                if (Slug != null)
                {
                    ModelState.AddModelError("", "Danh mục đã có");
                    return View(category);
                }
                _dataContext.Update(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Sửa danh mục thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Có một vài thứ đang bị lỗi";
            }
            return View(category);
        }
        public async Task<IActionResult> Delete(int id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(id);

            if (category == null)
            {
                TempData["error"] = "Không tìm thấy danh mục để xóa.";
                return RedirectToAction("Index"); 
            }
            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Danh mục đã xóa";
            return RedirectToAction("Index");
        }
    }
}
