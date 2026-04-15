using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;
using System.Net.Http.Json;

namespace ProjectManagementBlazor.Services
{
    public class ReportService : BaseApiService, IReportService
    {
        public ReportService(HttpClient httpClient, IErrorHandlingService errorHandling)
            : base(httpClient, errorHandling)
        {
        }

        public async Task<ApiResponse<SprintReportResponse>> GenerateSprintReportAsync(Guid sprintId)
        {
            return await SendRequestAsync<SprintReportResponse>(
                () => _httpClient.GetAsync($"api/reports/sprint/{sprintId}"),
                "Не удалось сформировать отчёт по спринту");
        }

        public async Task<ApiResponse<TeamPerformanceReportResponse>> GenerateTeamPerformanceReportAsync(GenerateReportRequest request)
        {
            if (request.StartDate > request.EndDate)
            {
                await _errorHandling.TriggerError("Дата начала не может быть позже даты окончания");
                return ApiResponse<TeamPerformanceReportResponse>.Fail("Дата начала не может быть позже даты окончания");
            }

            return await SendRequestAsync<TeamPerformanceReportResponse>(
                () => _httpClient.PostAsJsonAsync("api/reports/team-performance", request),
                "Не удалось сформировать отчёт по производительности");
        }

        public async Task<ApiResponse<VelocityReportResponse>> GenerateVelocityReportAsync(Guid projectId, int lastSprintsCount = 5)
        {
            if (lastSprintsCount <= 0)
                lastSprintsCount = 5;

            return await SendRequestAsync<VelocityReportResponse>(
                () => _httpClient.GetAsync($"api/reports/velocity/{projectId}?lastSprintsCount={lastSprintsCount}"),
                "Не удалось сформировать Velocity отчёт");
        }

        public async Task<byte[]> ExportReportAsync(GenerateReportRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/reports/export", request);
                if (!response.IsSuccessStatusCode)
                {
                    await _errorHandling.TriggerError($"Ошибка {response.StatusCode}: не удалось экспортировать отчёт");
                    return Array.Empty<byte>();
                }
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                await _errorHandling.TriggerError($"Ошибка экспорта: {ex.Message}");
                return Array.Empty<byte>();
            }
        }
    }
}