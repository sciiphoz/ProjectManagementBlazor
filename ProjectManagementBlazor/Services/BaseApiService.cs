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

        protected async Task<ApiResponse> SendRequestAsync(
            Func<Task<HttpResponseMessage>> requestFunc,
            string errorMessage)
        {
            try
            {
                var response = await requestFunc();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    return apiResponse ?? ApiResponse.Ok();
                }
                else
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    return apiResponse ?? ApiResponse.Fail(errorMessage);
                }
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail($"{errorMessage}: {ex.Message}");
            }
        }

        protected async Task<ApiResponse<T>> SendRequestAsync<T>(
            Func<Task<HttpResponseMessage>> requestFunc,
            string errorMessage)
        {
            try
            {
                var response = await requestFunc();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
                    return apiResponse ?? ApiResponse<T>.Fail(errorMessage);
                }
                else
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
                    return apiResponse ?? ApiResponse<T>.Fail(errorMessage);
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.Fail($"{errorMessage}: {ex.Message}");
            }
        }
    }
}