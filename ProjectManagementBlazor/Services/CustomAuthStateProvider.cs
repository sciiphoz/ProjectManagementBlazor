using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using ProjectManagementBlazor.Interfaces;

namespace ProjectManagementBlazor.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CustomAuthStateProvider>? _logger;

        public CustomAuthStateProvider(
            ILocalStorageService localStorage,
            HttpClient httpClient,
            ILogger<CustomAuthStateProvider>? logger = null)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
            _logger = logger;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsStringAsync("authToken");

                if (string.IsNullOrEmpty(token))
                {
                    _logger?.LogDebug("Токен не найден в localStorage");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                var user = new ClaimsPrincipal(identity);

                _logger?.LogInformation("Пользователь авторизован: {UserCount} claims", identity.Claims.Count());
                return new AuthenticationState(user);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Ошибка при получении состояния аутентификации");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            try
            {
                await _localStorage.SetItemAsStringAsync("authToken", token);
                var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                var user = new ClaimsPrincipal(identity);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
                _logger?.LogInformation("Пользователь успешно авторизован");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Ошибка при авторизации пользователя");
                throw;
            }
        }

        public async Task MarkUserAsLoggedOut()
        {
            try
            {
                await _localStorage.RemoveItemAsync("authToken");
                _httpClient.DefaultRequestHeaders.Authorization = null;
                var identity = new ClaimsIdentity();
                var user = new ClaimsPrincipal(identity);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
                _logger?.LogInformation("Пользователь вышел из системы");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Ошибка при выходе пользователя");
            }
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
            {
                _logger?.LogWarning("JWT токен пуст");
                return new List<Claim>();
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(jwt))
                {
                    _logger?.LogWarning("Не удалось прочитать JWT токен");
                    return new List<Claim>();
                }

                var jsonToken = handler.ReadJwtToken(jwt);
                var claims = jsonToken.Claims.ToList();

                var roleClaim = claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
                if (roleClaim != null && !claims.Any(c => c.Type == ClaimTypes.Role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
                }

                _logger?.LogDebug("Успешно распарсено {ClaimCount} claims из JWT", claims.Count);
                return claims;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Ошибка при парсинге JWT токена");
                return new List<Claim>();
            }
        }
    }
}