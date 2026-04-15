using Microsoft.AspNetCore.Components.Authorization;
using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;
using System.Net.Http.Json;

namespace ProjectManagementBlazor.Services
{
    public class AuthService : BaseApiService, IAuthService
    {
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(
            HttpClient httpClient,
            IErrorHandlingService errorHandling,
            AuthenticationStateProvider authStateProvider)
            : base(httpClient, errorHandling)
        {
            _authStateProvider = authStateProvider;
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UsernameOrEmail))
            {
                await _errorHandling.TriggerError("Введите логин или email");
                return ApiResponse<AuthResponse>.Fail("Введите логин или email");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                await _errorHandling.TriggerError("Введите пароль");
                return ApiResponse<AuthResponse>.Fail("Введите пароль");
            }

            var result = await SendRequestAsync<AuthResponse>(
                () => _httpClient.PostAsJsonAsync("api/auth/login", request),
                "Не удалось выполнить вход");

            if (result.Success && result.Data != null && _authStateProvider is CustomAuthStateProvider customProvider)
            {
                await customProvider.MarkUserAsAuthenticated(result.Data.Token);
            }

            return result;
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
            {
                await _errorHandling.TriggerError("Введите имя пользователя");
                return ApiResponse<AuthResponse>.Fail("Введите имя пользователя");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                await _errorHandling.TriggerError("Введите email");
                return ApiResponse<AuthResponse>.Fail("Введите email");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                await _errorHandling.TriggerError("Введите пароль");
                return ApiResponse<AuthResponse>.Fail("Введите пароль");
            }

            if (request.Password != request.ConfirmPassword)
            {
                await _errorHandling.TriggerError("Пароли не совпадают");
                return ApiResponse<AuthResponse>.Fail("Пароли не совпадают");
            }

            if (request.Password.Length < 6)
            {
                await _errorHandling.TriggerError("Пароль должен содержать минимум 6 символов");
                return ApiResponse<AuthResponse>.Fail("Пароль должен содержать минимум 6 символов");
            }

            return await SendRequestAsync<AuthResponse>(
                () => _httpClient.PostAsJsonAsync("api/auth/register", request),
                "Не удалось зарегистрироваться");
        }

        public async Task<ApiResponse<UserResponse>> GetCurrentUserAsync()
        {
            return await SendRequestAsync<UserResponse>(
                () => _httpClient.GetAsync("api/users/me"),
                "Не удалось получить данные пользователя");
        }
    }
}