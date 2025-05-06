using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using TaskManagement.Client.Helpers;
using TaskManagement.Client.Models;

namespace TaskManagement.Client.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginRequest loginRequest);
        Task<RegisterResponse> Register(RegisterRequest registerRequest);
        Task Logout();
        Task<bool> IsAuthenticated();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthStateProvider _authStateProvider;
        private readonly JsonSerializerOptions _jsonOptions;

        public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider as AuthStateProvider;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<AuthResponse> Login(LoginRequest loginRequest)
        {
            try
            {
                // Ensure we're sending proper JSON content
                var content = new StringContent(
                    JsonSerializer.Serialize(loginRequest),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("http://localhost:5052/api/Auth/login", content);

              
                Console.WriteLine($"Login Status: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = JsonSerializer.Deserialize<AuthResponse>(
                        responseContent, _jsonOptions);

                    if (authResponse != null)
                    {
                        await _authStateProvider.SetAuthenticationStateAsync(authResponse);
                        return authResponse;
                    }
                }

                // Handle specific error responses
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    // Try to get the error message from response
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(
                            responseContent, _jsonOptions);
                        throw new Exception(errorResponse?.Message ?? "Invalid email or password");
                    }
                    catch (JsonException)
                    {
                        throw new Exception("Invalid email or password");
                    }
                }

                throw new Exception("Login failed. Please try again.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<RegisterResponse> Register(RegisterRequest registerRequest)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(registerRequest);
                Console.WriteLine($"Register Request: {jsonContent}");

                var content = new StringContent(
                    jsonContent,
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("http://localhost:5052/api/Auth/register", content);

                // For debugging
                Console.WriteLine($"Register Status: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<RegisterResponse>(
                        responseContent, _jsonOptions) ??
                        new RegisterResponse { Success = true, Message = "Registration successful" };
                }

                // Try to get error message from response
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(
                        responseContent, _jsonOptions);
                    throw new Exception(errorResponse?.Message ?? "Registration failed");
                }
                catch (JsonException)
                {
                    throw new Exception($"Registration failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register Exception: {ex.Message}");
                throw;
            }
        }

        public async Task Logout()
        {
            await _authStateProvider.ClearAuthenticationStateAsync();
        }

        public async Task<bool> IsAuthenticated()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.Identity?.IsAuthenticated ?? false;
        }
    }
  
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}