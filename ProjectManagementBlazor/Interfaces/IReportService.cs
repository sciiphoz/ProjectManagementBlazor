using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;


namespace ProjectManagementBlazor.Interfaces
{
    public interface IReportService
    {
        Task<ApiResponse<SprintReportResponse>> GenerateSprintReportAsync(Guid sprintId);
        Task<ApiResponse<TeamPerformanceReportResponse>> GenerateTeamPerformanceReportAsync(GenerateReportRequest request);
        Task<ApiResponse<VelocityReportResponse>> GenerateVelocityReportAsync(Guid projectId, int lastSprintsCount = 5);
    }
}