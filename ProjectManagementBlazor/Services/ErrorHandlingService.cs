namespace ProjectManagementBlazor.Services
{
    public interface IErrorHandlingService
    {
        event Func<string, Task>? OnError;
        Task TriggerError(string message);
        void ClearError();
        string? GetLastError();
    }

    public class ErrorHandlingService : IErrorHandlingService
    {
        public event Func<string, Task>? OnError;
        private string? _lastError;

        public async Task TriggerError(string message)
        {
            _lastError = message;
            if (OnError != null)
            {
                await OnError.Invoke(message);
            }
        }

        public void ClearError()
        {
            _lastError = null;
        }

        public string? GetLastError()
        {
            return _lastError;
        }
    }
}