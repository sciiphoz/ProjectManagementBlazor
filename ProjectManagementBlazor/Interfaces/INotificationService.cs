using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Responses;


namespace ProjectManagementBlazor.Interfaces
{
    public interface INotificationService
    {
        Task<ApiResponse<PagedResult<NotificationResponse>>> GetUserNotificationsAsync(PagedRequest request);
        Task<ApiResponse<int>> GetUnreadCountAsync();
        Task<ApiResponse> MarkAsReadAsync(Guid notificationId);
        Task<ApiResponse> MarkAllAsReadAsync();
    }
}