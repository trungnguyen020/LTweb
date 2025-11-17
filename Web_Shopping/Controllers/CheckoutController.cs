using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web_Shopping.Models;
using Web_Shopping.Models.ViewModels; // Đảm bảo có namespace chứa CartItemModel
using Web_Shopping.Repository;

namespace Web_Shopping.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<AppUserModel> _userManager; // 1. Khai báo UserManager

        // 2. Inject UserManager vào Constructor
        public CheckoutController(DataContext context, UserManager<AppUserModel> userManager)
        {
            _dataContext = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var ordercode = Guid.NewGuid().ToString();

                // 3. Lấy thông tin User hiện tại từ Database
                var currentUser = await _userManager.GetUserAsync(User);

                // Nếu không tìm thấy user (trường hợp hiếm), bắt đăng nhập lại
                if (currentUser == null) return RedirectToAction("Login", "Account");

                var orderitem = new OrderModels();
                orderitem.OrderCode = ordercode;
                orderitem.UserName = userEmail;
                orderitem.Status = 1;
                orderitem.CreateDate = DateTime.Now;

                // 4. Tự động điền Địa chỉ và SĐT từ Profile
                // Nếu trong profile chưa có thì ghi chú là chưa cập nhật
                orderitem.Address = currentUser.Address ?? "Chưa cập nhật địa chỉ";
                orderitem.PhoneNumber = currentUser.PhoneNumber ?? "Chưa cập nhật số điện thoại";

                _dataContext.Add(orderitem);
                _dataContext.SaveChanges();

                List<CartItemModel> cartItem = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

                foreach (var cart in cartItem)
                {
                    var orderdetails = new OrderDetails();
                    orderdetails.UserName = userEmail;
                    orderdetails.OrderCode = ordercode;
                    orderdetails.ProductId = cart.ProductId;
                    orderdetails.Price = cart.Price;
                    orderdetails.Quantity = cart.Quantity;
                    _dataContext.Add(orderdetails);
                    // Đã chuyển SaveChanges ra ngoài vòng lặp để tối ưu
                }
                _dataContext.SaveChanges(); // 5. Lưu tất cả chi tiết đơn hàng 1 lần

                HttpContext.Session.Remove("Cart");
                TempData["success"] = "Thanh toán thành công, vui lòng đợi duyệt đơn hàng";
                return RedirectToAction("History", "Account");
            }
        }
    }
}