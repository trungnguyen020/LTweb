using System.ComponentModel.DataAnnotations;

namespace Web_Shopping.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required (ErrorMessage ="Yêu cập nhập tên người dùng")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Yêu cập nhập Email"),EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password),Required(ErrorMessage ="Yêu cầu nhập mật khẩu")]
        public string Password { get; set; }
    }
}
