using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.API.DTOs;

namespace TaskManagement.API.Services.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskItemDto>> GetAllTasks();
        Task<List<TaskItemDto>> GetTasksByUser(int userId);
        Task<List<TaskItemDto>> GetTasksByStatus(DTOs.TaskStatus status);
        Task<List<TaskItemDto>> GetTasksCompletedSince(DateTime date);
        Task<TaskItemDto> GetTaskById(int id);
        Task<TaskItemDto> CreateTask(CreateTaskDto taskDto);
        Task<TaskItemDto> UpdateTask(int id, UpdateTaskDto taskDto);
        Task<bool> DeleteTask(int id);
    }
}