using Entities.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts;

public interface ITaskItemService
{
    Task<IEnumerable<TaskItemDto>> GetTaskItemsAsync(Guid projectId, bool trackChanges);
    Task<TaskItemDto?> GetTaskItemAsync(Guid projectId, Guid id, bool trackChanges);
    Task<TaskItemDto?> CreateTaskItemForProjectAsync(Guid projectId, TaskItemForCreationDto taskItemForCreation, bool trackChanges);
    Task DeleteTaskItemForProjectAsync(Guid projectId, Guid id, bool trackChanges);
    Task UpdateTaskItemForProjectAsync(Guid projectId, Guid id,
        TaskItemForUpdateDto taskItemForUpdate, bool projectTrackChanges, bool taskItemTrackChanges);
    Task<(TaskItemForUpdateDto? taskItemToPatch, TaskItem? taskItemEntity)> GetTaskItemForPatchAsync(
        Guid projectId, Guid id, bool projectTrackChanges, bool taskItemTrackChanges);
    Task SaveChangesForPatchAsync(TaskItemForUpdateDto taskItemToPatch, TaskItem taskItemEntity);
}
