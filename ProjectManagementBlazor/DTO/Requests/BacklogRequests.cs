using System.ComponentModel.DataAnnotations;

namespace ProjectManagementBlazor.DTO.Requests
{
    public enum BacklogItemType
    {
        UserStory = 0,
        Bug = 1,
        TechnicalTask = 2,
        Improvement = 3
    }

    public enum BacklogItemStatus
    {
        Backlog = 0,
        ToDo = 1,
        InProgress = 2,
        Review = 3,
        Done = 4
    }
    public class BacklogSearchRequest
    {
        public Guid ProjectId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SearchTerm { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public Guid? AssigneeId { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }
    public class CreateBacklogItemRequest
    {
        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public BacklogItemType Type { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string? Description { get; set; }

        [MaxLength(5000)]
        public string? AcceptanceCriteria { get; set; }

        public int? Priority { get; set; }

        [Range(0, 100)]
        public decimal? StoryPoints { get; set; }

        [Range(0, 999)]
        public decimal? EstimatedHours { get; set; }

        public Guid? AssigneeId { get; set; }
        public Guid CreatedById { get; set; }
    }

    public class UpdateBacklogItemRequest
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(5000)]
        public string? Description { get; set; }

        [MaxLength(5000)]
        public string? AcceptanceCriteria { get; set; }

        public BacklogItemType? Type { get; set; }
        public int? Priority { get; set; }
        public decimal? StoryPoints { get; set; }
        public decimal? EstimatedHours { get; set; }
        public BacklogItemStatus? Status { get; set; }
        public Guid? AssigneeId { get; set; }
        public Guid UserId { get; set; }
    }

    public class ChangeTaskStatusRequest
    {
        [Required]
        public BacklogItemStatus NewStatus { get; set; }

        public string? Comment { get; set; }
    }

    public class ReorderBacklogRequest
    {
        [Required]
        public List<ReorderItem> Items { get; set; } = new();
    }

    public class ReorderItem
    {
        public Guid Id { get; set; }
        public int NewOrder { get; set; }
    }

    public class AddCommentRequest
    {
        [Required]
        [MaxLength(10000)]
        public string Content { get; set; } = string.Empty;

        public List<Guid>? MentionedUserIds { get; set; }
    }

    public class UpdateCommentRequest
    {
        [Required]
        [MaxLength(10000)]
        public string Content { get; set; } = string.Empty;
    }

    public class UploadAttachmentRequest
    {
        [Required]
        public byte[] FileContent { get; set; } = Array.Empty<byte>();

        [Required]
        public string FileName { get; set; } = string.Empty;

        public string? MimeType { get; set; }
    }

    public class AddBlockerRequest
    {
        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Severity { get; set; } = "Medium";
    }

    public class ResolveBlockerRequest
    {
        [Required]
        [MaxLength(2000)]
        public string ResolutionNote { get; set; } = string.Empty;
    }
}