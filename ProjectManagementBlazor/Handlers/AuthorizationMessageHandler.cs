using Microsoft.AspNetCore.Components;
using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Services;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ProjectManagementBlazor.Handlers
{
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private static readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);

        public AuthorizationMessageHandler(ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath ?? "";
            var isAuthEndpoint = path.Contains("/auth/");

            if (!isAuthEndpoint)
            {
                var token = await _localStorage.GetItemAsStringAsync("authToken");
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized && !isAuthEndpoint)
            {
                var newToken = await TryRefreshToken();

                if (!string.IsNullOrEmpty(newToken))
                {
                    var retryRequest = await CloneRequest(request);
                    retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                    return await base.SendAsync(retryRequest, cancellationToken);
                }

                await _localStorage.RemoveItemAsync("authToken");
                await _localStorage.RemoveItemAsync("refreshToken");
                _navigationManager.NavigateTo("/login", true);

                return response;
            }

            return response;
        }

        private async Task<string?> TryRefreshToken()
        {
            if (!await _refreshLock.WaitAsync(TimeSpan.FromSeconds(3)))
                return null;

            try
            {
                var refreshToken = await _localStorage.GetItemAsStringAsync("refreshToken");
                if (string.IsNullOrEmpty(refreshToken))
                    return null;

                var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh-token")
                {
                    Content = JsonContent.Create(new { RefreshToken = refreshToken })
                };

                var response = await base.SendAsync(refreshRequest, CancellationToken.None);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(
                        content,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (result?.Success == true && result.Data != null)
                    {
                        await _localStorage.SetItemAsStringAsync("authToken", result.Data.Token);
                        if (!string.IsNullOrEmpty(result.Data.RefreshToken))
                            await _localStorage.SetItemAsStringAsync("refreshToken", result.Data.RefreshToken);
                        return result.Data.Token;
                    }
                }
            }
            catch { }
            finally
            {
                _refreshLock.Release();
            }

            return null;
        }

        private async Task<HttpRequestMessage> CloneRequest(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri);
            if (request.Content != null)
            {
                var content = await request.Content.ReadAsStringAsync();
                clone.Content = new StringContent(content);
                if (request.Content.Headers.ContentType != null)
                    clone.Content.Headers.ContentType = request.Content.Headers.ContentType;
            }
            foreach (var header in request.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            return clone;
        }
    }
}