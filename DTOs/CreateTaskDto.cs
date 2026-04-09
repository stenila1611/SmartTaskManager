using System;

namespace SmartTaskManager.DTOs;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = "medium";
    public string Status { get; set; } = "pending";
    public DateTime? DueDate { get; set; } 


}
