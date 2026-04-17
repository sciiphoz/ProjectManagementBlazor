using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;
using System.Net.Http.Json;

namespace ProjectManagementBlazor.Services
{
    public class SprintService : BaseApiService, ISprintService
    {

        public SprintService(HttpClient httpClient, IErrorHandlingService errorHandling)
            : base(httpClient, errorHandling)
        {
        }

        public async Task<ApiResponse<SprintResponse>> CreateSprintAsync(CreateSprintRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                await _errorHandling.TriggerError("Название спринта обязательно");
                return ApiResponse<SprintResponse>.Fail("Название спринта обязательно");
            }

            if (request.EndDate < request.StartDate)
            {
                await _errorHandling.TriggerError("Дата окончания не может быть раньше даты начала");
                return ApiResponse<SprintResponse>.Fail("Дата окончания не может быть раньше даты начала");
            }

            return await SendRequestAsync<SprintResponse>(
                () => _httpClient.PostAsJsonAsync("api/sprints", request),
                "Не удалось создать спринт");
        }

        public async Task<ApiResponse<SprintResponse>> GetSprintByIdAsync(Guid sprintId)
        {
            return await SendRequestAsync<SprintResponse>(
                () => _httpClient.GetAsync($"api/sprints/{sprintId}"),
                "Не удалось получить данные спринта");
        }

        public async Task<ApiResponse<List<SprintBriefResponse>>> GetProjectSprintsAsync(Guid projectId)
        {
            return await SendRequestAsync<List<SprintBriefResponse>>(
                () => _httpClient.GetAsync($"api/sprints/project/{projectId}"),
                "Не удалось получить список спринтов");
        }

        public async Task<ApiResponse<SprintResponse>> UpdateSprintAsync(Guid sprintId, UpdateSprintRequest request)
        {
            if (request.EndDate.HasValue && request.StartDate.HasValue && request.EndDate.Value < request.StartDate.Value)
            {
                await _errorHandling.TriggerError("Дата окончания не может быть раньше даты начала");
                return ApiResponse<SprintResponse>.Fail("Дата окончания не может быть раньше даты начала");
            }

            return await SendRequestAsync<SprintResponse>(
                () => _httpClient.PutAsJsonAsync($"api/sprints/{sprintId}", request),
                "Не удалось обновить спринт");
        }

        public async Task<ApiResponse> DeleteSprintAsync(Guid sprintId)
        {
            return await SendRequestAsync(
                () => _httpClient.DeleteAsync($"api/sprints/{sprintId}"),
                "Не удалось удалить спринт");
        }

