using GeekyGadgets.Domain.Entity;
using GeekyGadgets.Domain.ViewModels.Account;
using GeekyGadgets.Service.Implementations;
using GeekyGadgets.Service.Interfaces;
using GeekyGadgets.Service.JWT;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace GeekyGadgets.Controllers
{
    public class AccountController : Controller
    {

        private readonly AccountService _userService;
        private readonly HttpService _httpService;
        private readonly JwtService _jwtService; 
        public AccountController(AccountService userService, HttpService httpService, JwtService jwtService)    
        {
            _userService = userService;
            _httpService = httpService;
            _jwtService = jwtService;
        }

        [ResponseCache(CacheProfileName = "NoCache")]
        [Service.JWT.Authorize]
        public IActionResult Index()
        {
            

            // используйте email или другие данные для выполнения запросов к базе данных или другим службам

            return View();
        }
        //[ResponseCache(CacheProfileName = "NoCache")]
        [HttpGet("register")]
        [ValidateAntiForgeryToken]
        public IActionResult Register()
        {
            return View();
        }
        // [ResponseCache(CacheProfileName = "NoCache")]
        [HttpPost("register")]
        [ValidateAntiForgeryToken]
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
        // [ResponseCache(CacheProfileName = "NoCache")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("authenticate")]
        public IActionResult Authenticate()
        {
            return View();
        }
        // [ResponseCache(CacheProfileName = "NoCache")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost("authenticate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authenticate(AuthenticateRequest model)
        {
            var response = await _userService.Authenticate(model);

            if (response == null)
            {
                ViewBag.ErrorMessage = "Неправильный логин или пароль. Попробуйте еще раз или зарегистрируйтесь.";
                return View(model);
            }

           
            Response.Headers.Add("Authorization", "Bearer " + response.Token);

            if (!Request.Cookies.TryGetValue("token", out var Token))
            {
                HttpContext.Response.Cookies.Append("token", response.Token, new CookieOptions
                {
                    SameSite = SameSiteMode.None,
                    Secure = true
                });
            }

            return Ok(response);
        }

        [HttpPost]       
        public async Task<IActionResult> Logout()
        {
            // Очистка аутентификационных куки
            await HttpContext.SignOutAsync("token");

            // Очистка JWT-токена из заголовка Authorization
            Response.Headers.Remove("Authorization");

            // Удаление куки с токеном
            if (Request.Cookies.TryGetValue("token", out var token))
            {
                Response.Cookies.Delete("token");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
