using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApiService _apiService;

        public DashboardController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Dashboard
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token) || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var events = await _apiService.GetAsync<List<dynamic>>("/api/events");

            if (events == null) events = new List<dynamic>();

            return View(events);
        }

        // GET: /Dashboard/ViewEnrolments/5
        public async Task<IActionResult> ViewEnrolments(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var enrolments = await _apiService.GetAsync<List<dynamic>>($"/api/events/{id}/enrolments", token);

            if (enrolments == null) enrolments = new List<dynamic>();

            ViewBag.EventId = id;
            return View(enrolments);
        }

        // GET: /Dashboard/CaptureResult/5
        public IActionResult CaptureResult(int enrolmentId, string participantName, int eventId)
        {
            ViewBag.EnrolmentId = enrolmentId;
            ViewBag.ParticipantName = participantName;
            ViewBag.EventId = eventId;
            return View();
        }

        // POST: /Dashboard/CaptureResult
        [HttpPost]
        public async Task<IActionResult> CaptureResult(int enrolmentId, string finishTime, int finishPosition, int categoryPosition, int eventId)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var timeSpan = TimeSpan.Parse(finishTime);

            var success = await _apiService.PostAsync($"/api/events/{eventId}/results", new
            {
                enrolmentId = enrolmentId,
                finishTime = timeSpan,
                finishPosition = finishPosition,
                categoryPosition = categoryPosition,
                status = "Official"
            }, token);

            if (success)
            {
                TempData["SuccessMessage"] = "Result captured successfully!";
                return RedirectToAction("ViewEnrolments", new { id = eventId });
            }

            TempData["ErrorMessage"] = "Failed to capture result.";
            return RedirectToAction("ViewEnrolments", new { id = eventId });
        }

        // POST: /Dashboard/UploadBanner
        [HttpPost]
        public async Task<IActionResult> UploadBanner(int eventId, IFormFile file)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (token == null) return RedirectToAction("Login", "Account");

            if (file != null && file.Length > 0)
            {
                using var content = new MultipartFormDataContent();
                using var stream = file.OpenReadStream();
                content.Add(new StreamContent(stream), "file", file.FileName);

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Make sure the port matches your API's port (7076)
                var response = await client.PostAsync($"https://localhost:7076/api/events/{eventId}/upload-banner", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Banner image uploaded successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to upload image.";
                }
            }

            return RedirectToAction("ViewEnrolments", new { id = eventId });
        }
    }
}