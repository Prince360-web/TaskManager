using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs;
using TaskManagement.API.Models;
using TaskManagement.API.Services.Interfaces;

namespace TaskManagement.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskItemDto>> GetAllTasks()
        {
            return await _context.TaskItems
                .Include(t => t.AssignedToUser)
                .Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedToUserId = t.AssignedToUserId,
                    AssignedToUserName = t.AssignedToUser != null ? t.AssignedToUser.FullName : null,
                    Status = (DTOs.TaskStatus)(int)t.Status, // Cast TaskStatus enum to int and then to DTOs.TaskStatus
                    DueDate = t.DueDate,
                    CreatedDate = t.CreatedDate
                })
                .ToListAsync();
        }

        public async Task<List<TaskItemDto>> GetTasksByUser(int userId)
        {
            return await _context.TaskItems
                .Include(t => t.AssignedToUser)
                .Where(t => t.AssignedToUserId == userId)
                .Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedToUserId = t.AssignedToUserId,
                    AssignedToUserName = t.AssignedToUser != null ? t.AssignedToUser.FullName : null,
                    Status = (DTOs.TaskStatus)(int)t.Status, // Cast TaskStatus enum to int and then to DTOs.TaskStatus
                    DueDate = t.DueDate,
                    CreatedDate = t.CreatedDate
                })
                .ToListAsync();
        }

        public async Task<List<TaskItemDto>> GetTasksByStatus(DTOs.TaskStatus status)
        {
            return await _context.TaskItems
                .Include(t => t.AssignedToUser)
                .Where(t => t.Status == (Models.TaskStatus)(int)status) 
                .Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedToUserId = t.AssignedToUserId,
                    AssignedToUserName = t.AssignedToUser != null ? t.AssignedToUser.FullName : null,
                    Status = status,
                    DueDate = t.DueDate,
                    CreatedDate = t.CreatedDate
                })
                .ToListAsync();
        }

        public async Task<List<TaskItemDto>> GetTasksCompletedSince(DateTime date)
        {
            return await _context.TaskItems
                .Include(t => t.AssignedToUser)
                .Where(t => t.Status == Models.TaskStatus.Completed && t.CreatedDate >= date) 
                .Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedToUserId = t.AssignedToUserId,
                    AssignedToUserName = t.AssignedToUser != null ? t.AssignedToUser.FullName : null,
                    Status = DTOs.TaskStatus.Completed, // Use the TaskStatus enum value directly
                    DueDate = t.DueDate,
                    CreatedDate = t.CreatedDate
                })
                .ToListAsync();
        }

        public async Task<TaskItemDto> GetTaskById(int id)
        {
            var task = await _context.TaskItems
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return null;

            return new TaskItemDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUser != null ? task.AssignedToUser.FullName : null,
                Status = (DTOs.TaskStatus)(int)task.Status, // Cast TaskStatus enum to int and then to DTOs.TaskStatus
                DueDate = task.DueDate,
                CreatedDate = task.CreatedDate
            };
        }

        public async Task<TaskItemDto> CreateTask(CreateTaskDto taskDto)
        {
            var task = new TaskItem
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                AssignedToUserId = taskDto.AssignedToUserId,
                Status = (Models.TaskStatus)(int)taskDto.Status, 
                DueDate = taskDto.DueDate,
                CreatedDate = DateTime.Now
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            await _context.Entry(task)
                .Reference(t => t.AssignedToUser)
                .LoadAsync();

            return new TaskItemDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUser != null ? task.AssignedToUser.FullName : null,
                Status = (DTOs.TaskStatus)(int)task.Status, 
                DueDate = task.DueDate,
                CreatedDate = task.CreatedDate
            };
        }

        public async Task<TaskItemDto> UpdateTask(int id, UpdateTaskDto taskDto)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null)
                return null;

            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.AssignedToUserId = taskDto.AssignedToUserId;
            task.Status = (Models.TaskStatus)(int)taskDto.Status; 
            task.DueDate = taskDto.DueDate;

            await _context.SaveChangesAsync();

            // Reload the task with user information
            await _context.Entry(task)
                .Reference(t => t.AssignedToUser)
                .LoadAsync();

            return new TaskItemDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUser != null ? task.AssignedToUser.FullName : null,
                Status = (DTOs.TaskStatus)(int)task.Status, // Cast TaskStatus enum to int and then to DTOs.TaskStatus
                DueDate = task.DueDate,
                CreatedDate = task.CreatedDate
            };
        }

        public async Task<bool> DeleteTask(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null)
                return false;

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}