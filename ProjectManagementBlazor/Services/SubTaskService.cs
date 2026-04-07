using System.Net.Http.Json;
using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;

namespace ProjectManagementBlazor.Services
{
    public class SubTaskService : ISubTaskService
    {
        private readonly HttpClient _httpClient;

        public SubTaskService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<SubTaskResponse>> CreateSubTaskAsync(CreateSubTaskRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/subtasks", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<SubTaskResponse>>()
                   ?? ApiResponse<SubTaskResponse>.Fail("Ошибка создания подзадачи");
        }

        public async Task<ApiResponse<SubTaskResponse>> GetSubTaskByIdAsync(Guid subTaskId)
        {
            var response = await _httpClient.GetAsync($"api/subtasks/{subTaskId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<SubTaskResponse>>()
                   ?? ApiResponse<SubTaskResponse>.Fail("Подзадача не найдена");
        }

        public async Task<ApiResponse<List<SubTaskResponse>>> GetBacklogItemSubTasksAsync(Guid backlogItemId)
        {
            var response = await _httpClient.GetAsync($"api/subtasks/backlog-item/{backlogItemId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<SubTaskResponse>>>()
                   ?? ApiResponse<List<SubTaskResponse>>.Fail("Ошибка получения подзадач");
        }

        public async Task<ApiResponse<SubTaskResponse>> UpdateSubTaskAsync(Guid subTaskId, UpdateSubTaskRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/subtasks/{subTaskId}", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<SubTaskResponse>>()
                   ?? ApiResponse<SubTaskResponse>.Fail("Ошибка обновления подзадачи");
        }

        public async Task<ApiResponse> DeleteSubTaskAsync(Guid subTaskId)
        {
            var response = await _httpClient.DeleteAsync($"api/subtasks/{subTaskId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка удаления подзадачи");
        }

        public async Task<ApiResponse<SubTaskResponse>> StartSubTaskAsync(StartSubTaskRequest request)
        {
            var response = await _httpClient.PostAsync($"api/subtasks/{request.SubTaskId}/start", null);
            return await response.Content.ReadFromJsonAsync<ApiResponse<SubTaskResponse>>()
                   ?? ApiResponse<SubTaskResponse>.Fail("Ошибка начала работы");
        }

        public async Task<ApiResponse<SubTaskResponse>> CompleteSubTaskAsync(CompleteSubTaskRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/subtasks/{request.SubTaskId}/complete", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<SubTaskResponse>>()
                   ?? ApiResponse<SubTaskResponse>.Fail("Ошибка завершения подзадачи");
        }

        public async Task<ApiResponse<SubTaskResponse>> ChangeStatusAsync(Guid subTaskId, ChangeSubTaskStatusRequest request)
        {
            var response = await _httpClient.PatchAsJsonAsync($"api/subtasks/{subTaskId}/status", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<SubTaskResponse>>()
                   ?? ApiResponse<SubTaskResponse>.Fail("Ошибка изменения статуса");
        }

        public async Task<ApiResponse> ReorderSubTasksAsync(ReorderSubTasksRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/subtasks/reorder", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка изменения порядка");
        }

        public async Task<ApiResponse<SubTaskStatisticsResponse>> GetSubTaskStatisticsAsync(Guid backlogItemId)
        {
            var response = await _httpClient.GetAsync($"api/subtasks/backlog-item/{backlogItemId}/statistics");
            return await response.Content.ReadFromJsonAsync<ApiResponse<SubTaskStatisticsResponse>>()
                   ?? ApiResponse<SubTaskStatisticsResponse>.Fail("Ошибка получения статистики");
        }

        public async Task<ApiResponse<BlockerResponse>> AddBlockerToSubTaskAsync(Guid subTaskId, AddBlockerRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/subtasks/{subTaskId}/blockers", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<BlockerResponse>>()
                   ?? ApiResponse<BlockerResponse>.Fail("Ошибка добавления блокера");
        }
    }
}