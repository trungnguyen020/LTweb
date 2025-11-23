using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Shopping.Models;
using Web_Shopping.Repository;

namespace Web_Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Publisher,Saler,Admin")]
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<AppUserModel> _userManager;

        public HomeController(DataContext context, UserManager<AppUserModel> userManager)
        {
            _dataContext = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Thống kê số liệu để hiện ra Dashboard
            ViewBag.CountProducts = await _dataContext.Products.CountAsync();
            ViewBag.CountOrders = await _dataContext.Orders.CountAsync();
            ViewBag.CountUsers = await _userManager.Users.CountAsync();
            ViewBag.CountNewOrders = await _dataContext.Orders.Where(o => o.Status == 1).CountAsync();

            return View();
        }
    }
}