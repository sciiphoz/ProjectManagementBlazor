using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;
using System.Net.Http.Json;

namespace ProjectManagementBlazor.Services
{
    public class RetrospectiveService : BaseApiService, IRetrospectiveService
    {
        public RetrospectiveService(HttpClient httpClient, IErrorHandlingService errorHandling)
            : base(httpClient, errorHandling)
        {
        }

        public async Task<ApiResponse<RetrospectiveBoardResponse>> GetRetrospectiveBoardAsync(Guid sprintId)
        {
            return await SendRequestAsync<RetrospectiveBoardResponse>(
                () => _httpClient.GetAsync($"api/retrospective/sprint/{sprintId}"),
                "Не удалось загрузить ретроспективу");
        }

        public async Task<ApiResponse<RetrospectiveItemResponse>> AddRetrospectiveItemAsync(Guid sprintId, AddRetrospectiveItemRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                await _errorHandling.TriggerError("Содержание элемента не может быть пустым");
                return ApiResponse<RetrospectiveItemResponse>.Fail("Содержание элемента не может быть пустым");
            }

            var validCategories = new[] { "Good", "Bad", "Idea", "Action" };
            if (!validCategories.Contains(request.Category))
            {
                await _errorHandling.TriggerError("Неверная категория элемента");
                return ApiResponse<RetrospectiveItemResponse>.Fail("Неверная категория элемента");
            }

            return await SendRequestAsync<RetrospectiveItemResponse>(
                () => _httpClient.PostAsJsonAsync($"api/retrospective/sprint/{sprintId}/items", request),
                "Не удалось добавить элемент");
        }

        public async Task<ApiResponse> VoteRetrospectiveItemAsync(Guid itemId)
        {
            return await SendRequestAsync(
                () => _httpClient.PostAsync($"api/retrospective/items/{itemId}/vote", null),
                "Не удалось проголосовать");
        }

        public async Task<ApiResponse> RemoveVoteAsync(Guid itemId)
        {
            return await SendRequestAsync(
                () => _httpClient.DeleteAsync($"api/retrospective/items/{itemId}/vote"),
                "Не удалось удалить голос");
        }

        public async Task<ApiResponse> DeleteRetrospectiveItemAsync(Guid itemId)
        {
            return await SendRequestAsync(
                () => _httpClient.DeleteAsync($"api/retrospective/items/{itemId}"),
                "Не удалось удалить элемент");
        }
    }
}