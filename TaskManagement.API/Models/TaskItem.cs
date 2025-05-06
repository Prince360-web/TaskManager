using System;

namespace TaskManagement.API.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? AssignedToUserId { get; set; }
        public TaskStatus Status { get; set; } // "Pending", "In Progress", "Completed"
        public DateTime DueDate { get; set; }
        public DateTime CreatedDate { get; set; }


        // Navigation property
        public virtual User AssignedToUser { get; set; }
    }
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed
    }
}