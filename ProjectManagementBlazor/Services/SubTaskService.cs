using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;
using System.Net.Http.Json;

namespace ProjectManagementBlazor.Services
{
    public class SubTaskService : BaseApiService, ISubTaskService
    {
        public SubTaskService(HttpClient httpClient, IErrorHandlingService errorHandling)
            : base(httpClient, errorHandling)
        {
        }

        public async Task<ApiResponse<SubTaskResponse>> CreateSubTaskAsync(CreateSubTaskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                await _errorHandling.TriggerError("Название подзадачи обязательно");
                return ApiResponse<SubTaskResponse>.Fail("Название подзадачи обязательно");
            }

            return await SendRequestAsync<SubTaskResponse>(
                () => _httpClient.PostAsJsonAsync("api/subtasks", request),
                "Не удалось создать подзадачу");
        }

        public async Task<ApiResponse<SubTaskResponse>> GetSubTaskByIdAsync(Guid subTaskId)
        {
            return await SendRequestAsync<SubTaskResponse>(
                () => _httpClient.GetAsync($"api/subtasks/{subTaskId}"),
                "Не удалось получить данные подзадачи");
        }

        public async Task<ApiResponse<List<SubTaskResponse>>> GetBacklogItemSubTasksAsync(Guid backlogItemId)
        {
            return await SendRequestAsync<List<SubTaskResponse>>(
                () => _httpClient.GetAsync($"api/subtasks/backlog-item/{backlogItemId}"),
                "Не удалось получить список подзадач");
        }

        public async Task<ApiResponse<SubTaskResponse>> UpdateSubTaskAsync(Guid subTaskId, UpdateSubTaskRequest request)
        {
            if (request.Title != null && string.IsNullOrWhiteSpace(request.Title))
            {
                await _errorHandling.TriggerError("Название подзадачи не может быть пустым");
                return ApiResponse<SubTaskResponse>.Fail("Название подзадачи не может быть пустым");
            }

            return await SendRequestAsync<SubTaskResponse>(
                () => _httpClient.PutAsJsonAsync($"api/subtasks/{subTaskId}", request),
                "Не удалось обновить подзадачу");
        }

        public async Task<ApiResponse> DeleteSubTaskAsync(Guid subTaskId)
        {
            return await SendRequestAsync(
                () => _httpClient.DeleteAsync($"api/subtasks/{subTaskId}"),
                "Не удалось удалить подзадачу");
        }

        public async Task<ApiResponse<SubTaskResponse>> StartSubTaskAsync(StartSubTaskRequest request)
        {
            return await SendRequestAsync<SubTaskResponse>(
                () => _httpClient.PostAsync($"api/subtasks/{request.SubTaskId}/start", null),
                "Не удалось начать выполнение подзадачи");
        }

        public async Task<ApiResponse<SubTaskResponse>> CompleteSubTaskAsync(CompleteSubTaskRequest request)
        {
            return await SendRequestAsync<SubTaskResponse>(
                () => _httpClient.PostAsJsonAsync($"api/subtasks/{request.SubTaskId}/complete", request),
                "Не удалось завершить подзадачу");
        }

        public async Task<ApiResponse<SubTaskResponse>> ChangeStatusAsync(Guid subTaskId, ChangeSubTaskStatusRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewStatus))
            {
                await _errorHandling.TriggerError("Не указан новый статус");
                return ApiResponse<SubTaskResponse>.Fail("Не указан новый статус");
            }

            return await SendRequestAsync<SubTaskResponse>(
                () => _httpClient.PatchAsJsonAsync($"api/subtasks/{subTaskId}/status", request),
                "Не удалось изменить статус подзадачи");
        }

        public async Task<ApiResponse> ReorderSubTasksAsync(ReorderSubTasksRequest request)
        {
            if (request.Items == null || !request.Items.Any())
            {
                return ApiResponse.Fail("Нет данных для переупорядочивания");
            }

            return await SendRequestAsync(
                () => _httpClient.PostAsJsonAsync("api/subtasks/reorder", request),
                "Не удалось изменить порядок подзадач");
        }

        public async Task<ApiResponse<SubTaskStatisticsResponse>> GetSubTaskStatisticsAsync(Guid backlogItemId)
        {
            return await SendRequestAsync<SubTaskStatisticsResponse>(
                () => _httpClient.GetAsync($"api/subtasks/backlog-item/{backlogItemId}/statistics"),
                "Не удалось получить статистику подзадач");
        }

        public async Task<ApiResponse<BlockerResponse>> AddBlockerToSubTaskAsync(Guid subTaskId, AddBlockerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                await _errorHandling.TriggerError("Описание блокера обязательно");
                return ApiResponse<BlockerResponse>.Fail("Описание блокера обязательно");
            }

            return await SendRequestAsync<BlockerResponse>(
                () => _httpClient.PostAsJsonAsync($"api/subtasks/{subTaskId}/blockers", request),
                "Не удалось добавить блокер");
        }
    }
}