using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;
using System.Net.Http.Json;

namespace ProjectManagementBlazor.Services
{
    public class BacklogService : BaseApiService, IBacklogService
    {
        public BacklogService(HttpClient httpClient, IErrorHandlingService errorHandling)
            : base(httpClient, errorHandling)
        {
        }

        public async Task<ApiResponse<BacklogItemResponse>> CreateBacklogItemAsync(CreateBacklogItemRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                await _errorHandling.TriggerError("Заголовок задачи обязателен");
                return ApiResponse<BacklogItemResponse>.Fail("Заголовок задачи обязателен");
            }

            return await SendRequestAsync<BacklogItemResponse>(
                () => _httpClient.PostAsJsonAsync("api/backlog", request),
                "Не удалось создать задачу");
        }

        public async Task<ApiResponse<BacklogItemResponse>> GetBacklogItemByIdAsync(Guid id)
        {
            return await SendRequestAsync<BacklogItemResponse>(
                () => _httpClient.GetAsync($"api/backlog/{id}"),
                "Не удалось получить данные задачи");
        }

        public async Task<ApiResponse<PagedResult<BacklogItemResponse>>> GetProjectBacklogAsync(Guid projectId, PagedRequest request)
        {
            var queryString = $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
            if (!string.IsNullOrEmpty(request.SearchTerm))
                queryString += $"&searchTerm={Uri.EscapeDataString(request.SearchTerm)}";

            Console.WriteLine($"Запрос к API: api/backlog/project/{projectId}{queryString}");

            var result = await SendRequestAsync<PagedResult<BacklogItemResponse>>(
                () => _httpClient.GetAsync($"api/backlog/project/{projectId}{queryString}"),
                "Не удалось получить бэклог проекта");

            if (result.Success && result.Data != null)
            {
                Console.WriteLine($"Получено задач: {result.Data.Items.Count}");
                foreach (var item in result.Data.Items)
                {
                    Console.WriteLine($"Задача: {item.Title}, Статус: {item.Status}, Исполнитель: {item.Assignee?.Id}");
                }
            }
            else
            {
                Console.WriteLine($"Ошибка получения бэклога: {result.Message}");
            }

            return result;
        }

        public async Task<ApiResponse<BacklogItemResponse>> UpdateBacklogItemAsync(Guid id, UpdateBacklogItemRequest request)
        {
            if (request.Title != null && string.IsNullOrWhiteSpace(request.Title))
            {
                await _errorHandling.TriggerError("Заголовок задачи не может быть пустым");
                return ApiResponse<BacklogItemResponse>.Fail("Заголовок задачи не может быть пустым");
            }

            return await SendRequestAsync<BacklogItemResponse>(
                () => _httpClient.PutAsJsonAsync($"api/backlog/{id}", request),
                "Не удалось обновить задачу");
        }

        public async Task<ApiResponse> DeleteBacklogItemAsync(Guid id)
        {
            return await SendRequestAsync(
                () => _httpClient.DeleteAsync($"api/backlog/{id}"),
                "Не удалось удалить задачу");
        }

        public async Task<ApiResponse<BacklogItemResponse>> ChangeStatusAsync(Guid id, ChangeTaskStatusRequest request)
        {
            return await SendRequestAsync<BacklogItemResponse>(
                () => _httpClient.PatchAsJsonAsync($"api/backlog/{id}/status", request),
                "Не удалось изменить статус задачи");
        }

        public async Task<ApiResponse> ReorderBacklogAsync(ReorderBacklogRequest request)
        {
            if (request.Items == null || !request.Items.Any())
            {
                return ApiResponse.Fail("Нет данных для переупорядочивания");
            }

            return await SendRequestAsync(
                () => _httpClient.PostAsJsonAsync("api/backlog/reorder", request),
                "Не удалось изменить порядок задач");
        }

