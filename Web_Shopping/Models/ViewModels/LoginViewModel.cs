using System.ComponentModel.DataAnnotations;

namespace Web_Shopping.Models.ViewModels
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cập nhập tên người dùng")]
        public string Username { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Yêu cầu nhập mật khẩu")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
