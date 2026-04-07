using System.Net.Http.Json;
using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;

namespace ProjectManagementBlazor.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<UserResponse>> GetUserByIdAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"api/users/{userId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<UserResponse>>()
                   ?? ApiResponse<UserResponse>.Fail("Пользователь не найден");
        }

        public async Task<ApiResponse<UserResponse>> UpdateProfileAsync(UpdateProfileRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync("api/users/me", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<UserResponse>>()
                   ?? ApiResponse<UserResponse>.Fail("Ошибка обновления профиля");
        }

        public async Task<ApiResponse> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/change-password", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка смены пароля");
        }

        public async Task<ApiResponse<PagedResult<UserResponse>>> GetAllUsersAsync(PagedRequest request)
        {
            var queryString = $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
            if (!string.IsNullOrEmpty(request.SearchTerm))
                queryString += $"&searchTerm={request.SearchTerm}";
            if (!string.IsNullOrEmpty(request.SortBy))
                queryString += $"&sortBy={request.SortBy}&sortDescending={request.SortDescending}";

            var response = await _httpClient.GetAsync($"api/users{queryString}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<UserResponse>>>()
                   ?? ApiResponse<PagedResult<UserResponse>>.Fail("Ошибка получения пользователей");
        }

        public async Task<ApiResponse<List<UserBriefResponse>>> GetProjectUsersAsync(Guid projectId)
        {
            var response = await _httpClient.GetAsync($"api/users/project/{projectId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<UserBriefResponse>>>()
                   ?? ApiResponse<List<UserBriefResponse>>.Fail("Ошибка получения участников проекта");
        }
    }
}