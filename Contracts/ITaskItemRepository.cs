using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts;

public interface ITaskItemRepository
{
    Task<IEnumerable<TaskItem>> GetTaskItemsAsync(Guid projectId, bool trackChanges);
    Task<TaskItem?> GetTaskItemAsync(Guid projectId, Guid id, bool trackChanges);
    void CreateTaskItemForProject(Guid projectId, TaskItem taskItem);
    void DeleteTaskItem(TaskItem taskItem);
}

