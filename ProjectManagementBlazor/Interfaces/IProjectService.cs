using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;

namespace ProjectManagementBlazor.Interfaces
{
    public interface IProjectService
    {
        // Управление проектами
        Task<ApiResponse<ProjectResponse>> CreateProjectAsync(CreateProjectRequest request);
        Task<ApiResponse<ProjectResponse>> GetProjectByIdAsync(Guid projectId);
        Task<ApiResponse<PagedResult<ProjectResponse>>> GetUserProjectsAsync(PagedRequest request);
        Task<ApiResponse<ProjectResponse>> UpdateProjectAsync(Guid projectId, UpdateProjectRequest request);
        Task<ApiResponse> DeleteProjectAsync(Guid projectId);
        Task<ApiResponse> ArchiveProjectAsync(Guid projectId);
        Task<ApiResponse> RestoreProjectAsync(Guid projectId);

        // Управление участниками
        Task<ApiResponse<List<ProjectMemberResponse>>> GetProjectMembersAsync(Guid projectId);
        Task<ApiResponse<ProjectMemberResponse>> AddMemberAsync(Guid projectId, AddProjectMemberRequest request);
        Task<ApiResponse> UpdateMemberRoleAsync(Guid projectId, UpdateMemberRoleRequest request);
        Task<ApiResponse> RemoveMemberAsync(Guid projectId, RemoveMemberRequest request);
        Task<ApiResponse<ProjectInvitationStatus>> CheckInvitationAsync(string token);
        Task<ApiResponse> AcceptInvitationAsync(string token);

        // Статистика
        Task<ApiResponse<ProjectStatisticsResponse>> GetProjectStatisticsAsync(Guid projectId);
    }
}