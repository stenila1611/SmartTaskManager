using System;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManager.DTOs;
using SmartTaskManager.Services;

namespace SmartTaskManager.Controllers;

[Authorize]
[ApiController]
[Route("api/ai")]
public class AIController:ControllerBase
{
    private readonly AIService _ai;
    public AIController(AIService ai)=> _ai = ai;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("query")]
    public async Task<IActionResult> Query([FromBody] AIQueryDto dto)
   =>Ok(new {answer = await _ai.ProcessQueryAsync(UserId, dto.Query, "query")});

    [HttpPost("summarize")]
    public async Task<IActionResult> Summarize()
    => Ok(new {summary = await _ai.ProcessQueryAsync(
        UserId, "Summarize my task workload. Give counts by status and priority. Mention overdue tasks.","summarize")});

    [HttpPost("suggestions")]
    public async  Task<IActionResult> Suggestions()
    =>Ok(new { suggestions = await _ai.ProcessQueryAsync(UserId, "Which 3 tasks should I focus on right now and why? Consider priority, due dates, and overdue status.", "suggestions")});

    [HttpPost("generate-tasks")]
    public async Task<IActionResult> GenerateTasks([FromBody] AIQueryDto dto)
    {
        var raw = await _ai.ProcessQueryAsync(UserId,
        dto.Query, "generate");
        try
        {
            var parsed = JsonSerializer.Deserialize<JsonElement>(raw.Replace("```json","").Replace("```","").Trim());
            return Ok(new {generatedTasks = parsed});
        }
        catch
        {
            return Ok(new {generatedTasks = raw});
        }
    }


}
