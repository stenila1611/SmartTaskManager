using System;
using Microsoft.EntityFrameworkCore;
using SmartTaskManager.Data;
using SmartTaskManager.DTOs;
using SmartTaskManager.Models;

namespace SmartTaskManager.Services;

public class TaskService
{
    private readonly AppDbContext _db;
    public TaskService(AppDbContext db)=>_db = db;

    public async Task<List<TaskItem>> GetTasksAsync(Guid userId, string? status, string? priority, bool? isOverdue)
    {
        var query = _db.Tasks.Where(t => t.UserId == userId);
        if(!string.IsNullOrEmpty(status))
            query = query.Where(t => t.Status == status);
        if(!string.IsNullOrEmpty(priority))
            query = query.Where(t => t.Priority == priority);
        if(isOverdue == true)
            query = query.Where(t => t.DueDate < DateTime.UtcNow &&
            t.Status != "completed");
        
        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    public async Task<TaskItem> CreateAsync(Guid userId, CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority ?? "medium",
            Status = "pending",
            DueDate = dto.DueDate
        };
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateAsync(Guid userId, Guid taskId, CreateTaskDto dto)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.TaskItemId == taskId
        && t.UserId == userId);
        if(task == null) return null;

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Priority = dto.Priority?? task.Priority;
        task.Status = dto.Status ?? task.Status;
        task.DueDate = dto.DueDate;
        await _db.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid taskId)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t=> t.TaskItemId == taskId 
        && t.UserId == userId);
        if(task == null) return false;
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return true;
    }
}
