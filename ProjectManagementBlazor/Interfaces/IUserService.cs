using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;


namespace ProjectManagementBlazor.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserResponse>> GetUserByIdAsync(Guid userId);
        Task<ApiResponse<UserResponse>> UpdateProfileAsync(UpdateProfileRequest request);
        Task<ApiResponse> ChangePasswordAsync(ChangePasswordRequest request);
        Task<ApiResponse<PagedResult<UserResponse>>> GetAllUsersAsync(PagedRequest request);
        Task<ApiResponse<List<UserBriefResponse>>> GetProjectUsersAsync(Guid projectId);
        Task<ApiResponse> ForgotPasswordAsync(string email);
        Task<ApiResponse> VerifyResetCodeAsync(string email, string code);
        Task<ApiResponse> ResetPasswordWithCodeAsync(string email, string code, string newPassword, string confirmPassword);
    }
}