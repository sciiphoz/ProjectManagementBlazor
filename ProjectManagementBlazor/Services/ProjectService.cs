using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;
using System.Net.Http.Json;

namespace ProjectManagementBlazor.Services
{
    public class ProjectService : BaseApiService, IProjectService
    {
        public ProjectService(HttpClient httpClient, IErrorHandlingService errorHandling)
            : base(httpClient, errorHandling)
        {
        }

        public async Task<ApiResponse<ProjectResponse>> CreateProjectAsync(CreateProjectRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                await _errorHandling.TriggerError("Название проекта обязательно для заполнения");
                return ApiResponse<ProjectResponse>.Fail("Название проекта обязательно для заполнения");
            }

            if (request.Name.Length > 100)
            {
                await _errorHandling.TriggerError("Название проекта не может превышать 100 символов");
                return ApiResponse<ProjectResponse>.Fail("Название проекта не может превышать 100 символов");
            }

            return await SendRequestAsync<ProjectResponse>(
                () => _httpClient.PostAsJsonAsync("api/projects", request),
                "Не удалось создать проект");
        }

        public async Task<ApiResponse<ProjectResponse>> GetProjectByIdAsync(Guid projectId)
        {
            return await SendRequestAsync<ProjectResponse>(
                () => _httpClient.GetAsync($"api/projects/{projectId}"),
                "Не удалось получить данные проекта");
        }

        public async Task<ApiResponse<PagedResult<ProjectResponse>>> GetUserProjectsAsync(PagedRequest request)
        {
            var queryString = $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
            if (!string.IsNullOrEmpty(request.SearchTerm))
                queryString += $"&searchTerm={Uri.EscapeDataString(request.SearchTerm)}";

            return await SendRequestAsync<PagedResult<ProjectResponse>>(
                () => _httpClient.GetAsync($"api/projects/my{queryString}"),
                "Не удалось получить список проектов");
        }

        public async Task<ApiResponse<ProjectResponse>> UpdateProjectAsync(Guid projectId, UpdateProjectRequest request)
        {
            if (request.Name != null && request.Name.Length > 100)
            {
                await _errorHandling.TriggerError("Название проекта не может превышать 100 символов");
                return ApiResponse<ProjectResponse>.Fail("Название проекта не может превышать 100 символов");
            }

            return await SendRequestAsync<ProjectResponse>(
                () => _httpClient.PutAsJsonAsync($"api/projects/{projectId}", request),
                "Не удалось обновить проект");
        }

        public async Task<ApiResponse> DeleteProjectAsync(Guid projectId)
        {
            return await SendRequestAsync(
                () => _httpClient.DeleteAsync($"api/projects/{projectId}"),
                "Не удалось удалить проект");
        }

        public async Task<ApiResponse> ArchiveProjectAsync(Guid projectId)
        {
            return await SendRequestAsync(
                () => _httpClient.PostAsync($"api/projects/{projectId}/archive", null),
                "Не удалось архивировать проект");
        }

        public async Task<ApiResponse> RestoreProjectAsync(Guid projectId)
        {
            return await SendRequestAsync(
                () => _httpClient.PostAsync($"api/projects/{projectId}/restore", null),
                "Не удалось восстановить проект");
        }

        public async Task<ApiResponse<List<ProjectMemberResponse>>> GetProjectMembersAsync(Guid projectId)
        {
            return await SendRequestAsync<List<ProjectMemberResponse>>(
                () => _httpClient.GetAsync($"api/projects/{projectId}/members"),
                "Не удалось получить список участников");
        }

        public async Task<ApiResponse<ProjectMemberResponse>> AddMemberAsync(Guid projectId, AddProjectMemberRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) && (request.UserId == null || request.UserId == Guid.Empty))
            {
                await _errorHandling.TriggerError("Не указан email или ID пользователя");
                return ApiResponse<ProjectMemberResponse>.Fail("Не указан email или ID пользователя");
            }

            if (string.IsNullOrWhiteSpace(request.Role))
            {
                await _errorHandling.TriggerError("Не указана роль");
                return ApiResponse<ProjectMemberResponse>.Fail("Не указана роль");
            }

            try
            {
                Console.WriteLine($"Добавление участника с email {request.Email} в проект {projectId} с ролью {request.Role}");

                var result = await SendRequestAsync<ProjectMemberResponse>(
                    () => _httpClient.PostAsJsonAsync($"api/projects/{projectId}/members", request),
                    "Не удалось добавить участника");

                Console.WriteLine($"Результат: Success={result.Success}, Data={result.Data != null}, Message={result.Message}");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение при добавлении участника: {ex.Message}");
                await _errorHandling.TriggerError($"Ошибка: {ex.Message}");
                return ApiResponse<ProjectMemberResponse>.Fail($"Ошибка: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateMemberRoleAsync(Guid projectId, UpdateMemberRoleRequest request)
        {
            if (request.UserId == Guid.Empty)
            {
                await _errorHandling.TriggerError("Не указан пользователь");
                return ApiResponse.Fail("Не указан пользователь");
            }

            if (string.IsNullOrWhiteSpace(request.NewRole))
            {
                await _errorHandling.TriggerError("Не указана новая роль");
                return ApiResponse.Fail("Не указана новая роль");
            }

            return await SendRequestAsync(
                () => _httpClient.PutAsJsonAsync($"api/projects/{projectId}/members/{request.UserId}/role", request),
                "Не удалось изменить роль участника");
        }

        public async Task<ApiResponse<ProjectInvitationStatus>> CheckInvitationAsync(string token)
        {
            return await SendRequestAsync<ProjectInvitationStatus>(
                () => _httpClient.GetAsync($"api/projects/invitations/check?token={Uri.EscapeDataString(token)}"),
                "Не удалось проверить приглашение");
        }

        public async Task<ApiResponse> AcceptInvitationAsync(string token)
        {
            return await SendRequestAsync(
                () => _httpClient.PostAsJsonAsync("api/projects/invitations/accept", new { token }),
                "Не удалось принять приглашение");
        }

        public async Task<ApiResponse> RemoveMemberAsync(Guid projectId, RemoveMemberRequest request)
        {
            if (request.UserId == Guid.Empty)
            {
                await _errorHandling.TriggerError("Не указан пользователь");
                return ApiResponse.Fail("Не указан пользователь");
            }

            return await SendRequestAsync(
                () => _httpClient.DeleteAsync($"api/projects/{projectId}/members/{request.UserId}"),
                "Не удалось удалить участника");
        }

        public async Task<ApiResponse<ProjectStatisticsResponse>> GetProjectStatisticsAsync(Guid projectId)
        {
            return await SendRequestAsync<ProjectStatisticsResponse>(
                () => _httpClient.GetAsync($"api/projects/{projectId}/statistics"),
                "Не удалось получить статистику проекта");
        }
    }
}