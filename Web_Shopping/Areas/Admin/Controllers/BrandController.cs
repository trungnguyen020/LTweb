using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Shopping.Models;
using Web_Shopping.Repository;

namespace Web_Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Publisher,Saler,Admin")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<BrandModel> brand = await _dataContext.Brands.AsNoTracking().ToListAsync();

            const int pageSize = 10;

            if (pg < 1)
            {
                pg = 1;
            }
            int recsCount = brand.Count();

            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = brand.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {


            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                var Slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
                if (Slug != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã có");
                    return View(brand);
                }
                _dataContext.Add(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm thương hiệu thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Có một vài thứ đang bị lỗi";
            }
            return View(brand);
        }
        public async Task<IActionResult> Edit(int id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(id);
            return View(brand);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandModel brand)
        {


            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                var Slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == brand.Slug && p.Id != brand.Id);
                if (Slug != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã có");
                    return View(brand);
                }
                _dataContext.Update(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Sửa thương hiệu thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Có một vài thứ đang bị lỗi";
            }
            return View(brand);
        }

        public async Task<IActionResult> Delete(int id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(id);

            if (brand == null)
            {
                TempData["error"] = "Không tìm thấy thương hiệu để xóa.";
                return RedirectToAction("Index");
            }
            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Thương hiệu đã xóa";
            return RedirectToAction("Index");
        }
    }
}
