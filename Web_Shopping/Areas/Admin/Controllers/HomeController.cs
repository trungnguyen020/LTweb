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
        public IActionResult Revenue()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetRevenueStatistic(string fromDate, string toDate)
        {
            
            DateTime start, end;
            if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
            {
                
                var today = DateTime.Now;
                var diff = today.DayOfWeek - DayOfWeek.Monday;
                if (diff < 0) diff += 7;
                start = today.AddDays(-diff).Date;
                end = start.AddDays(6).Date;
            }
            else
            {
                try
                {
                    start = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    end = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch
                {
                    start = DateTime.Now.Date;
                    end = DateTime.Now.Date;
                }
            }

            
            var rawData = await _dataContext.Orders
                .Where(o => o.Status == 0 && o.CreateDate.Date >= start && o.CreateDate.Date <= end)
                .Join(_dataContext.OrderDetails,
                      o => o.OrderCode,
                      od => od.OrderCode,
                      (o, od) => new {
                          Date = o.CreateDate.Date,
                          Total = od.Quantity * od.Price
                      })
                .GroupBy(x => x.Date)
                .Select(g => new {
                    Date = g.Key,
                    Total = g.Sum(x => x.Total)
                })
                .ToListAsync();

            var result = new List<object>();

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                var dayData = rawData.FirstOrDefault(x => x.Date == date);

                result.Add(new
                {
                    date = date.ToString("dd/MM") + " (" + GetDayName(date.DayOfWeek) + ")",
                    total = dayData != null ? dayData.Total : 0 // Nếu không có đơn thì là 0
                });
            }

            return Json(result);
        }

        
        private string GetDayName(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday: return "T2";
                case DayOfWeek.Tuesday: return "T3";
                case DayOfWeek.Wednesday: return "T4";
                case DayOfWeek.Thursday: return "T5";
                case DayOfWeek.Friday: return "T6";
                case DayOfWeek.Saturday: return "T7";
                case DayOfWeek.Sunday: return "CN";
                default: return "";
            }
        }
    }
}