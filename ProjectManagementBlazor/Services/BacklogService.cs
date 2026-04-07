using System.Net.Http.Json;
using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;

namespace ProjectManagementBlazor.Services
{
    public class BacklogService : IBacklogService
    {
        private readonly HttpClient _httpClient;

        public BacklogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<BacklogItemResponse>> CreateBacklogItemAsync(CreateBacklogItemRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/backlog", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<BacklogItemResponse>>()
                   ?? ApiResponse<BacklogItemResponse>.Fail("Ошибка создания задачи");
        }

        public async Task<ApiResponse<BacklogItemResponse>> GetBacklogItemByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/backlog/{id}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<BacklogItemResponse>>()
                   ?? ApiResponse<BacklogItemResponse>.Fail("Задача не найдена");
        }

        public async Task<ApiResponse<PagedResult<BacklogItemResponse>>> GetProjectBacklogAsync(Guid projectId, PagedRequest request)
        {
            var queryString = $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
            if (!string.IsNullOrEmpty(request.SearchTerm))
                queryString += $"&searchTerm={request.SearchTerm}";

            var response = await _httpClient.GetAsync($"api/backlog/project/{projectId}{queryString}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<BacklogItemResponse>>>()
                   ?? ApiResponse<PagedResult<BacklogItemResponse>>.Fail("Ошибка получения бэклога");
        }

        public async Task<ApiResponse<BacklogItemResponse>> UpdateBacklogItemAsync(Guid id, UpdateBacklogItemRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/backlog/{id}", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<BacklogItemResponse>>()
                   ?? ApiResponse<BacklogItemResponse>.Fail("Ошибка обновления задачи");
        }

        public async Task<ApiResponse> DeleteBacklogItemAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/backlog/{id}");
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка удаления задачи");
        }

        public async Task<ApiResponse<BacklogItemResponse>> ChangeStatusAsync(Guid id, ChangeTaskStatusRequest request)
        {
            var response = await _httpClient.PatchAsJsonAsync($"api/backlog/{id}/status", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<BacklogItemResponse>>()
                   ?? ApiResponse<BacklogItemResponse>.Fail("Ошибка изменения статуса");
        }

        public async Task<ApiResponse> ReorderBacklogAsync(ReorderBacklogRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/backlog/reorder", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка изменения порядка");
        }

        public async Task<ApiResponse<CommentResponse>> AddCommentAsync(Guid backlogItemId, AddCommentRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/backlog/{backlogItemId}/comments", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<CommentResponse>>()
                   ?? ApiResponse<CommentResponse>.Fail("Ошибка добавления комментария");
        }

        public async Task<ApiResponse<CommentResponse>> UpdateCommentAsync(Guid commentId, UpdateCommentRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/backlog/comments/{commentId}", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<CommentResponse>>()
                   ?? ApiResponse<CommentResponse>.Fail("Ошибка обновления комментария");
        }

        public async Task<ApiResponse> DeleteCommentAsync(Guid commentId)
        {
            var response = await _httpClient.DeleteAsync($"api/backlog/comments/{commentId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка удаления комментария");
        }

        public async Task<ApiResponse<AttachmentResponse>> UploadAttachmentAsync(Guid backlogItemId, UploadAttachmentRequest request)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(request.FileContent);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.MimeType ?? "application/octet-stream");
            content.Add(fileContent, "file", request.FileName);

            var response = await _httpClient.PostAsync($"api/backlog/{backlogItemId}/attachments", content);
            return await response.Content.ReadFromJsonAsync<ApiResponse<AttachmentResponse>>()
                   ?? ApiResponse<AttachmentResponse>.Fail("Ошибка загрузки файла");
        }

        public async Task<ApiResponse> DeleteAttachmentAsync(Guid attachmentId)
        {
            var response = await _httpClient.DeleteAsync($"api/backlog/attachments/{attachmentId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка удаления вложения");
        }

        public async Task<byte[]> DownloadAttachmentAsync(Guid attachmentId)
        {
            var response = await _httpClient.GetAsync($"api/backlog/attachments/{attachmentId}/download");
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<ApiResponse<BlockerResponse>> AddBlockerAsync(Guid backlogItemId, AddBlockerRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/backlog/{backlogItemId}/blockers", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<BlockerResponse>>()
                   ?? ApiResponse<BlockerResponse>.Fail("Ошибка добавления блокера");
        }

        public async Task<ApiResponse> ResolveBlockerAsync(Guid blockerId, ResolveBlockerRequest request)
        {
            var response = await _httpClient.PatchAsJsonAsync($"api/backlog/blockers/{blockerId}/resolve", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка разрешения блокера");
        }

        public async Task<ApiResponse<BacklogItemDetailResponse>> GetBacklogItemDetailAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/backlog/{id}/detail");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Ошибка API: {response.StatusCode}, Content: {errorContent}");
                    return ApiResponse<BacklogItemDetailResponse>.Fail($"Ошибка API: {response.StatusCode}");
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<BacklogItemDetailResponse>>();
                return result ?? ApiResponse<BacklogItemDetailResponse>.Fail("Ошибка получения деталей задачи");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение: {ex.Message}");
                return ApiResponse<BacklogItemDetailResponse>.Fail($"Ошибка: {ex.Message}");
            }
        }
    }
}