using GeekyGadgets.Domain.Entity;
using GeekyGadgets.Domain.ViewModels.Account;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekyGadgets.Service.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(RegistrationViewModel registrationViewModel);
        Task<string> AuthenticateAsync(LoginViewModel loginViewModel);
    }
}
