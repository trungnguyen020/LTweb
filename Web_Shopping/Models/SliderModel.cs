using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_Shopping.Repository.Validation;

namespace Web_Shopping.Models
{
    public class SliderModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Yêu cầu không bỏ trống tên Slider")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Yêu cầu không bỏ trống mô tả Slider")]
        public string Description { get; set; }
        public int Status { get; set; }
        public string Image { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpLoad { get; set; }
    }
}
