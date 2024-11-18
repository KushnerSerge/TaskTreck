

using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.VisualBasic;
using Service.Contracts;
using Shared;
using System.ComponentModel.Design;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Service;

internal sealed class ProjectService : IProjectService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public ProjectService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(bool trackChanges)
    {
        var projects = await _repository.Project.GetAllProjectsAsync(trackChanges);

        var projectsDto = _mapper.Map<IEnumerable<ProjectDto>>(projects);

        return projectsDto;
    }

    public async Task<ProjectDto?> GetProjectAsync(Guid id, bool trackChanges)
    {
        var project = await _repository.Project.GetProjectAsync(id, trackChanges);
        if (project is null)
            throw new ProjectNotFoundException(id);

        var projectDto = _mapper.Map<ProjectDto>(project);
        return projectDto;
    }

    public async Task<ProjectDto?> CreateProjectAsync(ProjectForCreationDto project)
    {
        var projectEntity = _mapper.Map<Project>(project);

        _repository.Project.CreateProject(projectEntity);
        await _repository.SaveAsync();

        var projectToReturn = _mapper.Map<ProjectDto>(projectEntity);
        return projectToReturn;
    }

    public async Task<IEnumerable<ProjectDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
    {
        if (ids is null)
            throw new IdParametersBadRequestException();

        var projectEntities = await _repository.Project.GetByIdsAsync(ids, trackChanges);
        if (ids.Count() != projectEntities.Count())
            throw new CollectionByIdsBadRequestException();

        var projectsToReturn = _mapper.Map<IEnumerable<ProjectDto>>(projectEntities);
        return projectsToReturn;
    }

    public async Task<(IEnumerable<ProjectDto> projects, string ids)> CreateProjectCollectionAsync(IEnumerable<ProjectForCreationDto> projectCollection)
    {
        if (projectCollection is null || !projectCollection.Any())
        {
            // Throw custom exception for bad request
            throw new ProjectCollectionBadRequest();
        }

        using var transaction = await _repository.BeginTransactionAsync(); // Start transaction
        try
        {
            var projectEntities = _mapper.Map<IEnumerable<Project>>(projectCollection);

            foreach (var project in projectEntities)
            {
                // Simulate failure for demonstration purposes
                if (project.Name.Contains("Fail"))
                {
                    throw new ProjectCollectionBadRequest();
                }

                _repository.Project.CreateProject(project);
            }

            await _repository.SaveAsync(); // Save all projects to the database

            // Commit the transaction after successful save
            await transaction.CommitAsync();

            var projectCollectionToReturn = _mapper.Map<IEnumerable<ProjectDto>>(projectEntities);
            var ids = string.Join(",", projectCollectionToReturn.Select(c => c.Id));

            return (projects: projectCollectionToReturn, ids: ids);
        }
        catch (ProjectCollectionBadRequest ex)
        {
            // Rollback transaction for custom exceptions
            await transaction.RollbackAsync();
            _logger.LogError($"Validation error in {nameof(CreateProjectCollectionAsync)}: {ex.Message}");
            throw; // Re-throw to be handled by the controller or middleware
        }
        catch (Exception ex)
        {
            // Rollback transaction for other exceptions
            await transaction.RollbackAsync();
            _logger.LogError($"Transaction failed in {nameof(CreateProjectCollectionAsync)}: {ex.Message}");
            throw; // Re-throw to be handled by the controller or middleware
        }
    }


    public async Task DeleteProjectAsync(Guid projectId, bool trackChanges)
    {
        var project = await _repository.Project.GetProjectAsync(projectId, trackChanges);
        if (project is null)
            throw new ProjectNotFoundException(projectId);

        _repository.Project.DeleteProject(project);
        await _repository.SaveAsync();
    }

    public async Task UpdateProjectAsync(Guid projectId, ProjectForUpdateDto projectForUpdate, bool trackChanges)
    {
        var projectEntity = await _repository.Project.GetProjectAsync(projectId, trackChanges);
        if (projectEntity is null)
            throw new ProjectNotFoundException(projectId);

        _mapper.Map(projectForUpdate, projectEntity);
        await _repository.SaveAsync();
        return;
    }
}