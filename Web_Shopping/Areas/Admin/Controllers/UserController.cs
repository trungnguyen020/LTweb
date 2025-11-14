using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_Shopping.Models;
using Web_Shopping.Repository;

namespace Web_Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;

        public UserController(DataContext context, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usersWithRoles = await (from user in _userManager.Users
                                             // LEFT JOIN tới bảng UserRoles
                                         join userRole in _dataContext.UserRoles on user.Id equals userRole.UserId into ur
                                         from userRole in ur.DefaultIfEmpty() // Cú pháp 'DefaultIfEmpty' chính là LEFT JOIN

                                             // LEFT JOIN tới bảng Roles
                                         join role in _dataContext.Roles on userRole.RoleId equals role.Id into r
                                         from role in r.DefaultIfEmpty()

                                         select new
                                         {
                                             User = user, // Lấy toàn bộ đối tượng user
                                             RoleName = (role == null ? "Chưa gán" : role.Name) // Nếu 'role' là null, hiển thị "Chưa gán"
                                         })
                                .ToListAsync();

            return View(usersWithRoles);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var role = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(role, "Id", "Name");
            return View(new AppUserModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            if (ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash);
                if (createUserResult.Succeeded)
                {
                    var CreateUser = await _userManager.FindByEmailAsync(user.Email);
                    var roles = _roleManager.FindByIdAsync(user.RoleId);
                    //gán quyền
                    var addToRoleResult = await _userManager.AddToRoleAsync(CreateUser, roles.Result.Name);
                    if (!addToRoleResult.Succeeded)
                    {
                        foreach(var error in createUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var error in createUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(user);
                }
            }
            else 
            {
                TempData["error"] = "Có một vài thứ đang bị lỗi";
            }
            var role = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(role, "Id", "Name");
            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            var role = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(role, "Id", "Name");
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, AppUserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(Id);//lấy User dựa vào id
            if(existingUser == null)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;

                var updateUserResult = await _userManager.UpdateAsync(existingUser);
                if(updateUserResult.Succeeded)
                {
                    var newRole = await _roleManager.FindByIdAsync(user.RoleId);
                    if (newRole != null)
                    {
                        var oldRoles = await _userManager.GetRolesAsync(existingUser);

                        if (oldRoles.Any())
                        {
                            await _userManager.RemoveFromRolesAsync(existingUser, oldRoles);
                        }

                        await _userManager.AddToRoleAsync(existingUser, newRole.Name);
                    }
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var error in updateUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(existingUser);
                }
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(existingUser);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var loggedInUserId = _userManager.GetUserId(User);
            if (Id == loggedInUserId)
            {
                TempData["error"] = "Bạn không thể xóa tài khoản đang đăng nhập.";
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByIdAsync(Id);
            if(user == null)
            {
                TempData["error"] = "Không tìm thấy tài khoản.";
                return RedirectToAction("Index");
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                TempData["error"] = "Lỗi khi xóa tài khoản: " + string.Join(", ", deleteResult.Errors.Select(e => e.Description));
                return RedirectToAction("Index");
            }
            TempData["success"] = "User đã xóa thành công";
            return RedirectToAction("Index");
        }
    }
}
