using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Client.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? AssignedToUserId { get; set; }
        public string AssignedToUserName { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class TaskCreateRequest
    {
        [Required]
        [StringLength(100, ErrorMessage = "Title is too long.")]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public int? AssignedToUserId { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        [Required]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
    }

    public class TaskUpdateRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Title is too long.")]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public int? AssignedToUserId { get; set; }

        [Required]
        public TaskStatus Status { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }

    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed
    }
}