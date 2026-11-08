using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;
using RaceDayAPI.Services;
using System.Security.Claims;


namespace RaceDayAPI.Controllers
{
    [Route("api/events")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobService _blobService; // <-- Added

        public EventsController(ApplicationDbContext context, BlobService blobService) // <-- Updated
        {
            _context = context;
            _blobService = blobService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        private bool IsOrganiser() => User.IsInRole("Organiser");

        // GET: api/events
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllEvents(
            [FromQuery] string? status,
            [FromQuery] string? eventType)
        {
            var query = _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .Include(e => e.Enrolments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(e => e.Status == status);

            if (!string.IsNullOrEmpty(eventType))
                query = query.Where(e => e.EventType == eventType);

            var events = await query.OrderByDescending(e => e.Date).ToListAsync();

            var result = events.Select(e => new EventDto
            {
                EventId = e.EventId,
                OrganiserId = e.OrganiserId,
                OrganiserName = $"{e.Organiser.FirstName} {e.Organiser.LastName}",
                Name = e.Name,
                Description = e.Description,
                Date = e.Date,
                Location = e.Location,
                Distance = e.Distance,
                EventType = e.EventType,
                MaxParticipants = e.MaxParticipants,
                CurrentEnrolments = e.Enrolments.Count(x => x.Status != "Cancelled"),
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                BannerImageUrl = e.BannerImageUrl, // <-- Added so MVC can see it
                Categories = e.Categories.Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    EventId = c.EventId,
                    Name = c.Name,
                    Description = c.Description,
                    EntryFee = c.EntryFee,
                    AgeGroup = c.AgeGroup,
                    Distance = c.Distance
                }).ToList()
            });

            return Ok(result);
        }

        // GET: api/events/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvent(int id)
        {
            var e = await _context.Events
                .Include(x => x.Organiser)
                .Include(x => x.Categories)
                .Include(x => x.Enrolments)
                .FirstOrDefaultAsync(x => x.EventId == id);

            if (e == null) return NotFound(new { message = "Event not found" });

            return Ok(new EventDto
            {
                EventId = e.EventId,
                OrganiserId = e.OrganiserId,
                OrganiserName = $"{e.Organiser.FirstName} {e.Organiser.LastName}",
                Name = e.Name,
                Description = e.Description,
                Date = e.Date,
                Location = e.Location,
                Distance = e.Distance,
                EventType = e.EventType,
                MaxParticipants = e.MaxParticipants,
                CurrentEnrolments = e.Enrolments.Count(x => x.Status != "Cancelled"),
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                BannerImageUrl = e.BannerImageUrl, // <-- Added so MVC can see it
                Categories = e.Categories.Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    EventId = c.EventId,
                    Name = c.Name,
                    Description = c.Description,
                    EntryFee = c.EntryFee,
                    AgeGroup = c.AgeGroup,
                    Distance = c.Distance
                }).ToList()
            });
        }

        // POST: api/events
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (model.EventType != "run" && model.EventType != "walk" && model.EventType != "cycle")
                return BadRequest(new { message = "EventType must be 'run', 'walk', or 'cycle'" });

            var userId = GetUserId();

            var newEvent = new Events
            {
                OrganiserId = userId,
                Name = model.Name,
                Description = model.Description,
                Date = model.Date,
                Location = model.Location,
                Distance = model.Distance,
                EventType = model.EventType,
                MaxParticipants = model.MaxParticipants,
                Status = "Open",
                CreatedAt = DateTime.Now
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = "Create Event",
                Details = $"Created event '{newEvent.Name}'"
            });
            await _context.SaveChangesAsync();

            return StatusCode(201, new { eventId = newEvent.EventId, message = "Event created successfully" });
        }

        // PUT: api/events/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventDto model)
        {
            var userId = GetUserId();
            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null) return NotFound(new { message = "Event not found" });
            if (existingEvent.OrganiserId != userId)
                return Forbid();

            if (!string.IsNullOrEmpty(model.Name)) existingEvent.Name = model.Name;
            if (model.Description != null) existingEvent.Description = model.Description;
            if (model.Date != default) existingEvent.Date = model.Date;
            if (!string.IsNullOrEmpty(model.Location)) existingEvent.Location = model.Location;
            if (model.Distance > 0) existingEvent.Distance = model.Distance;
            if (!string.IsNullOrEmpty(model.EventType)) existingEvent.EventType = model.EventType;
            if (model.MaxParticipants > 0) existingEvent.MaxParticipants = model.MaxParticipants;
            if (!string.IsNullOrEmpty(model.Status)) existingEvent.Status = model.Status;

            existingEvent.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Event updated successfully" });
        }

        // DELETE: api/events/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = GetUserId();
            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null) return NotFound(new { message = "Event not found" });
            if (existingEvent.OrganiserId != userId) return Forbid();

            _context.Events.Remove(existingEvent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/events/{id}/upload-banner
        [HttpPost("{id}/upload-banner")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UploadBanner(int id, IFormFile file)
        {
            var userId = GetUserId();
            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null) return NotFound(new { message = "Event not found" });
            if (existingEvent.OrganiserId != userId) return Forbid();

            try
            {
                string imageUrl = await _blobService.UploadFileAsync(file);
                existingEvent.BannerImageUrl = imageUrl;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Image uploaded successfully", url = imageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Upload failed", error = ex.Message });
            }
        }
    }
}