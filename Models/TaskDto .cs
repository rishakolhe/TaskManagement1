using System;

namespace TaskManagement.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; } // Add this property if it's needed
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DueDate { get; set; }
        public int EmployeeId { get; set; }
    }

}
