using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using TaskManagement.Client.Models;
using TaskManagement.Client.Services;

namespace TaskManagement.Client.Helpers
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly TaskManagement.Client.Services.ILocalStorageService _localStorage;
        private readonly AuthenticationState _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public AuthStateProvider(HttpClient httpClient, TaskManagement.Client.Services.ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return _anonymous;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);

            return CreateAuthenticationState(savedToken);
        }

        public async Task SetAuthenticationStateAsync(AuthResponse authResponse)
        {
            if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
            {
                await _localStorage.SetItemAsync("authToken", authResponse.Token);
                await _localStorage.SetItemAsync("userId", authResponse.UserId);
                await _localStorage.SetItemAsync("userRole", authResponse.Role);
                await _localStorage.SetItemAsync("userName", authResponse.FullName);

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.Token);

                var authState = CreateAuthenticationState(authResponse.Token);
                NotifyAuthenticationStateChanged(Task.FromResult(authState));
            }
        }

        public async Task ClearAuthenticationStateAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("userId");
            await _localStorage.RemoveItemAsync("userRole");
            await _localStorage.RemoveItemAsync("userName");

            _httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        }

        private AuthenticationState CreateAuthenticationState(string token)
        {
            // This simplified example doesn't validate the JWT - in production, you'd want to do that
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "User"),
                new Claim(ClaimTypes.Role, "User")
            };

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public override bool Equals(object? obj)
        {
            return obj is AuthStateProvider provider &&
                   EqualityComparer<HttpClient>.Default.Equals(_httpClient, provider._httpClient) &&
                   EqualityComparer<TaskManagement.Client.Services.ILocalStorageService>.Default.Equals(_localStorage, provider._localStorage) &&
                   EqualityComparer<AuthenticationState>.Default.Equals(_anonymous, provider._anonymous);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
public class AuthenticationHeaderHandler : DelegatingHandler
{
    private readonly TaskManagement.Client.Services.ILocalStorageService _localStorage;

    public AuthenticationHeaderHandler(TaskManagement.Client.Services.ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Get token from local storage
        var token = await _localStorage.GetItemAsync<string>("authToken");

        // Add token to request if it exists
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        // Continue with the request
        return await base.SendAsync(request, cancellationToken);
    }
}

