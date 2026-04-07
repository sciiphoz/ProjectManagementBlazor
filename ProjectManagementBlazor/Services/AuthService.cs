// Services/AuthService.cs
using System.Net.Http.Json;
using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using Microsoft.AspNetCore.Components.Authorization;
using ProjectManagementBlazor.Interfaces;

namespace ProjectManagementBlazor.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();

            if (result?.Success == true && result.Data != null && _authStateProvider is CustomAuthStateProvider customProvider)
            {
                await customProvider.MarkUserAsAuthenticated(result.Data.Token);
            }

            return result ?? ApiResponse<AuthResponse>.Fail("Ошибка соединения");
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
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>()
                   ?? ApiResponse<AuthResponse>.Fail("Ошибка соединения");
        }

        public async Task<ApiResponse<UserResponse>> GetCurrentUserAsync()
        {
            var response = await _httpClient.GetAsync("api/users/me");
            return await response.Content.ReadFromJsonAsync<ApiResponse<UserResponse>>()
                   ?? ApiResponse<UserResponse>.Fail("Ошибка получения текущего пользователя");
        }
    }
}