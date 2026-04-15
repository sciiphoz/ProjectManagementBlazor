using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;
using System.Net.Http.Json;

namespace ProjectManagementBlazor.Services
{
    public class UserService : BaseApiService, IUserService
    {
        public UserService(HttpClient httpClient, IErrorHandlingService errorHandling)
            : base(httpClient, errorHandling)
        {
        }

        public async Task<ApiResponse<UserResponse>> GetUserByIdAsync(Guid userId)
        {
            return await SendRequestAsync<UserResponse>(
                () => _httpClient.GetAsync($"api/users/{userId}"),
                "Не удалось получить данные пользователя");
        }

        public async Task<ApiResponse<UserResponse>> UpdateProfileAsync(UpdateProfileRequest request)
        {
            if (request.Email != null && !IsValidEmail(request.Email))
            {
                await _errorHandling.TriggerError("Введите корректный email адрес");
                return ApiResponse<UserResponse>.Fail("Введите корректный email адрес");
            }

            return await SendRequestAsync<UserResponse>(
                () => _httpClient.PutAsJsonAsync("api/users/me", request),
                "Не удалось обновить профиль");
        }

        public async Task<ApiResponse> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                await _errorHandling.TriggerError("Введите текущий пароль");
                return ApiResponse.Fail("Введите текущий пароль");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                await _errorHandling.TriggerError("Введите новый пароль");
                return ApiResponse.Fail("Введите новый пароль");
            }

            if (request.NewPassword.Length < 6)
            {
                await _errorHandling.TriggerError("Новый пароль должен содержать минимум 6 символов");
                return ApiResponse.Fail("Новый пароль должен содержать минимум 6 символов");
            }

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                await _errorHandling.TriggerError("Новый пароль и подтверждение не совпадают");
                return ApiResponse.Fail("Новый пароль и подтверждение не совпадают");
            }

            return await SendRequestAsync(
                () => _httpClient.PostAsJsonAsync("api/users/change-password", request),
                "Не удалось сменить пароль");
        }

        public async Task<ApiResponse<PagedResult<UserResponse>>> GetAllUsersAsync(PagedRequest request)
        {
            var queryString = $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
            if (!string.IsNullOrEmpty(request.SearchTerm))
                queryString += $"&searchTerm={Uri.EscapeDataString(request.SearchTerm)}";
            if (!string.IsNullOrEmpty(request.SortBy))
                queryString += $"&sortBy={request.SortBy}&sortDescending={request.SortDescending}";

            return await SendRequestAsync<PagedResult<UserResponse>>(
                () => _httpClient.GetAsync($"api/users{queryString}"),
                "Не удалось получить список пользователей");
        }

        public async Task<ApiResponse<List<UserBriefResponse>>> GetProjectUsersAsync(Guid projectId)
        {
            return await SendRequestAsync<List<UserBriefResponse>>(
                () => _httpClient.GetAsync($"api/users/project/{projectId}"),
                "Не удалось получить участников проекта");
        }

        #region Private Methods

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}