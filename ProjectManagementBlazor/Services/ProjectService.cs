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
            if (request.UserId == Guid.Empty)
            {
                await _errorHandling.TriggerError("Не указан пользователь");
                return ApiResponse<ProjectMemberResponse>.Fail("Не указан пользователь");
            }

            if (string.IsNullOrWhiteSpace(request.Role))
            {
                await _errorHandling.TriggerError("Не указана роль");
                return ApiResponse<ProjectMemberResponse>.Fail("Не указана роль");
            }

            return await SendRequestAsync<ProjectMemberResponse>(
                () => _httpClient.PostAsJsonAsync($"api/projects/{projectId}/members", request),
                "Не удалось добавить участника");
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