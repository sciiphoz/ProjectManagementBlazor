namespace ProjectManagementBlazor.Interfaces
{
    public interface INotificationCounterService
    {
        event Action<int>? OnUnreadCountChanged;
        Task UpdateUnreadCount(int count);
        Task<int> GetCurrentCount();
    }
}