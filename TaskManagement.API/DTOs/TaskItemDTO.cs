using System;

namespace TaskManagement.API.DTOs
{
    public class TaskItemDto
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

    public class CreateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int? AssignedToUserId { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class UpdateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int? AssignedToUserId { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime DueDate { get; set; }
    }
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed
    }
}