using System;

namespace SmartTaskManager.Models;

public class AILog
{
    public Guid AILogId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string QueryType { get; set; } = string.Empty;
    public string UserQuery { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
