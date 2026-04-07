using System.Net.Http.Json;
using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;

namespace ProjectManagementBlazor.Services
{
    public class ProjectService : IProjectService
    {
        private readonly HttpClient _httpClient;

        public ProjectService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<ProjectResponse>> CreateProjectAsync(CreateProjectRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/projects", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<ProjectResponse>>()
                   ?? ApiResponse<ProjectResponse>.Fail("Ошибка создания проекта");
        }

        public async Task<ApiResponse<ProjectResponse>> GetProjectByIdAsync(Guid projectId)
        {
            var response = await _httpClient.GetAsync($"api/projects/{projectId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<ProjectResponse>>()
                   ?? ApiResponse<ProjectResponse>.Fail("Проект не найден");
        }

        public async Task<ApiResponse<PagedResult<ProjectResponse>>> GetUserProjectsAsync(PagedRequest request)
        {
            var queryString = $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
            if (!string.IsNullOrEmpty(request.SearchTerm))
                queryString += $"&searchTerm={request.SearchTerm}";

            var response = await _httpClient.GetAsync($"api/projects/my{queryString}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<ProjectResponse>>>()
                   ?? ApiResponse<PagedResult<ProjectResponse>>.Fail("Ошибка получения проектов");
        }

        public async Task<ApiResponse<ProjectResponse>> UpdateProjectAsync(Guid projectId, UpdateProjectRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/projects/{projectId}", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<ProjectResponse>>()
                   ?? ApiResponse<ProjectResponse>.Fail("Ошибка обновления проекта");
        }

        public async Task<ApiResponse> DeleteProjectAsync(Guid projectId)
        {
            var response = await _httpClient.DeleteAsync($"api/projects/{projectId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка удаления проекта");
        }

        public async Task<ApiResponse> ArchiveProjectAsync(Guid projectId)
        {
            var response = await _httpClient.PostAsync($"api/projects/{projectId}/archive", null);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка архивации проекта");
        }

        public async Task<ApiResponse> RestoreProjectAsync(Guid projectId)
        {
            var response = await _httpClient.PostAsync($"api/projects/{projectId}/restore", null);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка восстановления проекта");
        }

        public async Task<ApiResponse<List<ProjectMemberResponse>>> GetProjectMembersAsync(Guid projectId)
        {
            var response = await _httpClient.GetAsync($"api/projects/{projectId}/members");
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<ProjectMemberResponse>>>()
                   ?? ApiResponse<List<ProjectMemberResponse>>.Fail("Ошибка получения участников");
        }

        public async Task<ApiResponse<ProjectMemberResponse>> AddMemberAsync(Guid projectId, AddProjectMemberRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/projects/{projectId}/members", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<ProjectMemberResponse>>()
                   ?? ApiResponse<ProjectMemberResponse>.Fail("Ошибка добавления участника");
        }

        public async Task<ApiResponse> UpdateMemberRoleAsync(Guid projectId, UpdateMemberRoleRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/projects/{projectId}/members/{request.UserId}/role", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка обновления роли");
        }

        public async Task<ApiResponse> RemoveMemberAsync(Guid projectId, RemoveMemberRequest request)
        {
            var response = await _httpClient.DeleteAsync($"api/projects/{projectId}/members/{request.UserId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка удаления участника");
        }

        public async Task<ApiResponse<ProjectStatisticsResponse>> GetProjectStatisticsAsync(Guid projectId)
        {
            var response = await _httpClient.GetAsync($"api/projects/{projectId}/statistics");
            return await response.Content.ReadFromJsonAsync<ApiResponse<ProjectStatisticsResponse>>()
                   ?? ApiResponse<ProjectStatisticsResponse>.Fail("Ошибка получения статистики");
        }
    }
}