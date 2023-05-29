using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using GeekyGadgets.DAL.Interfaces;
using GeekyGadgets.DAL.Repositories;
using GeekyGadgets.Domain.Entity;
using GeekyGadgets.Domain.Enum;
using GeekyGadgets.Domain.Helpers;
using GeekyGadgets.Domain.Response;
using GeekyGadgets.Domain.ViewModels.Account;
using GeekyGadgets.Service.Interfaces;
using GeekyGadgets.Service.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace GeekyGadgets.Service.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IBaseRepository<Profile> _proFileRepository;
        private readonly ILogger<AccountService> _logger;
        private readonly IBaseRepository<Basket> _basketRepository;
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public AccountService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings, IBaseRepository<Profile> proFileRepository, IBaseRepository<Basket> basketRepository = null)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
            _proFileRepository = proFileRepository;
            _basketRepository = basketRepository;
        }


        public async Task<User> RegisterUserAsync(RegistrationViewModel registrationViewModel)
        {
            var existingUser = await _userRepository.GetAll().FirstOrDefaultAsync(u => u.Email == registrationViewModel.Email);
            if (existingUser != null)
            {
                throw new ApplicationException("User with the same email already exists");
            }
            if (registrationViewModel.Password != registrationViewModel.PasswordConfirm)
            {
                throw new ApplicationException("Password and Confirm password do not match");
            }

            if (!IsValidPassword(registrationViewModel.Password))
            {
                throw new ApplicationException("Invalid password format");
            }

            var query = "INSERT INTO Users (Name, Email, Password, Role) VALUES (@Name, @Email, @Password, @Role)";

            var parameters = new List<SqlParameter>()
    {
        new SqlParameter("@Name", registrationViewModel.Name),
        new SqlParameter("@Email", registrationViewModel.Email),
        new SqlParameter("@Password", BCrypt.Net.BCrypt.HashPassword(registrationViewModel.Password)),
        new SqlParameter("@Role", Role.User)
    };

            await _userRepository.ExecuteQueryAsync<User>(query, parameters.ToArray());

            var newUser = await _userRepository.GetAll().FirstOrDefaultAsync(u => u.Email == registrationViewModel.Email);

            var profile = new Profile()
            {
                UserId = newUser.Id,
            };
            var basket = new Basket()
            {
                UserId = newUser.Id
            };

            await _proFileRepository.Create(profile);
            await _basketRepository.Create(basket);

            return newUser;
        }

         private bool IsValidPassword(string password)
        {
            return password.Length >= 8 && Regex.IsMatch(password, @"^[a-zA-Z0-9]+$");
        }


        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            // Проверка формата email
            if (!IsValidEmail(model.Email))
            {
                return null;
            }

            var query = "SELECT Password FROM Users WHERE Email = @Email";

            var parameters = new List<SqlParameter>()
    {
        new SqlParameter("@Email", model.Email)
    };

            var hashedPassword = await _userRepository.ExecuteQueryFirstOrDefaultAsync<string>(query, parameters.ToArray());

            if (hashedPassword == null || !BCrypt.Net.BCrypt.Verify(model.Password, hashedPassword))
                return null;

            var userQuery = "SELECT * FROM Users WHERE Email = @Email";

            var userParameters = new List<SqlParameter>()
    {
        new SqlParameter("@Email", model.Email)
    };

            var user = await _userRepository.ExecuteQueryFirstOrDefaultAsync<User>(userQuery, userParameters.ToArray());

            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }



        public async Task<User> GetById(int userId)
        {
            var users = await _userRepository.GetAll().ToListAsync();
            var user = users.FirstOrDefault(u => u.Id == userId);
            return user;
        }
        private string generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("12jkyp78rol86p01");

            var claims = new List<Claim>
    {
        new Claim("id", user.Id.ToString()),
        new Claim("name", user.Name),
        new Claim("email", user.Email),
        new Claim("role", user.Role.ToString()),
        new Claim("password", user.Password)
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public async Task<BaseResponse<bool>> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Name == model.UserName);
                if (user == null)
                {
                    return new BaseResponse<bool>()
                    {
                        StatusCode = StatusCode.UserNotFound,
                        Description = "Пользователь не найден"
                    };
                }

                user.Password = HashPasswordHelper.HashPassowrd(model.NewPassword);
                await _userRepository.Update(user);

                return new BaseResponse<bool>()
                {
                    Data = true,
                    StatusCode = StatusCode.OK,
                    Description = "Пароль обновлен"
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[ChangePassword]: {ex.Message}");
                return new BaseResponse<bool>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        object? IAccountService.GetById(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
