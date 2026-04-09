using System;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using SmartTaskManager.Data;
using SmartTaskManager.Models;

namespace SmartTaskManager.Services;

public class AIService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    public AIService(AppDbContext db, IConfiguration config, IHttpClientFactory factory)
    {
        _db = db;
        _config = config;
        _http = factory.CreateClient("groq");
    }

    public async Task<string> ProcessQueryAsync(Guid userId, string userQuery, string queryType)
    {
        var tasks = await _db.Tasks
                    .Where(t => t.UserId == userId)
                    .OrderBy(t => t.DueDate)
                    .Take(20)
                    .ToListAsync();

        var taskContext = string.Join("\n", tasks.Select(
            t => $"- {t.Title} | Status: {t.Status} | Priority: {t.Priority} | Due: {t.DueDate?.ToString("yyyy-MM-dd")??"none"} | Overdue: {(t.DueDate < DateTime.UtcNow && t.Status != "completed"?"YES":"no")}"

        ));

        var systemPrompt = queryType switch
        {
            "generate" => "Extract tasks from the user's text. Return ONLY a valid JSON array. Each item: {title, priority, dueDate}. No explanation.",
            _=>"You are a productivity assistant. Answer ONLY based on the task data provided. Be concise. No markdown."

        };
        var userMessage = queryType == "generate"?userQuery:$"USER TASKS:\n{taskContext}\n\nQUESTION: {userQuery}";
        var response = await CallGroqAsync(systemPrompt, userMessage);

        _db.AILogs.Add(new AILog
        {
            UserId = userId,
            QueryType = queryType,
            UserQuery = userQuery,
            Response = response
        });

        return response;
    
    }

    private async Task<string> CallGroqAsync(string system, string user)
    {
        try
        {
            var body = new
            {
                model = _config["GroqSettings:Model"],
                messages = new[]
                {
                    new {role = "system", content = system},
                    new {role = "user", content = user}
                },
                max_tokens = 1024,
                temperature = 0.3
            };
            
            var res = await _http.PostAsJsonAsync("openai/v1/chat/completions", body);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.
            ReadFromJsonAsync<JsonElement>();

            return json.GetProperty("choices")[0].
            GetProperty("message").GetProperty("content").GetString()??"No response.";
        }catch(Exception ex)
        {
            return "AI assistant is temporarily unavailable. Please try again shortly.";
        }
    }

}
