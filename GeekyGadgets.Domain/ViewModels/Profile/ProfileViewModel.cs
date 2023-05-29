using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekyGadgets.Domain.ViewModels.Profile
{
    public class ProfileViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите возраст")]
        [Range(0, 150, ErrorMessage = "Диапазон возраста должен быть от 0 до 150")]
        public short Age { get; set; }

        [Required(ErrorMessage = "Укажите адрес")]
        [MinLength(5, ErrorMessage = "Минимальная длина должна быть больше 5 символов")]
        [MaxLength(250, ErrorMessage = "Максимальная длина должна быть меньше 250 символов")]
        public string Address { get; set; }

        public string UserName { get; set; }

      //  public string NewPassword { get; set; }
    }
}
