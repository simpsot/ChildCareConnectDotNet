using Microsoft.EntityFrameworkCore;
using ChildCareConnect.Data;
using ChildCareConnect.Models;

namespace ChildCareConnect.Services;

public class TaskService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public TaskService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<TaskItem>> GetTasksForUserAsync(string userId, string? statusFilter = null, string? priorityFilter = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Tasks
            .Include(t => t.Assignee)
            .Include(t => t.Creator)
            .Include(t => t.TaskTags)
                .ThenInclude(tt => tt.Tag)
            .Where(t => t.AssigneeId == userId);

        if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
        {
            query = query.Where(t => t.Status == statusFilter);
        }

        if (!string.IsNullOrEmpty(priorityFilter) && priorityFilter != "All")
        {
            query = query.Where(t => t.Priority == priorityFilter);
        }

        return await query
            .OrderBy(t => t.Status == "Completed" ? 1 : 0)
            .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
            .ThenByDescending(t => t.Priority == "Urgent" ? 4 : t.Priority == "High" ? 3 : t.Priority == "Normal" ? 2 : 1)
            .ToListAsync();
    }

    public async Task<List<TaskItem>> GetAllTasksAsync(string? statusFilter = null, string? priorityFilter = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Tasks
            .Include(t => t.Assignee)
            .Include(t => t.Creator)
            .Include(t => t.TaskTags)
                .ThenInclude(tt => tt.Tag)
            .AsQueryable();

        if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
        {
            query = query.Where(t => t.Status == statusFilter);
        }

        if (!string.IsNullOrEmpty(priorityFilter) && priorityFilter != "All")
        {
            query = query.Where(t => t.Priority == priorityFilter);
        }

        return await query
            .OrderBy(t => t.Status == "Completed" ? 1 : 0)
            .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
            .ThenByDescending(t => t.Priority == "Urgent" ? 4 : t.Priority == "High" ? 3 : t.Priority == "Normal" ? 2 : 1)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Tasks
            .Include(t => t.Assignee)
            .Include(t => t.Creator)
            .Include(t => t.TaskTags)
                .ThenInclude(tt => tt.Tag)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<int> GetPendingTaskCountAsync(string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Tasks
            .CountAsync(t => t.AssigneeId == userId && t.Status != "Completed");
    }

    public async Task<int> GetTotalPendingTaskCountAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Tasks
            .CountAsync(t => t.Status != "Completed");
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task, List<string>? tagIds = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        task.Id = Guid.NewGuid().ToString();
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        context.Tasks.Add(task);

        if (tagIds != null && tagIds.Any())
        {
            foreach (var tagId in tagIds)
            {
                context.TaskTags.Add(new TaskTag
                {
                    TaskId = task.Id,
                    TagId = tagId,
                    AddedAt = DateTime.UtcNow
                });
            }
        }

        await context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateTaskAsync(string id, TaskItem updates, List<string>? tagIds = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var task = await context.Tasks
            .Include(t => t.TaskTags)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        if (task == null) return null;

        task.Title = updates.Title;
        task.Description = updates.Description;
        task.Priority = updates.Priority;
        task.Status = updates.Status;
        task.DueDate = updates.DueDate;
        task.AssigneeId = updates.AssigneeId;
        task.UpdatedAt = DateTime.UtcNow;

        if (tagIds != null)
        {
            context.TaskTags.RemoveRange(task.TaskTags);
            foreach (var tagId in tagIds)
            {
                context.TaskTags.Add(new TaskTag
                {
                    TaskId = task.Id,
                    TagId = tagId,
                    AddedAt = DateTime.UtcNow
                });
            }
        }

        await context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> UpdateTaskStatusAsync(string id, string newStatus)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var task = await context.Tasks.FindAsync(id);
        if (task == null) return false;

        task.Status = newStatus;
        task.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTaskAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var task = await context.Tasks.FindAsync(id);
        if (task == null) return false;

        context.Tasks.Remove(task);
        await context.SaveChangesAsync();
        return true;
    }
}
