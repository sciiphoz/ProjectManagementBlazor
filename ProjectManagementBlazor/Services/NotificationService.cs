using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;
using System.Net.Http.Json;

namespace ProjectManagementBlazor.Services
{
    public class NotificationService : BaseApiService, INotificationService
    {
        public NotificationService(HttpClient httpClient, IErrorHandlingService errorHandling)
            : base(httpClient, errorHandling)
        {
        }

        public async Task<ApiResponse<PagedResult<NotificationResponse>>> GetUserNotificationsAsync(PagedRequest request)
        {
            var queryString = $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

            return await SendRequestAsync<PagedResult<NotificationResponse>>(
                () => _httpClient.GetAsync($"api/notifications{queryString}"),
                "Не удалось загрузить уведомления");
        }

        public async Task<ApiResponse<int>> GetUnreadCountAsync()
        {
            var result = await SendRequestAsync<int>(
                () => _httpClient.GetAsync("api/notifications/unread-count"),
                "Не удалось получить количество уведомлений");

            return result;
        }

        public async Task<ApiResponse> MarkAsReadAsync(Guid notificationId)
        {
            return await SendRequestAsync(
                () => _httpClient.PostAsync($"api/notifications/{notificationId}/read", null),
                "Не удалось отметить уведомление");
        }

        public async Task<ApiResponse> MarkAllAsReadAsync()
        {
            return await SendRequestAsync(
                () => _httpClient.PostAsync("api/notifications/mark-all-read", null),
                "Не удалось отметить все уведомления");
        }
    }
}