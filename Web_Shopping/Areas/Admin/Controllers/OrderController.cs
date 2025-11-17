using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Shopping.Models;
using Web_Shopping.Repository;
using Microsoft.AspNetCore.Identity;

namespace Web_Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Publisher,Saler,Admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<AppUserModel> _userManager;
        public OrderController(DataContext context, UserManager<AppUserModel> userManager)
        {
            _dataContext = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<OrderModels> order = await _dataContext.Orders.AsNoTracking().ToListAsync();

            const int pageSize = 10;

            if (pg < 1)
            {
                pg = 1;
            }
            int recsCount = order.Count();

            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = order.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }
        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            if (string.IsNullOrEmpty(ordercode))
            {
                return RedirectToAction("Index");
            }
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.OrderStatus = order.Status;
            ViewBag.Order = order;
            var userInfo = await _userManager.FindByEmailAsync(order.UserName);
            ViewBag.UserProfile = userInfo;
            var DetailsOrder = await _dataContext.OrderDetails.Include(od=>od.Product).Where(od=>od.OrderCode==ordercode).ToListAsync();
            return View(DetailsOrder);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o=>o.OrderCode==ordercode);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new {success = true, message = "trạng thái của đơn hàng đã sửa thành công"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có một lỗi trong quá trình sửa");
            }
        }
    }
}
