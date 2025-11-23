using Microsoft.AspNetCore.Mvc;
using Web_Shopping.Models;
using Web_Shopping.Models.ViewModels;
using Web_Shopping.Repository;

namespace Web_Shopping.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext _context)
        {
            _dataContext = _context;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItem = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemVIewModel cartVM = new()
            {
                CartItem = cartItem,
                GrandTotal = cartItem.Sum(x => x.Quantity * x.Price)
            };
            return View(cartVM);
        }
        public async Task<IActionResult> Add(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItems = cart.Where(c => c.ProductId == Id).FirstOrDefault();

            if (cartItems == null)
            {
                cart.Add(new CartItemModel(product));
            }
            else
            {
                cartItems.Quantity += 1;
            }
            HttpContext.Session.SetJson("Cart", cart);

            TempData["success"] = "Thêm sản phẩm vào giỏ hàng thành công";
            return Redirect(Request.Headers["Referer"].ToString());
        }
        private decimal GetGrandTotal(List<CartItemModel> cart)
        {
            return cart.Sum(x => x.Quantity * x.Price);
        }
        [HttpPost]
        public IActionResult Increase(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

            if (cartItem == null) return NotFound();

            // Tăng số lượng
            ++cartItem.Quantity;

            // Lưu lại Session
            HttpContext.Session.SetJson("Cart", cart);

            // Trả về JSON để JavaScript cập nhật giao diện
            return Json(new
            {
                success = true,
                qty = cartItem.Quantity,
                subTotal = (cartItem.Price * cartItem.Quantity).ToString("#,##0 VNĐ"),
                grandTotal = GetGrandTotal(cart).ToString("#,##0 VNĐ")
            });
        }
        [HttpPost]
        public IActionResult Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

            if (cartItem == null) return NotFound();

            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(c => c.ProductId == Id);
            }

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
                // Nếu giỏ hàng trống, trả về signal để reload trang
                return Json(new { success = true, isEmpty = true });
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            // Nếu sản phẩm bị xóa (do giảm về 0), cartItem sẽ không còn trong list cart
            // Ta kiểm tra xem item còn tồn tại không để trả về dữ liệu đúng
            var itemStillExists = cart.Any(c => c.ProductId == Id);

            if (!itemStillExists)
            {
                // Báo cho JS biết item này đã bị xóa để ẩn dòng đó đi
                return Json(new { success = true, qty = 0, grandTotal = GetGrandTotal(cart).ToString("#,##0 VNĐ") });
            }

            return Json(new
            {
                success = true,
                qty = cartItem.Quantity,
                subTotal = (cartItem.Price * cartItem.Quantity).ToString("#,##0 VNĐ"),
                grandTotal = GetGrandTotal(cart).ToString("#,##0 VNĐ")
            });
        }
        public IActionResult Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");

            cart.RemoveAll(p => p.ProductId == Id);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "Loại bỏ sản phẩm ra khỏi giỏ hàng thành công";
            return RedirectToAction("Index");
        }
        public IActionResult Clear(int Id)
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Đã xóa giỏ hàng";
            return RedirectToAction("Index");
        }
    }
}
