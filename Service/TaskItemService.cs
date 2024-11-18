using AutoMapper;
using Contracts;
using Entities.Models;
using Service.Contracts;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service;

internal sealed class TaskItemService : ITaskItemService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public TaskItemService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IEnumerable<TaskItemDto>> GetTaskItemsAsync(Guid projectId, bool trackChanges)
    {
        try
        {
            var project = await _repository.Project.GetProjectAsync(projectId, trackChanges);
            if (project is null)
            {
                _logger.LogError($"Project with ID {projectId} was not found.");
                return Enumerable.Empty<TaskItemDto>();
            }

            var taskItemsFromDb = await _repository.TaskItem.GetTaskItemsAsync(projectId, trackChanges);
            return _mapper.Map<IEnumerable<TaskItemDto>>(taskItemsFromDb);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong in {nameof(GetTaskItemsAsync)}: {ex.Message}");
            return Enumerable.Empty<TaskItemDto>();
        }
    }

    public async Task<TaskItemDto?> GetTaskItemAsync(Guid projectId, Guid id, bool trackChanges)
    {
        try
        {
            var project = await _repository.Project.GetProjectAsync(projectId, trackChanges);
            if (project is null)
            {
                _logger.LogError($"Project with ID {projectId} was not found.");
                return null;
            }

            var taskItemDb = await _repository.TaskItem.GetTaskItemAsync(projectId, id, trackChanges);
            if (taskItemDb is null)
            {
                _logger.LogError($"TaskItem with ID {id} was not found for Project ID {projectId}.");
                return null;
            }

            return _mapper.Map<TaskItemDto>(taskItemDb);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong in {nameof(GetTaskItemAsync)}: {ex.Message}");
            return null;
        }
    }

    public async Task<TaskItemDto?> CreateTaskItemForProjectAsync(Guid projectId, TaskItemForCreationDto taskItemForCreation, bool trackChanges)
    {
        try
        {
            var project = await _repository.Project.GetProjectAsync(projectId, trackChanges);
            if (project is null)
            {
                _logger.LogError($"Project with ID {projectId} was not found.");
                return null;
            }

            var taskItemEntity = _mapper.Map<TaskItem>(taskItemForCreation);
            _repository.TaskItem.CreateTaskItemForProject(projectId, taskItemEntity);
            await _repository.SaveAsync();

            return _mapper.Map<TaskItemDto>(taskItemEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong in {nameof(CreateTaskItemForProjectAsync)}: {ex.Message}");
            return null;
        }
    }

    public async Task DeleteTaskItemForProjectAsync(Guid projectId, Guid id, bool trackChanges)
    {
        try
        {
            var project = await _repository.Project.GetProjectAsync(projectId, trackChanges);
            if (project is null)
            {
                _logger.LogError($"Project with ID {projectId} was not found.");
                return;
            }

            var taskItemForProject = await _repository.TaskItem.GetTaskItemAsync(projectId, id, trackChanges);
            if (taskItemForProject is null)
            {
                _logger.LogError($"TaskItem with ID {id} was not found for Project ID {projectId}.");
                return;
            }

            _repository.TaskItem.DeleteTaskItem(taskItemForProject);
           await _repository.SaveAsync();
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong in {nameof(DeleteTaskItemForProjectAsync)}: {ex.Message}");
            return;
        }
    }

    public async Task UpdateTaskItemForProjectAsync(Guid projectId, Guid id, TaskItemForUpdateDto taskItemForUpdate,
        bool projectTrackChanges, bool taskItemTrackChanges)
    {
        try
        {
            var project = await _repository.Project.GetProjectAsync(projectId, projectTrackChanges);
            if (project is null)
            {
                _logger.LogError($"Project with ID {projectId} was not found.");
                return;
            }

            var taskItemEntity = await _repository.TaskItem.GetTaskItemAsync(projectId, id, taskItemTrackChanges);
            if (taskItemEntity is null)
            {
                _logger.LogError($"TaskItem with ID {id} was not found for Project ID {projectId}.");
                return;
            }

            _mapper.Map(taskItemForUpdate, taskItemEntity);
            _repository.SaveAsync();
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong in {nameof(UpdateTaskItemForProjectAsync)}: {ex.Message}");
            return;
        }
    }

    public async Task<(TaskItemForUpdateDto? taskItemToPatch, TaskItem? taskItemEntity)> GetTaskItemForPatchAsync
        (Guid projectId, Guid id, bool projectTrackChanges, bool taskItemTrackChanges)
    {
        try
        {
            var project = await _repository.Project.GetProjectAsync(projectId, projectTrackChanges);
            if (project is null)
            {
                _logger.LogError($"Project with ID {projectId} was not found.");
                return (null, null);
            }

            var taskItemEntity = await _repository.TaskItem.GetTaskItemAsync(projectId, id, taskItemTrackChanges);
            if (taskItemEntity is null)
            {
                _logger.LogError($"TaskItem with ID {id} was not found for Project ID {projectId}.");
                return (null, null);
            }

            var taskItemToPatch = _mapper.Map<TaskItemForUpdateDto>(taskItemEntity);
            return (taskItemToPatch, taskItemEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong in {nameof(GetTaskItemForPatchAsync)}: {ex.Message}");
            return (null, null);
        }
    }

    public async Task SaveChangesForPatchAsync(TaskItemForUpdateDto taskItemToPatch, TaskItem taskItemEntity)
    {
        try
        {
            _mapper.Map(taskItemToPatch, taskItemEntity);
            await _repository.SaveAsync();
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong in {nameof(SaveChangesForPatchAsync)}: {ex.Message}");
            return;
        }
    }
}