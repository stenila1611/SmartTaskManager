using System;
namespace SmartTaskManager.Models;

public class TaskItem
{
    public Guid TaskItemId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "pending";     // pending | in-progress | completed
    public string Priority { get; set; } = "medium";    // low | medium | high
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
}
