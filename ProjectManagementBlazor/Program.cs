using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProjectManagementBlazor;
using ProjectManagementBlazor.Handlers;
using ProjectManagementBlazor.Interfaces;
using ProjectManagementBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7000";

builder.Services.AddHttpClient("AuthorizedClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddScoped<AuthorizationMessageHandler>();
builder.Services.AddScoped(sp =>
{
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new AuthorizationMessageHandler(localStorage, navigationManager);
});

builder.Services.AddScoped<IErrorHandlingService, ErrorHandlingService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddSingleton<INotificationCounterService, NotificationCounterService>();
builder.Services.AddSingleton<DialogService>();

builder.Services.AddScoped<IAuthService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("AuthorizedClient");
    var errorHandling = sp.GetRequiredService<IErrorHandlingService>();
    return new AuthService(client, errorHandling, sp.GetRequiredService<AuthenticationStateProvider>());
});

builder.Services.AddScoped<IUserService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("AuthorizedClient");
    return new UserService(client, sp.GetRequiredService<IErrorHandlingService>());
});

builder.Services.AddScoped<IProjectService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("AuthorizedClient");
    return new ProjectService(client, sp.GetRequiredService<IErrorHandlingService>());
});

builder.Services.AddScoped<ISprintService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("AuthorizedClient");
    return new SprintService(client, sp.GetRequiredService<IErrorHandlingService>());
});

builder.Services.AddScoped<IBacklogService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("AuthorizedClient");
    return new BacklogService(client, sp.GetRequiredService<IErrorHandlingService>());
});

builder.Services.AddScoped<ISubTaskService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("AuthorizedClient");
    return new SubTaskService(client, sp.GetRequiredService<IErrorHandlingService>());
});

builder.Services.AddScoped<IDashboardService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("AuthorizedClient");
    return new DashboardService(client, sp.GetRequiredService<IErrorHandlingService>());
});

builder.Services.AddScoped<IReportService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("AuthorizedClient");
    return new ReportService(client, sp.GetRequiredService<IErrorHandlingService>());
});

builder.Services.AddScoped<IRetrospectiveService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("AuthorizedClient");
    return new RetrospectiveService(client, sp.GetRequiredService<IErrorHandlingService>());
});

builder.Services.AddScoped<INotificationService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("AuthorizedClient");
    return new NotificationService(client, sp.GetRequiredService<IErrorHandlingService>());
});

builder.Services.AddScoped<IActivityLogService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("AuthorizedClient");
    return new ActivityLogService(client, sp.GetRequiredService<IErrorHandlingService>());
});

await builder.Build().RunAsync();