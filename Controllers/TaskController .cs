using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // For async tasks
using Task = TaskManagement.Models.Task; // Alias for Task model
using TaskManagement.Data;
using TaskManagement.DTOs;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TaskController> _logger;

        public TaskController(ApplicationDbContext context, ILogger<TaskController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Task>>> GetTasks()
        {
            return await _context.Tasks.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Task>> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> PostTask(TaskDto taskDto)
        {
            if (taskDto == null)
            {
                return BadRequest("Task cannot be null.");
            }

            if (taskDto.EmployeeId != 0 && !_context.Employees.Any(e => e.Id == taskDto.EmployeeId))
            {
                return BadRequest("Invalid EmployeeId.");
            }

            var task = new Task
            {
                Name = taskDto.Name,
                Description = taskDto.Description,
                IsCompleted = taskDto.IsCompleted,
                DueDate = taskDto.DueDate,
                EmployeeId = taskDto.EmployeeId
            };

            _context.Tasks.Add(task);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while saving the task.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, taskDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(int id, TaskDto taskDto)
        {
            if (taskDto == null || id != taskDto.Id)
            {
                return BadRequest();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            task.Name = taskDto.Name;
            task.Description = taskDto.Description;
            task.IsCompleted = taskDto.IsCompleted;
            task.DueDate = taskDto.DueDate;
            task.EmployeeId = taskDto.EmployeeId;

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
