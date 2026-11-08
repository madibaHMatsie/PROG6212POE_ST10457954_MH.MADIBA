using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Services;
using RaceDayMVC.ViewModels;
using Newtonsoft.Json;

namespace RaceDayMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Call the API
            var response = await _apiService.PostAsync<dynamic>("/api/auth/login", new
            {
                email = model.Email,
                password = model.Password
            });

            if (response != null)
            {
                // Save the Token and Role in Session
                HttpContext.Session.SetString("JwtToken", (string)response.token);
                HttpContext.Session.SetString("UserRole", (string)response.role);
                HttpContext.Session.SetString("UserName", (string)response.firstName);

                // Redirect based on role
                if ((string)response.role == "Organiser")
                    return RedirectToAction("Index", "Dashboard"); // We will build this later
                else
                    return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _apiService.PostAsync("/api/auth/register", new
            {
                email = model.Email,
                password = model.Password,
                firstName = model.FirstName,
                lastName = model.LastName,
                role = model.Role
            });

            if (success)
            {
                // Auto-login after registration
                return await Login(new LoginViewModel { Email = model.Email, Password = model.Password });
            }

            ModelState.AddModelError("", "Registration failed. Email might already exist.");
            return View(model);
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}