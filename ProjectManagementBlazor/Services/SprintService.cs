using System.Net.Http.Json;
using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;

namespace ProjectManagementBlazor.Services
{
    public class SprintService : ISprintService
    {
        private readonly HttpClient _httpClient;

        public SprintService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<SprintResponse>> CreateSprintAsync(CreateSprintRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/sprints", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<SprintResponse>>()
                   ?? ApiResponse<SprintResponse>.Fail("Ошибка создания спринта");
        }

        public async Task<ApiResponse<SprintResponse>> GetSprintByIdAsync(Guid sprintId)
        {
            var response = await _httpClient.GetAsync($"api/sprints/{sprintId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<SprintResponse>>()
                   ?? ApiResponse<SprintResponse>.Fail("Спринт не найден");
        }

        public async Task<ApiResponse<List<SprintBriefResponse>>> GetProjectSprintsAsync(Guid projectId)
        {
            var response = await _httpClient.GetAsync($"api/sprints/project/{projectId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<SprintBriefResponse>>>()
                   ?? ApiResponse<List<SprintBriefResponse>>.Fail("Ошибка получения спринтов");
        }

        public async Task<ApiResponse<SprintResponse>> UpdateSprintAsync(Guid sprintId, UpdateSprintRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/sprints/{sprintId}", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<SprintResponse>>()
                   ?? ApiResponse<SprintResponse>.Fail("Ошибка обновления спринта");
        }

        public async Task<ApiResponse> DeleteSprintAsync(Guid sprintId)
        {
            var response = await _httpClient.DeleteAsync($"api/sprints/{sprintId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка удаления спринта");
        }

        public async Task<ApiResponse<SprintResponse>> StartSprintAsync(StartSprintRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/sprints/{request.SprintId}/start", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<SprintResponse>>()
                   ?? ApiResponse<SprintResponse>.Fail("Ошибка запуска спринта");
        }

        public async Task<ApiResponse<SprintResponse>> CompleteSprintAsync(CompleteSprintRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/sprints/{request.SprintId}/complete", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<SprintResponse>>()
                   ?? ApiResponse<SprintResponse>.Fail("Ошибка завершения спринта");
        }

        public async Task<ApiResponse> CancelSprintAsync(Guid sprintId)
        {
            var response = await _httpClient.PostAsync($"api/sprints/{sprintId}/cancel", null);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка отмены спринта");
        }

        public async Task<ApiResponse<SprintBoardResponse>> GetSprintBoardAsync(Guid sprintId)
        {
            var response = await _httpClient.GetAsync($"api/sprints/{sprintId}/board");
            return await response.Content.ReadFromJsonAsync<ApiResponse<SprintBoardResponse>>()
                   ?? ApiResponse<SprintBoardResponse>.Fail("Ошибка получения доски спринта");
        }

        public async Task<ApiResponse> UpdateTaskStatusAsync(Guid taskId, string newStatus)
        {
            var response = await _httpClient.PatchAsync($"api/sprints/tasks/{taskId}/status",
                new StringContent($"\"{newStatus}\"", System.Text.Encoding.UTF8, "application/json"));
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка обновления статуса");
        }

        public async Task<ApiResponse> MoveToSprintAsync(MoveToSprintRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/sprints/move-to-sprint", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка перемещения задач");
        }

        public async Task<ApiResponse> MoveToBacklogAsync(Guid backlogItemId)
        {
            var response = await _httpClient.PostAsync($"api/sprints/{backlogItemId}/move-to-backlog", null);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка перемещения задачи");
        }

        public async Task<ApiResponse<SprintMetrics>> GetSprintMetricsAsync(Guid sprintId)
        {
            var response = await _httpClient.GetAsync($"api/sprints/{sprintId}/metrics");
            return await response.Content.ReadFromJsonAsync<ApiResponse<SprintMetrics>>()
                   ?? ApiResponse<SprintMetrics>.Fail("Ошибка получения метрик");
        }

        public async Task<ApiResponse<List<BurndownPoint>>> GetBurndownChartAsync(Guid sprintId)
        {
            var response = await _httpClient.GetAsync($"api/sprints/{sprintId}/burndown");
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<BurndownPoint>>>()
                   ?? ApiResponse<List<BurndownPoint>>.Fail("Ошибка получения графика");
        }

        public async Task<ApiResponse> SaveReviewNotesAsync(Guid sprintId, string notes)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/sprints/{sprintId}/review-notes", notes);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка сохранения заметок");
        }

        public async Task<ApiResponse> SaveRetrospectiveNotesAsync(Guid sprintId, string notes)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/sprints/{sprintId}/retrospective-notes", notes);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка сохранения заметок");
        }

        public async Task<ApiResponse<List<SprintVelocityHistory>>> GetSprintHistoryAsync(Guid projectId, int count = 5)
        {
            var response = await _httpClient.GetAsync($"api/sprints/project/{projectId}/history?count={count}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<SprintVelocityHistory>>>()
                   ?? ApiResponse<List<SprintVelocityHistory>>.Fail("Ошибка получения истории");
        }
    }
}