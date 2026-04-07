using System.Net.Http.Json;
using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;

namespace ProjectManagementBlazor.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly HttpClient _httpClient;

        public DashboardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<PersonalDashboardResponse>> GetPersonalDashboardAsync(DashboardRequest? request = null)
        {
            var queryString = "";
            if (request != null)
            {
                var paramsList = new List<string>();
                if (request.Date.HasValue)
                    paramsList.Add($"date={request.Date.Value:yyyy-MM-dd}");
                if (request.ProjectId.HasValue)
                    paramsList.Add($"projectId={request.ProjectId}");
                if (request.IncludeAllProjects)
                    paramsList.Add("includeAllProjects=true");

                if (paramsList.Any())
                    queryString = "?" + string.Join("&", paramsList);
            }

            var response = await _httpClient.GetAsync($"api/dashboard/my-day{queryString}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<PersonalDashboardResponse>>()
                   ?? ApiResponse<PersonalDashboardResponse>.Fail("Ошибка получения дашборда");
        }

        public async Task<ApiResponse<DailyScrumResponse>> GetDailyScrumViewAsync(Guid projectId, Guid? sprintId = null)
        {
            try
            {
                var url = $"api/dashboard/daily-scrum?projectId={projectId}";
                if (sprintId.HasValue)
                {
                    url += $"&sprintId={sprintId}";
                }

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadFromJsonAsync<ApiResponse<DailyScrumResponse>>();
                return content ?? ApiResponse<DailyScrumResponse>.Fail("Ошибка получения данных");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return ApiResponse<DailyScrumResponse>.Fail(ex.Message);
            }
        }

        public async Task<ApiResponse> UpdateDailyTasksAsync(UpdateDailyTasksRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/dashboard/my-day", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse>()
                   ?? ApiResponse.Fail("Ошибка обновления ежедневных задач");
        }
    }
}