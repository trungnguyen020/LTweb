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
    public class SliderController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SliderController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Sliders.OrderByDescending(p=>p.Id).ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderModel slider)
        {
            if (ModelState.IsValid)
            {
                
                if (slider.ImageUpLoad != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/slider");
                    string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpLoad.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await slider.ImageUpLoad.CopyToAsync(fs);
                    fs.Close();
                    slider.Image = imageName;
                }

                _dataContext.Add(slider);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm slider thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Có một vài thứ đang bị lỗi";
            }
            return View(slider);
        }
        public async Task<IActionResult> Edit(int id)
        {
            SliderModel slider = await _dataContext.Sliders.FindAsync(id);

            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SliderModel slider)
        {
            var slider_existed = _dataContext.Sliders.Find(slider.Id);
            if (ModelState.IsValid)
            {
                if (slider_existed == null)
                {
                    return NotFound(); 
                }
                if (slider.ImageUpLoad != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/slider");
                    if (!string.IsNullOrEmpty(slider_existed.Image))
                    {
                        string oldImagePath = Path.Combine(uploadsDir, slider_existed.Image);

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpLoad.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await slider.ImageUpLoad.CopyToAsync(fs);
                    fs.Close();
                    slider_existed.Image = imageName;
                }
                slider_existed.Name = slider.Name;
                slider_existed.Description = slider.Description;
                slider_existed.Status = slider.Status;

                _dataContext.Update(slider_existed);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Sửa slider thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Có một vài thứ đang bị lỗi";
            }
            return View(slider);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            SliderModel slider = await _dataContext.Sliders.FindAsync(id);
            if (slider == null)
            {
                TempData["error"] = "Không tìm thấy sản phẩm để xóa.";
                return RedirectToAction("Index");
            }
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/slider");
                string oldfileImage = Path.Combine(uploadsDir, slider.Image);
                if (System.IO.File.Exists(oldfileImage))
                {
                    System.IO.File.Delete(oldfileImage);
                }
            _dataContext.Sliders.Remove(slider);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Slider đã xóa";
            return RedirectToAction("Index");
        }
    }
}
