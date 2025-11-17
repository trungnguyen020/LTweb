using Microsoft.AspNetCore.Identity;

namespace Web_Shopping.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Occupation {  get; set; }
        public string RoleId { get; set; }
        public string Address { get; set; }
    }
}
