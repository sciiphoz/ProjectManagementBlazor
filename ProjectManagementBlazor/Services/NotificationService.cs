using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;

using System.Net.Http.Json;

namespace ProjectManagementBlazor.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;

        public NotificationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<PagedResult<NotificationResponse>>> GetUserNotificationsAsync(PagedRequest request)
        {
            var queryString = $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
            var response = await _httpClient.GetAsync($"api/notifications{queryString}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<NotificationResponse>>>()
                   ?? ApiResponse<PagedResult<NotificationResponse>>.Fail("Ошибка получения уведомлений");
        }

        public async Task<ApiResponse<int>> GetUnreadCountAsync()
        {
            var response = await _httpClient.GetAsync("api/notifications/unread-count");
            return await response.Content.ReadFromJsonAsync<ApiResponse<int>>()
                   ?? ApiResponse<int>.Fail("Ошибка получения количества уведомлений");
        }

        public async Task<ApiResponse> MarkAsReadAsync(Guid notificationId)
        {
            var response = await _httpClient.PostAsync($"api/notifications/{notificationId}/read", null);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка отметки уведомления");
        }

        public async Task<ApiResponse> MarkAllAsReadAsync()
        {
            var response = await _httpClient.PostAsync("api/notifications/mark-all-read", null);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка отметки уведомлений");
        }
    }
}