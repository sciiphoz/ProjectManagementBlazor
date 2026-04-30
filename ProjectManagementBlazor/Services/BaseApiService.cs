using Microsoft.AspNetCore.Components;
using ProjectManagementBlazor.DTO.Common;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProjectManagementBlazor.Services
{
    public abstract class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly IErrorHandlingService _errorHandling;
        protected readonly JsonSerializerOptions _options;
        private readonly NavigationManager? _navigationManager;

        protected BaseApiService(HttpClient httpClient, IErrorHandlingService errorHandling)
        {
            _httpClient = httpClient;
            _errorHandling = errorHandling;
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        protected BaseApiService(HttpClient httpClient, IErrorHandlingService errorHandling, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _errorHandling = errorHandling;
            _navigationManager = navigationManager;
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        protected async Task<ApiResponse<T>> SendRequestAsync<T>(
            Func<Task<HttpResponseMessage>> requestFunc,
            string errorMessage)
        {
            try
            {
                var response = await requestFunc();
                var content = await response.Content.ReadAsStringAsync();

                // Проверка 401 — токен истёк
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _navigationManager?.NavigateTo("/login", true);
                    return ApiResponse<T>.Fail("Требуется авторизация");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = $"Ошибка {response.StatusCode}: {errorMessage}";
                    await _errorHandling.TriggerError(error);
                    return ApiResponse<T>.Fail(error);
                }

                var result = JsonSerializer.Deserialize<ApiResponse<T>>(content, _options);
                return result ?? ApiResponse<T>.Fail("Ошибка десериализации ответа");
            }
            catch (HttpRequestException ex)
            {
                await _errorHandling.TriggerError($"Ошибка сети: {ex.Message}");
                return ApiResponse<T>.Fail("Ошибка подключения к серверу. Проверьте соединение.");
            }
            catch (TaskCanceledException)
            {
                await _errorHandling.TriggerError("Превышено время ожидания ответа от сервера");
                return ApiResponse<T>.Fail("Превышено время ожидания ответа от сервера");
            }
            catch (JsonException ex)
            {
                await _errorHandling.TriggerError($"Ошибка обработки данных: {ex.Message}");
                return ApiResponse<T>.Fail("Ошибка обработки данных от сервера");
            }
            catch (Exception ex)
            {
                await _errorHandling.TriggerError($"Неизвестная ошибка: {ex.Message}");
                return ApiResponse<T>.Fail("Произошла неизвестная ошибка");
            }
        }

        protected async Task<ApiResponse> SendRequestAsync(
            Func<Task<HttpResponseMessage>> requestFunc,
            string errorMessage)
        {
            try
            {
                var response = await requestFunc();
                var content = await response.Content.ReadAsStringAsync();

                // Проверка 401 — токен истёк
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _navigationManager?.NavigateTo("/login", true);
                    return ApiResponse.Fail("Требуется авторизация");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = $"Ошибка {response.StatusCode}: {errorMessage}";
                    await _errorHandling.TriggerError(error);
                    return ApiResponse.Fail(error);
                }

                var result = JsonSerializer.Deserialize<ApiResponse>(content, _options);
                return result ?? ApiResponse.Fail("Ошибка десериализации ответа");
            }
            catch (HttpRequestException ex)
            {
                await _errorHandling.TriggerError($"Ошибка сети: {ex.Message}");
                return ApiResponse.Fail("Ошибка подключения к серверу. Проверьте соединение.");
            }
            catch (TaskCanceledException)
            {
                await _errorHandling.TriggerError("Превышено время ожидания ответа от сервера");
                return ApiResponse.Fail("Превышено время ожидания ответа от сервера");
            }
            catch (Exception ex)
            {
                await _errorHandling.TriggerError($"Неизвестная ошибка: {ex.Message}");
                return ApiResponse.Fail("Произошла неизвестная ошибка");
            }
        }
    }
}