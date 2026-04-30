using ProjectManagementBlazor.Interfaces;

namespace ProjectManagementBlazor.Services
{
    public class NotificationCounterService : INotificationCounterService
    {
        private int _currentCount = 0;

        public event Action<int>? OnUnreadCountChanged;

        public Task UpdateUnreadCount(int count)
        {
            _currentCount = count;
            OnUnreadCountChanged?.Invoke(count);
            return Task.CompletedTask;
        }

        public Task<int> GetCurrentCount()
        {
            return Task.FromResult(_currentCount);
        }
    }
}