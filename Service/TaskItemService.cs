using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
        var project = await _repository.Project.GetProjectAsync(projectId, trackChanges);
        if (project is null)
            throw new ProjectNotFoundException(projectId);

        var taskItemsFromDb = await _repository.TaskItem.GetTaskItemsAsync(projectId, trackChanges);
        var taskItemsDto = _mapper.Map<IEnumerable<TaskItemDto>>(taskItemsFromDb);
        return taskItemsDto;

    }

    public async Task<TaskItemDto?> GetTaskItemAsync(Guid projectId, Guid id, bool trackChanges)
    {
        var project = await _repository.Project.GetProjectAsync(projectId, trackChanges);
        if (project is null)
            throw new ProjectNotFoundException(projectId);

        var taskItemDb = await _repository.TaskItem.GetTaskItemAsync(projectId, id, trackChanges);
        if (taskItemDb is null)
            throw new TaskItemNotFoundException(id);

        var taskItem = _mapper.Map<TaskItemDto>(taskItemDb);
        return taskItem;
    }

    public async Task<TaskItemDto?> CreateTaskItemForProjectAsync(Guid projectId, TaskItemForCreationDto taskItemForCreation, bool trackChanges)
    {
        var project = await _repository.Project.GetProjectAsync(projectId, trackChanges);
        if (project is null)
            throw new ProjectNotFoundException(projectId);

        var taskItemEntity = _mapper.Map<TaskItem>(taskItemForCreation);

        _repository.TaskItem.CreateTaskItemForProject(projectId, taskItemEntity);
        await _repository.SaveAsync();

        var taskItemToReturn = _mapper.Map<TaskItemDto>(taskItemEntity);
        return taskItemToReturn;
    }

    public async Task DeleteTaskItemForProjectAsync(Guid projectId, Guid id, bool trackChanges)
    {
        var project = await _repository.Project.GetProjectAsync(projectId, trackChanges);
        if (project is null)
            throw new ProjectNotFoundException(projectId);

        var taskItemForProject = await _repository.TaskItem.GetTaskItemAsync(projectId, id, trackChanges);
        if (taskItemForProject is null)
            throw new TaskItemNotFoundException(id);

        _repository.TaskItem.DeleteTaskItem(taskItemForProject);
        await _repository.SaveAsync();
    }

    public async Task UpdateTaskItemForProjectAsync(Guid projectId, Guid id, TaskItemForUpdateDto taskItemForUpdate,
        bool projectTrackChanges, bool taskItemTrackChanges)
    {
        var project = await _repository.Project.GetProjectAsync(projectId, projectTrackChanges);
        if (project is null)
            throw new ProjectNotFoundException(projectId);

        var taskItemEntity = await _repository.TaskItem.GetTaskItemAsync(projectId, id, taskItemTrackChanges);
        if (taskItemEntity is null)
            throw new TaskItemNotFoundException(id);

        _mapper.Map(taskItemForUpdate, taskItemEntity);

        await _repository.SaveAsync();
    }

    public async Task<(TaskItemForUpdateDto? taskItemToPatch, TaskItem? taskItemEntity)> GetTaskItemForPatchAsync
        (Guid projectId, Guid id, bool projectTrackChanges, bool taskItemTrackChanges)
    {
        var project = await _repository.Project.GetProjectAsync(projectId, projectTrackChanges);
        if (project is null)
            throw new ProjectNotFoundException(projectId);

        var taskItemEntity = await _repository.TaskItem.GetTaskItemAsync(projectId, id, taskItemTrackChanges);
        if (taskItemEntity is null)
            throw new TaskItemNotFoundException(projectId);

        var taskItemToPatch = _mapper.Map<TaskItemForUpdateDto>(taskItemEntity);
        return (taskItemToPatch, taskItemEntity);

    }

    public async Task SaveChangesForPatchAsync(TaskItemForUpdateDto taskItemToPatch, TaskItem taskItemEntity)
    {
        _mapper.Map(taskItemToPatch, taskItemEntity);
        await _repository.SaveAsync();
    }
}