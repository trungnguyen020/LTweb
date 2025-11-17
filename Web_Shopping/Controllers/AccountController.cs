using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Web_Shopping.Models;
using Web_Shopping.Models.ViewModels;
using Web_Shopping.Repository;

namespace Web_Shopping.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly DataContext _dataContext;
        public AccountController(SignInManager<AppUserModel>signInManager, UserManager<AppUserModel>userManager, DataContext context) 
        { 
            _signInManager = signInManager;
            _userManager = userManager;
            _dataContext = context;
        }
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl});
        }
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> History()
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                // User is not logged in, redirect to login
                return RedirectToAction("Login", "Account"); // Replace "Account" with your controller name
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var Orders = await _dataContext.Orders
                .Where(od => od.UserName == userEmail).OrderByDescending(od => od.Id).ToListAsync();
            ViewBag.UserEmail = userEmail;
            return View(Orders);
        }
        public async Task<IActionResult> CancelOrder(string ordercode)
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                // User is not logged in, redirect to login
                return RedirectToAction("Login", "Account");
            }
            try
            {
                var order = await _dataContext.Orders.Where(o => o.OrderCode == ordercode).FirstAsync();
                order.Status = 3;
                _dataContext.Update(order);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                return BadRequest("An error occurred while canceling the order.");
            }


            return RedirectToAction("History", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false,false);
                if (result.Succeeded) 
                {
                    // return RedirectToAction(loginVM.ReturnUrl ?? "/");
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Sai tên đăng nhập hoặc tài khoản");
            }
            return View(loginVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                AppUserModel newUser = new AppUserModel { UserName = user.Username, Email = user.Email };
                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, "Author");
                    TempData["success"] = "Tạo tài khoản thành công.";
                    return Redirect("/account/login");
                }
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

                    if (result.Succeeded)
                    {
                        TempData["success"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập.";
                        return RedirectToAction("Login");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                // Nếu không tìm thấy user
                // Vẫn báo "thành công" để bảo mật (không cho kẻ tấn công biết email nào là đúng)
                TempData["success"] = "Nếu email của bạn tồn tại, mật khẩu đã được reset.";
                return RedirectToAction("Login");
            }

            return View(model);
        }
        [Authorize] // Bắt buộc phải đăng nhập
        public async Task<IActionResult> Details(string ordercode)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); 
            }

            var userEmail = user.Email;

            var order = await _dataContext.Orders
                                .FirstOrDefaultAsync(o => o.OrderCode == ordercode && o.UserName == userEmail);

            if (order == null)
            {
                TempData["error"] = "Bạn không có quyền xem đơn hàng này.";
                return RedirectToAction("History"); 
            }

            var orderDetails = await _dataContext.OrderDetails
                                    .Include(od => od.Product)
                                    .Where(od => od.OrderCode == ordercode)
                                    .ToListAsync();

            ViewBag.OrderStatus = order.Status;

            return View(orderDetails);
        }
        // [HttpGet] Hiển thị trang hồ sơ
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");
            return View(user);
        }

        // [HttpPost] Cập nhật thông tin
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(AppUserModel userUpdate)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Cập nhật các thông tin
            user.PhoneNumber = userUpdate.PhoneNumber;
            user.Address = userUpdate.Address;
            // user.Occupation = userUpdate.Occupation; // Nếu muốn sửa cả nghề nghiệp

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["success"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("Profile", user);
        }
    }
}
