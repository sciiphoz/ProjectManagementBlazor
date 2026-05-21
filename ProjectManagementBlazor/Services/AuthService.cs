using Microsoft.AspNetCore.Components.Authorization;
using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProjectManagementBlazor.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(
            HttpClient httpClient,
            AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UsernameOrEmail))
                return ApiResponse<AuthResponse>.Fail("Введите логин или email");

            if (string.IsNullOrWhiteSpace(request.Password))
                return ApiResponse<AuthResponse>.Fail("Введите пароль");

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Login HTTP {response.StatusCode}: {content}");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(content, options);

                if (apiResponse == null)
                    return ApiResponse<AuthResponse>.Fail("Не удалось выполнить вход");

                if (apiResponse.Success && apiResponse.Data != null && _authStateProvider is CustomAuthStateProvider customProvider)
                {
                    await customProvider.MarkUserAsAuthenticated(apiResponse.Data.Token);
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login exception: {ex.Message}");
                return ApiResponse<AuthResponse>.Fail($"Не удалось выполнить вход: {ex.Message}");
            }
        }

        public async Task<ApiResponse> LogoutAsync()
        {
            if (_authStateProvider is CustomAuthStateProvider customProvider)
            {
                await customProvider.MarkUserAsLoggedOut();
            }
            return ApiResponse.Ok("Выход выполнен");
        }

        public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return ApiResponse<AuthResponse>.Fail("Введите имя пользователя");

            if (string.IsNullOrWhiteSpace(request.Email))
                return ApiResponse<AuthResponse>.Fail("Введите email");

            if (string.IsNullOrWhiteSpace(request.Password))
                return ApiResponse<AuthResponse>.Fail("Введите пароль");

            if (request.Password != request.ConfirmPassword)
                return ApiResponse<AuthResponse>.Fail("Пароли не совпадают");

            if (request.Password.Length < 6)
                return ApiResponse<AuthResponse>.Fail("Пароль должен содержать минимум 6 символов");

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Register HTTP {response.StatusCode}: {content}");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(content, options);

                return apiResponse ?? ApiResponse<AuthResponse>.Fail("Не удалось зарегистрироваться");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register exception: {ex.Message}");
                return ApiResponse<AuthResponse>.Fail($"Не удалось зарегистрироваться: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserResponse>> GetCurrentUserAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/users/me");
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<UserResponse>>(content, options);
                return apiResponse ?? ApiResponse<UserResponse>.Fail("Не удалось получить данные пользователя");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserResponse>.Fail($"Ошибка: {ex.Message}");
            }
        }
    }
}