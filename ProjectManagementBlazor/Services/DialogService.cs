namespace ProjectManagementBlazor.Services
{
    public class DialogService
    {
        public event Func<string, string, string, string, Task<bool>>? OnConfirm;

        public event Func<string, string, string, string, Task>? OnAlert;

        public async Task<bool> ConfirmAsync(string message, string title = "Подтверждение",
            string confirmText = "Да", string type = "default")
        {
            if (OnConfirm != null)
            {
                return await OnConfirm.Invoke(message, title, confirmText, type);
            }
            return false;
        }

        public async Task AlertAsync(string message, string title = "Уведомление",
            string icon = "", string type = "default")
        {
            if (OnAlert != null)
            {
                await OnAlert.Invoke(message, title, icon, type);
            }
        }
    }
}