using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;


namespace ProjectManagementBlazor.Interfaces
{
    public interface IRetrospectiveService
    {
        Task<ApiResponse<RetrospectiveBoardResponse>> GetRetrospectiveBoardAsync(Guid sprintId);
        Task<ApiResponse<RetrospectiveItemResponse>> AddRetrospectiveItemAsync(Guid sprintId, AddRetrospectiveItemRequest request);
        Task<ApiResponse> VoteRetrospectiveItemAsync(Guid itemId);
        Task<ApiResponse> RemoveVoteAsync(Guid itemId);
        Task<ApiResponse> DeleteRetrospectiveItemAsync(Guid itemId);
    }
}