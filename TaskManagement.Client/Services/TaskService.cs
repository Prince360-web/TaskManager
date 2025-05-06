using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskManagement.Client.Models;


namespace TaskManagement.Client.Services
{
    public class TaskService
    {
        private readonly HttpClient _httpClient;

        public TaskService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TaskItem>> GetTasksAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<TaskItem>>("http://localhost:5052/api/Tasks") ?? new List<TaskItem>();
        }

        public async Task<TaskItem> GetTaskByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<TaskItem>($"http://localhost:5052/api/Tasks/{id}");
        }

        public async Task<bool> CreateTaskAsync(TaskCreateRequest task)
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5052/api/Tasks", task);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTaskAsync(TaskUpdateRequest task)
        {
            var response = await _httpClient.PutAsJsonAsync($"http://localhost:5052/api/Tasks/{task.Id}", task);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"http://localhost:5052/api/Tasks/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<TaskItem>> GetTasksByStatusAsync(Models.TaskStatus status)
        {
            return await _httpClient.GetFromJsonAsync<List<TaskItem>>($"http://localhost:5052/api/Tasks/status/{status}") ?? new List<TaskItem>();
        }

        public async Task<List<TaskItem>> GetTasksByDueDateAsync(DateTime dateFrom, DateTime dateTo)
        {
            return await _httpClient.GetFromJsonAsync<List<TaskItem>>($"http://localhost:5052/api/Tasks/completed?dateFrom={dateFrom:yyyy-MM-dd}&dateTo={dateTo:yyyy-MM-dd}") ?? new List<TaskItem>();
        }
    }
}