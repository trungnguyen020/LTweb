using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web_Shopping.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Shopping.Repository
{
    public class SeedData
    {
        public static async Task SeedingData(DataContext _context, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context.Database.Migrate();
            if (!_context.Products.Any())
            {
                CategoryModel macbook = new CategoryModel { Name = "Macbook", Slug = "macbook", Description = "macbook is Large Brand in the world", Status = 1 };
                CategoryModel pc = new CategoryModel { Name = "Pc", Slug = "pc", Description = "pc is Large Brand in the world", Status = 1 };
                BrandModel apple = new BrandModel { Name = "Apple", Slug = "apple", Description = "apple is Large Brand in the world", Status = 1 };
                BrandModel samsung = new BrandModel { Name = "Samsung", Slug = "samsung", Description = "samsung is Large Brand in the world", Status = 1 };
                _context.Products.AddRange(
                    new ProductModel { Name = "Macbook", Slug = "macbook", Description = "macbook is best", Image = "1.jpg", Category = macbook, Brand = apple, Price = 1233 },
                    new ProductModel { Name = "Pc", Slug = "pc", Description = "pc is best", Image = "2.jpg", Category = pc, Brand = samsung, Price = 1233 }
                );
                await _context.SaveChangesAsync();
            }
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("Author"));
                await roleManager.CreateAsync(new IdentityRole("Publisher"));
            }
            if (!userManager.Users.Any(u => u.UserName == "admin"))
            {
                // 1. Tạo user mới
                var adminUser = new AppUserModel
                {
                    UserName = "admin123", // Tên đăng nhập
                    Email = "admin@gmail.com",
                    EmailConfirmed = true
                    // (Thêm các thuộc tính khác nếu cần, ví dụ: Occupation, v.v.)
                };

                // 2. Tạo user với mật khẩu
                // (Mật khẩu "admin123" sẽ được HASH)
                var result = await userManager.CreateAsync(adminUser, "admin123");

                // 3. Gán Role "Admin" cho user đó
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
