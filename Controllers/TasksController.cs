using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManager.DTOs;
using SmartTaskManager.Services;

namespace SmartTaskManager.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TasksController:ControllerBase
{
    private readonly TaskService _tasks;
    public TasksController(TaskService tasks) => _tasks = tasks;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status,[FromQuery]string? priority, [FromQuery] bool? isOverdue)
    =>Ok(await _tasks.GetTasksAsync(UserId, status, priority, isOverdue));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    =>Ok(await _tasks.CreateAsync(UserId, dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateTaskDto dto)
    {
        var result = await _tasks.UpdateAsync(UserId, id, dto);
        return result == null? NotFound(): Ok(result);
    
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _tasks.DeleteAsync(UserId, id);
        return result? NoContent(): NotFound();
    }

    [HttpGet("Overdue")]
    public async Task<IActionResult> Overdue()=>Ok(await _tasks.GetTasksAsync(UserId, null, null, true));
}
