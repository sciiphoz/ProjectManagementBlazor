namespace ProjectManagementBlazor.DTO.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public DateTime? EmailConfirmedAt { get; set; }
    }

    public class UserBriefResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}