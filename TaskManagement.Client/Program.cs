using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Headers;
using TaskManagement.Client;
using TaskManagement.Client.Helpers;
using TaskManagement.Client.Services;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.Toast;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationHeaderHandler>();
builder.Services.AddBlazoredToast();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

// Register services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TaskManagement.Client.Services.ILocalStorageService, TaskManagement.Client.Services.LocalStorageService>();

// Configure the HttpClient for the API
builder.Services.AddScoped<AuthenticationHeaderHandler>();
builder.Services.AddHttpClient("TaskManagementAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5052");
})
.AddHttpMessageHandler<AuthenticationHeaderHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("TaskManagementAPI"));

builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddAuthorizationCore();


builder.Services.AddAuthorizationCore();
// Add authentication state provider
builder.Services.AddScoped<AuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient("AuthenticatedClient", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
})
.AddHttpMessageHandler<AuthenticationHeaderHandler>();
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});


builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("AuthenticatedClient"));

await builder.Build().RunAsync();