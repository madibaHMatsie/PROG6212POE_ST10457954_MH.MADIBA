using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Services;
using RaceDayMVC.ViewModels;
using System.Diagnostics;
using RaceDayMVC.Models; // Ensure this matches where your ErrorViewModel is

namespace RaceDayMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiService _apiService;

        public HomeController(ILogger<HomeController> logger, ApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        // GET: /Home/Index
        public async Task<IActionResult> Index()
        {
            var events = await _apiService.GetAsync<List<EventViewModel>>("/api/events");

            if (events == null)
            {
                events = new List<EventViewModel>();
            }

            return View(events);
        }

        // GET: /Home/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Fetch the specific event from the API
            var ev = await _apiService.GetAsync<EventViewModel>($"/api/events/{id}");
            if (ev == null) return NotFound();

            return View(ev);
        }

        // POST: /Home/Enrol
        [HttpPost]
        public async Task<IActionResult> Enrol(int eventId, int categoryId)
        {
            
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            
            var success = await _apiService.PostAsync($"/api/events/{eventId}/enrol", new { categoryId = categoryId }, token);

            if (success)
            {
                TempData["SuccessMessage"] = "You have successfully enrolled!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to enrol. You might already be enrolled or the event is full.";
            return RedirectToAction("Details", new { id = eventId });
        }
        public async Task<IActionResult> MyEnrolments()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            // Fetch the list of enrolments using the participant's token
            var enrolments = await _apiService.GetAsync<List<dynamic>>("/api/enrolments", token);

            if (enrolments == null)
                enrolments = new List<dynamic>();

            return View(enrolments);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}