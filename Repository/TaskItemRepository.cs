using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository;

internal sealed class TaskItemRepository : RepositoryBase<TaskItem>, ITaskItemRepository
{
    public TaskItemRepository(RepositoryContext repositoryContext)
    : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<TaskItem>> GetTaskItemsAsync(Guid projectId, bool trackChanges) =>
       await FindByCondition(e => e.ProjectId.Equals(projectId), trackChanges)
        .OrderBy(e => e.Status)
        .ToListAsync();

    public async Task<TaskItem?> GetTaskItemAsync(Guid projectId, Guid id, bool trackChanges) =>
       await FindByCondition(e => e.ProjectId.Equals(projectId) && e.Id.Equals(id), trackChanges)
        .SingleOrDefaultAsync();

    public void CreateTaskItemForProject(Guid projectId, TaskItem taskItem)
    {
        taskItem.ProjectId = projectId;
        Create(taskItem);
    }

    public void DeleteTaskItem(TaskItem taskItem) => Delete(taskItem);
}
