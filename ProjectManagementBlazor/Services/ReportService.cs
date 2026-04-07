
using ProjectManagementBlazor.Interfaces;
using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using System.Net.Http.Json;

namespace ProjectManagementBlazor.Services
{
    public class ReportService : IReportService
    {
        private readonly HttpClient _httpClient;

        public ReportService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<SprintReportResponse>> GenerateSprintReportAsync(Guid sprintId)
        {
            var response = await _httpClient.GetAsync($"api/reports/sprint/{sprintId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<SprintReportResponse>>()
                   ?? ApiResponse<SprintReportResponse>.Fail("Ошибка формирования отчета");
        }

        public async Task<ApiResponse<TeamPerformanceReportResponse>> GenerateTeamPerformanceReportAsync(GenerateReportRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reports/team-performance", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<TeamPerformanceReportResponse>>()
                   ?? ApiResponse<TeamPerformanceReportResponse>.Fail("Ошибка формирования отчета");
        }

        public async Task<ApiResponse<VelocityReportResponse>> GenerateVelocityReportAsync(Guid projectId, int lastSprintsCount = 5)
        {
            var response = await _httpClient.GetAsync($"api/reports/velocity/{projectId}?lastSprintsCount={lastSprintsCount}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<VelocityReportResponse>>()
                   ?? ApiResponse<VelocityReportResponse>.Fail("Ошибка формирования отчета");
        }

        public async Task<byte[]> ExportReportAsync(GenerateReportRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reports/export", request);
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}