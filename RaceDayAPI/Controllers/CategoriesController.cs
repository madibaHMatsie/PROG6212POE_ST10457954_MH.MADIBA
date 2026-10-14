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
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        // GET: api/events/{eventId}/categories
        [HttpGet("events/{eventId}/categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoriesForEvent(int eventId)
        {
            if (!await _context.Events.AnyAsync(e => e.EventId == eventId))
                return NotFound(new { message = "Event not found" });

            var categories = await _context.Categories
                .Where(c => c.EventId == eventId)
                .Include(c => c.Enrolments)
                .ToListAsync();

            return Ok(categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                EventId = c.EventId,
                Name = c.Name,
                Description = c.Description,
                EntryFee = c.EntryFee,
                AgeGroup = c.AgeGroup,
                Distance = c.Distance,
                EnrolmentCount = c.Enrolments.Count(x => x.Status != "Cancelled")
            }));
        }

        // POST: api/events/{eventId}/categories
        [HttpPost("events/{eventId}/categories")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateCategory(int eventId, [FromBody] CreateCategoryDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var ev = await _context.Events.FindAsync(eventId);

            if (ev == null) return NotFound(new { message = "Event not found" });
            if (ev.OrganiserId != userId) return Forbid();

            var category = new Category
            {
                EventId = eventId,
                Name = model.Name,
                Description = model.Description,
                EntryFee = model.EntryFee,
                AgeGroup = model.AgeGroup,
                Distance = model.Distance
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                categoryId = category.CategoryId,
                message = "Category created successfully"
            });
        }

        // PUT: api/categories/{id}
        [HttpPut("categories/{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto model)
        {
            var userId = GetUserId();
            var category = await _context.Categories
                .Include(c => c.Events)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return NotFound(new { message = "Category not found" });
            if (category.Events.OrganiserId != userId) return Forbid();

            if (!string.IsNullOrEmpty(model.Name)) category.Name = model.Name;
            if (model.Description != null) category.Description = model.Description;
            if (model.EntryFee >= 0) category.EntryFee = model.EntryFee;
            if (model.AgeGroup != null) category.AgeGroup = model.AgeGroup;
            if (model.Distance != null) category.Distance = model.Distance;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Category updated successfully" });
        }

        // DELETE: api/categories/{id}
        [HttpDelete("categories/{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = GetUserId();
            var category = await _context.Categories
                .Include(c => c.Events)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return NotFound(new { message = "Category not found" });
            if (category.Events.OrganiserId != userId) return Forbid();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
