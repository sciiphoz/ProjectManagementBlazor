using Microsoft.JSInterop;
using ProjectManagementBlazor.Services;

namespace ProjectManagementBlazor.Services
{
    public interface ILocalStorageService
    {
        Task<T?> GetItemAsync<T>(string key);
        Task<string?> GetItemAsStringAsync(string key);
        Task SetItemAsync<T>(string key, T value);
        Task SetItemAsStringAsync(string key, string value);
        Task RemoveItemAsync(string key);
        Task ClearAsync();
    }

    public class LocalStorageService : ILocalStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<T?> GetItemAsync<T>(string key)
        {
            var json = await GetItemAsStringAsync(key);
            return string.IsNullOrEmpty(json) ? default : System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        public async Task<string?> GetItemAsStringAsync(string key)
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        }

        public async Task SetItemAsync<T>(string key, T value)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            await SetItemAsStringAsync(key, json);
        }

        public async Task SetItemAsStringAsync(string key, string value)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
        }

        public async Task RemoveItemAsync(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public async Task ClearAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.clear");
        }
    }
}