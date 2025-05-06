using System.Net.Http.Json;
using TaskManagement.Client.Models;

namespace TaskManagement.Client.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<User>>("http://localhost:5052/api/Users") ?? new List<User>();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<User>($"http://localhost:5052/api/Users/{id}");
        }

        public async Task<bool> UpdateUserAsync(UserUpdateRequest user)
        {
            var response = await _httpClient.PutAsJsonAsync($"http://localhost:5052/api/Users/{user.Id}", user);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"http://localhost:5052/api/Users/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<User>>("http://localhost:5052/api/Users/");
        }
    }
}