        public async Task<ApiResponse<CommentResponse>> AddCommentAsync(Guid backlogItemId, AddCommentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                await _errorHandling.TriggerError("Текст комментария не может быть пустым");
                return ApiResponse<CommentResponse>.Fail("Текст комментария не может быть пустым");
            }

            return await SendRequestAsync<CommentResponse>(
                () => _httpClient.PostAsJsonAsync($"api/backlog/{backlogItemId}/comments", request),
                "Не удалось добавить комментарий");
        }

        public async Task<ApiResponse<CommentResponse>> UpdateCommentAsync(Guid commentId, UpdateCommentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                await _errorHandling.TriggerError("Текст комментария не может быть пустым");
                return ApiResponse<CommentResponse>.Fail("Текст комментария не может быть пустым");
            }

            return await SendRequestAsync<CommentResponse>(
                () => _httpClient.PutAsJsonAsync($"api/backlog/comments/{commentId}", request),
                "Не удалось обновить комментарий");
        }

        public async Task<ApiResponse> DeleteCommentAsync(Guid commentId)
        {
            return await SendRequestAsync(
                () => _httpClient.DeleteAsync($"api/backlog/comments/{commentId}"),
                "Не удалось удалить комментарий");
        }

        public async Task<ApiResponse<AttachmentResponse>> UploadAttachmentAsync(Guid backlogItemId, UploadAttachmentRequest request)
        {
            if (request.FileContent == null || request.FileContent.Length == 0)
            {
                await _errorHandling.TriggerError("Файл не выбран");
                return ApiResponse<AttachmentResponse>.Fail("Файл не выбран");
            }

            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(request.FileContent);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.MimeType ?? "application/octet-stream");
            content.Add(fileContent, "file", request.FileName);

            return await SendRequestAsync<AttachmentResponse>(
                () => _httpClient.PostAsync($"api/backlog/{backlogItemId}/attachments", content),
                "Не удалось загрузить файл");
        }

        public async Task<ApiResponse> DeleteAttachmentAsync(Guid attachmentId)
        {
            return await SendRequestAsync(
                () => _httpClient.DeleteAsync($"api/backlog/attachments/{attachmentId}"),
                "Не удалось удалить вложение");
        }

        public async Task<byte[]> DownloadAttachmentAsync(Guid attachmentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/backlog/attachments/{attachmentId}/download");
                if (!response.IsSuccessStatusCode)
                {
                    await _errorHandling.TriggerError($"Ошибка {response.StatusCode}: не удалось скачать файл");
                    return Array.Empty<byte>();
                }
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                await _errorHandling.TriggerError($"Ошибка скачивания: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        public async Task<ApiResponse<BlockerResponse>> AddBlockerAsync(Guid backlogItemId, AddBlockerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                await _errorHandling.TriggerError("Описание блокера обязательно");
                return ApiResponse<BlockerResponse>.Fail("Описание блокера обязательно");
            }

            return await SendRequestAsync<BlockerResponse>(
                () => _httpClient.PostAsJsonAsync($"api/backlog/{backlogItemId}/blockers", request),
                "Не удалось добавить блокер");
        }

        public async Task<ApiResponse> ResolveBlockerAsync(Guid blockerId, ResolveBlockerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ResolutionNote))
            {
                await _errorHandling.TriggerError("Укажите причину разрешения блокера");
                return ApiResponse.Fail("Укажите причину разрешения блокера");
            }

            return await SendRequestAsync(
                () => _httpClient.PatchAsJsonAsync($"api/backlog/blockers/{blockerId}/resolve", request),
                "Не удалось разрешить блокер");
        }

        public async Task<ApiResponse<BacklogItemDetailResponse>> GetBacklogItemDetailAsync(Guid id)
        {
            return await SendRequestAsync<BacklogItemDetailResponse>(
                () => _httpClient.GetAsync($"api/backlog/{id}/detail"),
                "Не удалось получить детали задачи");
        }
    }
}