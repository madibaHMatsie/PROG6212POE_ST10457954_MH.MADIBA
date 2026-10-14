using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;
using System.Security.Claims;

namespace RaceDayAPI.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class EnrolmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrolmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        // GET: api/enrolments
        [HttpGet("enrolments")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            var userId = GetUserId();

            var enrolments = await _context.Enrolments
                .Where(e => e.ParticipantId == userId)
                .Include(e => e.Events)
                .Include(e => e.Category)
                .Include(e => e.Result)
                .ToListAsync();

            return Ok(enrolments.Select(e => new EnrolmentDto
            {
                EnrolmentId = e.EnrolmentId,
                ParticipantId = e.ParticipantId,
                EventId = e.EventId,
                EventName = e.Events.Name,
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name,
                EnrolmentDate = e.EnrolmentDate,
                Status = e.Status,
                BibNumber = e.BibNumber,
                Results = e.Result == null ? null : new ResultDto
                {
                    ResultId = e.Result.ResultId,
                    EnrolmentId = e.Result.EnrolmentId,
                    FinishTime = e.Result.FinishTime,
                    FinishPosition = e.Result.FinishPosition,
                    CategoryPosition = e.Result.CategoryPosition,
                    Status = e.Result.Status
                }
            }));
        }

        // GET: api/events/{eventId}/enrolments
        [HttpGet("events/{eventId}/enrolments")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetEnrolmentsForEvent(int eventId)
        {
            var userId = GetUserId();
            var ev = await _context.Events.FindAsync(eventId);

            if (ev == null) return NotFound(new { message = "Event not found" });
            if (ev.OrganiserId != userId) return Forbid();

            var enrolments = await _context.Enrolments
                .Where(e => e.EventId == eventId)
                .Include(e => e.Participant)
                .Include(e => e.Category)
                .Include(e => e.Result)
                .ToListAsync();

            return Ok(enrolments.Select(e => new EnrolmentDto
            {
                EnrolmentId = e.EnrolmentId,
                ParticipantId = e.ParticipantId,
                ParticipantName = $"{e.Participant.FirstName} {e.Participant.LastName}",
                EventId = e.EventId,
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name,
                EnrolmentDate = e.EnrolmentDate,
                Status = e.Status,
                BibNumber = e.BibNumber
            }));
        }

        // POST: api/events/{eventId}/enrol
        [HttpPost("events/{eventId}/enrol")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> EnrolInEvent(int eventId, [FromBody] CreateEnrolmentDto model)
        {
            var userId = GetUserId();

            var ev = await _context.Events
                .Include(e => e.Enrolments)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (ev == null) return NotFound(new { message = "Event not found" });

            if (ev.Status != "Open")
                return BadRequest(new { message = "Event is not open for enrolment" });

            var category = await _context.Categories.FindAsync(model.CategoryId);
            if (category == null || category.EventId != eventId)
                return BadRequest(new { message = "Invalid category for this event" });

            if (ev.Enrolments.Count(x => x.Status != "Cancelled") >= ev.MaxParticipants)
                return BadRequest(new { message = "Event is full" });

            if (await _context.Enrolments.AnyAsync(e => e.EventId == eventId && e.ParticipantId == userId && e.Status != "Cancelled"))
                return Conflict(new { message = "You are already enrolled in this event" });

            var bib = new Random().Next(1000, 9999);

            var enrolment = new Enrolment
            {
                ParticipantId = userId,
                EventId = eventId,
                CategoryId = model.CategoryId,
                EnrolmentDate = DateTime.Now,
                Status = "Confirmed",
                BibNumber = bib
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                enrolmentId = enrolment.EnrolmentId,
                bibNumber = bib,
                message = "Enrolled successfully"
            });
        }

        // DELETE: api/enrolments/{id}
        [HttpDelete("enrolments/{id}")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> CancelEnrolment(int id)
        {
            var userId = GetUserId();
            var enrolment = await _context.Enrolments.FindAsync(id);

            if (enrolment == null) return NotFound(new { message = "Enrolment not found" });
            if (enrolment.ParticipantId != userId) return Forbid();

            enrolment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}