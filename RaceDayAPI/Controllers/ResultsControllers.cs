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
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        // GET: api/enrolments/me/results
        [HttpGet("enrolments/me/results")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = GetUserId();

            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Events)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Category)
                .Where(r => r.Enrolment.ParticipantId == userId)
                .ToListAsync();

            return Ok(results.Select(r => new ParticipantResultDto
            {
                ResultId = r.ResultId,
                EnrolmentId = r.EnrolmentId,
                FinishTime = r.FinishTime,
                FinishPosition = r.FinishPosition,
                CategoryPosition = r.CategoryPosition,
                Status = r.Status,
                EventName = r.Enrolment.Events.Name,
                EventLocation = r.Enrolment.Events.Location,
                EventDate = r.Enrolment.Events.Date,
                CategoryName = r.Enrolment.Category.Name
            }));
        }

        // GET: api/events/{eventId}/results
        [HttpGet("events/{eventId}/results")]
        public async Task<IActionResult> GetEventResults(int eventId)
        {
            if (!await _context.Events.AnyAsync(e => e.EventId == eventId))
                return NotFound(new { message = "Event not found" });

            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Participant)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Events)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Category)
                .Where(r => r.Enrolment.EventId == eventId)
                .OrderBy(r => r.FinishPosition)
                .ToListAsync();

            return Ok(results.Select(r => new ResultDto
            {
                ResultId = r.ResultId,
                EnrolmentId = r.EnrolmentId,
                FinishTime = r.FinishTime,
                FinishPosition = r.FinishPosition,
                CategoryPosition = r.CategoryPosition,
                Status = r.Status,
                ParticipantName = $"{r.Enrolment.Participant.FirstName} {r.Enrolment.Participant.LastName}",
                EventName = r.Enrolment.Events.Name,
                CategoryName = r.Enrolment.Category.Name
            }));
        }

        // GET: api/enrolments/{enrolmentId}/result
        [HttpGet("enrolments/{enrolmentId}/result")]
        public async Task<IActionResult> GetResult(int enrolmentId)
        {
            var userId = GetUserId();

            var result = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Events)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Category)
                .FirstOrDefaultAsync(r => r.EnrolmentId == enrolmentId);

            if (result == null) return NotFound(new { message = "Result not found" });

            if (User.IsInRole("Participant") && result.Enrolment.ParticipantId != userId)
                return Forbid();

            return Ok(new ResultDto
            {
                ResultId = result.ResultId,
                EnrolmentId = result.EnrolmentId,
                FinishTime = result.FinishTime,
                FinishPosition = result.FinishPosition,
                CategoryPosition = result.CategoryPosition,
                Status = result.Status,
                EventName = result.Enrolment.Events.Name,
                CategoryName = result.Enrolment.Category.Name
            });
        }

        // POST: api/events/{eventId}/results
        [HttpPost("events/{eventId}/results")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateResult(int eventId, [FromBody] CreateResultDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound(new { message = "Event not found" });
            if (ev.OrganiserId != userId) return Forbid();

            var enrolment = await _context.Enrolments.FindAsync(model.EnrolmentId);
            if (enrolment == null || enrolment.EventId != eventId)
                return NotFound(new { message = "Enrolment not found for this event" });

            if (await _context.Results.AnyAsync(r => r.EnrolmentId == model.EnrolmentId))
                return Conflict(new { message = "Result already exists for this enrolment" });

            var result = new Result
            {
                EnrolmentId = model.EnrolmentId,
                FinishTime = model.FinishTime,
                FinishPosition = model.FinishPosition,
                CategoryPosition = model.CategoryPosition,
                Status = string.IsNullOrEmpty(model.Status) ? "Official" : model.Status
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { resultId = result.ResultId, message = "Result captured" });
        }

        // PUT: api/results/{resultId}
        [HttpPut("results/{resultId}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateResult(int resultId, [FromBody] UpdateResultDto model)
        {
            var userId = GetUserId();

            var result = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Events)
                .FirstOrDefaultAsync(r => r.ResultId == resultId);

            if (result == null) return NotFound(new { message = "Result not found" });
            if (result.Enrolment.Events.OrganiserId != userId) return Forbid();

            if (model.FinishTime != default) result.FinishTime = model.FinishTime;
            if (model.FinishPosition > 0) result.FinishPosition = model.FinishPosition;
            if (model.CategoryPosition != null) result.CategoryPosition = model.CategoryPosition;
            if (!string.IsNullOrEmpty(model.Status)) result.Status = model.Status;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Result updated successfully" });
        }
    }
}