using System.ComponentModel.DataAnnotations;

namespace GeekyGadgets.Domain.ViewModels.Account
{
    public class LoginViewModel
    {
   
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        
    }
}