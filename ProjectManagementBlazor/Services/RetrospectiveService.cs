
using ProjectManagementBlazor.Interfaces;
using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using System.Net.Http.Json;

namespace ProjectManagementBlazor.Services
{
    public class RetrospectiveService : IRetrospectiveService
    {
        private readonly HttpClient _httpClient;

        public RetrospectiveService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<RetrospectiveBoardResponse>> GetRetrospectiveBoardAsync(Guid sprintId)
        {
            var response = await _httpClient.GetAsync($"api/retrospective/sprint/{sprintId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<RetrospectiveBoardResponse>>()
                   ?? ApiResponse<RetrospectiveBoardResponse>.Fail("Ошибка получения ретроспективы");
        }

        public async Task<ApiResponse<RetrospectiveItemResponse>> AddRetrospectiveItemAsync(Guid sprintId, AddRetrospectiveItemRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/retrospective/sprint/{sprintId}/items", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<RetrospectiveItemResponse>>()
                   ?? ApiResponse<RetrospectiveItemResponse>.Fail("Ошибка добавления элемента");
        }

        public async Task<ApiResponse> VoteRetrospectiveItemAsync(Guid itemId)
        {
            var response = await _httpClient.PostAsync($"api/retrospective/items/{itemId}/vote", null);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка голосования");
        }

        public async Task<ApiResponse> RemoveVoteAsync(Guid itemId)
        {
            var response = await _httpClient.DeleteAsync($"api/retrospective/items/{itemId}/vote");
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка удаления голоса");
        }

        public async Task<ApiResponse> DeleteRetrospectiveItemAsync(Guid itemId)
        {
            var response = await _httpClient.DeleteAsync($"api/retrospective/items/{itemId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка удаления элемента");
        }
    }
}