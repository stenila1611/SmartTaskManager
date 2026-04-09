using System;
using Microsoft.EntityFrameworkCore;
using SmartTaskManager.Models;
namespace SmartTaskManager.Data;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options){ }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<AILog> AILogs => Set<AILog>();

}
