using System.ComponentModel.DataAnnotations;

namespace ProjectManagementBlazor.DTO.Requests
{
    public enum SubTaskStatus
    {
        ToDo = 0,
        InProgress = 1,
        Done = 2
    }

    public class CreateSubTaskRequest
    {
        [Required]
        public Guid BacklogItemId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Range(0, 999)]
        public decimal? EstimatedHours { get; set; }

        public Guid? AssigneeId { get; set; }
    }

    public class UpdateSubTaskRequest
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Range(0, 999)]
        public decimal? EstimatedHours { get; set; }

        public decimal? ActualHours { get; set; }

        public string? Status { get; set; }

        public Guid? AssigneeId { get; set; }

        public int? OrderInParent { get; set; }
    }

    public class ChangeSubTaskStatusRequest
    {
        [Required]
        public string NewStatus { get; set; } = string.Empty;
    }

    public class ReorderSubTasksRequest
    {
        [Required]
        public List<ReorderSubTaskItem> Items { get; set; } = new();
    }

    public class ReorderSubTaskItem
    {
        public Guid Id { get; set; }
        public int NewOrder { get; set; }
    }

    public class StartSubTaskRequest
    {
        [Required]
        public Guid SubTaskId { get; set; }
    }

    public class CompleteSubTaskRequest
    {
        [Required]
        public Guid SubTaskId { get; set; }

        public decimal? ActualHours { get; set; }

        public string? CompletionNote { get; set; }
    }
}