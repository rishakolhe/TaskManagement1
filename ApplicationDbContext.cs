using Microsoft.EntityFrameworkCore;
using Task = TaskManagement.Models.Task; // Alias for your Task model
using TaskManagement.Models;

namespace TaskManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }
}
