using GeekyGadgets.Domain.Entity;
using GeekyGadgets.Domain.Response;
using GeekyGadgets.Domain.ViewModels.Account;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekyGadgets.Service.Interfaces
{
    public interface IAccountService
    {
        Task<User> RegisterUserAsync(RegistrationViewModel registrationViewModel);

        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);

        Task<BaseResponse<bool>> ChangePassword(ChangePasswordViewModel model);
        object? GetById(int userId);
    }
}