        public async Task<ApiResponse<SprintResponse>> StartSprintAsync(StartSprintRequest request)
        {
            try
            {
                Console.WriteLine($"Запуск спринта {request.SprintId} с {request.BacklogItemIds.Count} задачами");

                var result = await SendRequestAsync<SprintResponse>(
                    () => _httpClient.PostAsJsonAsync($"api/sprints/{request.SprintId}/start", request),
                    "Не удалось запустить спринт");

                if (!result.Success)
                {
                    Console.WriteLine($"Ошибка запуска спринта: {result.Message}");
                    await _errorHandling.TriggerError(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение при запуске спринта: {ex.Message}");
                await _errorHandling.TriggerError($"Ошибка: {ex.Message}");
                return ApiResponse<SprintResponse>.Fail($"Ошибка: {ex.Message}");
            }
        }

        public async Task<ApiResponse<SprintResponse>> CompleteSprintAsync(CompleteSprintRequest request)
        {
            try
            {
                Console.WriteLine($"Завершение спринта {request.SprintId}");

                var result = await SendRequestAsync<SprintResponse>(
                    () => _httpClient.PostAsJsonAsync($"api/sprints/{request.SprintId}/complete", request),
                    "Не удалось завершить спринт");

                if (!result.Success)
                {
                    Console.WriteLine($"Ошибка завершения спринта: {result.Message}");
                    await _errorHandling.TriggerError(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение при завершении спринта: {ex.Message}");
                await _errorHandling.TriggerError($"Ошибка: {ex.Message}");
                return ApiResponse<SprintResponse>.Fail($"Ошибка: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CancelSprintAsync(Guid sprintId)
        {
            try
            {
                Console.WriteLine($"Отмена спринта {sprintId}");

                var result = await SendRequestAsync(
                    () => _httpClient.PostAsync($"api/sprints/{sprintId}/cancel", null),
                    "Не удалось отменить спринт");

                if (!result.Success)
                {
                    Console.WriteLine($"Ошибка отмены спринта: {result.Message}");
                    await _errorHandling.TriggerError(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение при отмене спринта: {ex.Message}");
                await _errorHandling.TriggerError($"Ошибка: {ex.Message}");
                return ApiResponse.Fail($"Ошибка: {ex.Message}");
            }
        }

        public async Task<ApiResponse<SprintBoardResponse>> GetSprintBoardAsync(Guid sprintId)
        {
            return await SendRequestAsync<SprintBoardResponse>(
                () => _httpClient.GetAsync($"api/sprints/{sprintId}/board"),
                "Не удалось получить доску спринта");
        }

        public async Task<ApiResponse> UpdateTaskStatusAsync(Guid taskId, string newStatus)
        {
            return await SendRequestAsync(
                () => _httpClient.PatchAsJsonAsync($"api/sprints/tasks/{taskId}/status", newStatus),
                "Не удалось обновить статус задачи");
        }

        public async Task<ApiResponse> MoveToSprintAsync(MoveToSprintRequest request)
        {
            if (request.BacklogItemIds == null || !request.BacklogItemIds.Any())
            {
                await _errorHandling.TriggerError("Не выбраны задачи для перемещения");
                return ApiResponse.Fail("Не выбраны задачи для перемещения");
            }

            try
            {
                Console.WriteLine($"Перемещение задач {string.Join(",", request.BacklogItemIds)} в спринт {request.SprintId}");

                var result = await SendRequestAsync(
                    () => _httpClient.PostAsJsonAsync("api/sprints/move-to-sprint", request),
                    "Не удалось переместить задачи");

                if (result.Success)
                {
                    Console.WriteLine("Задачи успешно перемещены");
                }
                else
                {
                    Console.WriteLine($"Ошибка перемещения: {result.Message}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение при перемещении: {ex.Message}");
                await _errorHandling.TriggerError($"Ошибка: {ex.Message}");
                return ApiResponse.Fail($"Ошибка: {ex.Message}");
            }
        }

        public async Task<ApiResponse> MoveToBacklogAsync(Guid backlogItemId)
        {
            return await SendRequestAsync(
                () => _httpClient.PostAsync($"api/sprints/{backlogItemId}/move-to-backlog", null),
                "Не удалось переместить задачу");
        }

        public async Task<ApiResponse<SprintMetrics>> GetSprintMetricsAsync(Guid sprintId)
        {
            return await SendRequestAsync<SprintMetrics>(
                () => _httpClient.GetAsync($"api/sprints/{sprintId}/metrics"),
                "Не удалось получить метрики спринта");
        }

        public async Task<ApiResponse<List<BurndownPoint>>> GetBurndownChartAsync(Guid sprintId)
        {
            var result = await SendRequestAsync<List<BurndownPoint>>(
                () => _httpClient.GetAsync($"api/sprints/{sprintId}/burndown"),
                "Не удалось получить данные графика");

            return result;
        }

        public async Task<ApiResponse> SaveReviewNotesAsync(Guid sprintId, string notes)
        {
            return await SendRequestAsync(
                () => _httpClient.PostAsJsonAsync($"api/sprints/{sprintId}/review-notes", notes),
                "Не удалось сохранить заметки");
        }

        public async Task<ApiResponse> SaveRetrospectiveNotesAsync(Guid sprintId, string notes)
        {
            return await SendRequestAsync(
                () => _httpClient.PostAsJsonAsync($"api/sprints/{sprintId}/retrospective-notes", notes),
                "Не удалось сохранить заметки");
        }

        public async Task<ApiResponse<List<SprintVelocityHistory>>> GetSprintHistoryAsync(Guid projectId, int count = 5)
        {
            return await SendRequestAsync<List<SprintVelocityHistory>>(
                () => _httpClient.GetAsync($"api/sprints/project/{projectId}/history?count={count}"),
                "Не удалось получить историю спринтов");
        }
    }
}