using Automarket.Service.Implementations;
using Automarket.Service.JWT;
using GeekyGadgets.Domain.Entity;
using GeekyGadgets.Domain.ViewModels.Account;
using GeekyGadgets.Service.Implementations;
using GeekyGadgets.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace GeekyGadgets.Controllers
{
    public class UserController : Controller
    {

        private readonly UserService _userService;
        private readonly HttpService _httpService;
        private readonly JwtService _jwtService; 
        public UserController(UserService userService, HttpService httpService, JwtService jwtService)    
        {
            _userService = userService;
            _httpService = httpService;
            _jwtService = jwtService;
        }
        [Authorize]
        public IActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var email = identity.FindFirst(ClaimTypes.Email)?.Value;

            // используйте email или другие данные для выполнения запросов к базе данных или другим службам

            return View();
        }
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegistrationViewModel model)
        {
            try
            {
                var user = await _userService.RegisterUserAsync(model);
                return RedirectToAction("Index", "Home");
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var token = await _userService.AuthenticateAsync(model);

            if (token == null)
            {
                ViewBag.ErrorMessage = "Неправильный логин или пароль. Попробуйте еще раз или зарегистрируйтесь.";
                return View(model);
            }

            HttpContext.Response.Cookies.Append("access_token", token);

            return RedirectToAction("Index", "Home");
        }



        //[Authorize]
        
        [HttpGet("secret")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> Secret()
        {
            // аутентифицировать пользователя
           
            // получить токен из куки и добавить его в заголовок запроса
            if (!Request.Cookies.TryGetValue("access_token", out string token))
            {
                return RedirectToAction("Login");
            }

            if (!string.IsNullOrEmpty(token))
            {
                HttpContext.Request.Headers.Add("Authorization", $"Bearer {token}");
            }

            // Проверяем, что заголовок Authorization был успешно добавлен
            if (!HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                return RedirectToAction("Login");
            }

            var result = await HttpContext.AuthenticateAsync();
            if (!result.Succeeded)
            {
                Console.WriteLine($"Ошибка аутентификации: {result.Failure}");
                // пользователь не авторизован, перенаправить на страницу входа
                return RedirectToAction("Login");
            }


            // пользователь авторизован, получить идентификатор пользователя
            var userId = result.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Return the view
            return View();

        }


    }
}
