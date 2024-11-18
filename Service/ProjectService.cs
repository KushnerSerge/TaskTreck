

using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.VisualBasic;
using Service.Contracts;
using Shared;
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
        try
        {
            var projects = await _repository.Project.GetAllProjectsAsync(trackChanges);

            var projectsDto = _mapper.Map<IEnumerable<ProjectDto>>(projects);

            return projectsDto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong in the {nameof(GetAllProjectsAsync)} service method {ex}");
            throw;
        }
    }

    public async Task<ProjectDto?> GetProjectAsync(Guid id, bool trackChanges)
    {
        try
        {
            var project = await _repository.Project.GetProjectAsync(id, trackChanges);
            if (project is null)
            {
                _logger.LogWarn($"Project with ID {id} not found in {nameof(GetProjectAsync)}.");
                return null;
            }

            return _mapper.Map<ProjectDto>(project);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong in the {nameof(GetProjectAsync)} service method {ex}");
            return null;
        }
    }

    public async Task<ProjectDto?> CreateProjectAsync(ProjectForCreationDto project)
    {
        try
        {
            var projectEntity = _mapper.Map<Project>(project);
            _repository.Project.CreateProject(projectEntity);
            await _repository.SaveAsync();

            return _mapper.Map<ProjectDto>(projectEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong in the {nameof(CreateProjectAsync)} service method {ex}");
            throw;
        }
    }

    public async Task<IEnumerable<ProjectDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
    {
        try
        {
            if (ids is null || !ids.Any())
            {
                _logger.LogWarn($"Invalid IDs passed to {nameof(GetByIdsAsync)}.");
                return Enumerable.Empty<ProjectDto>();
            }

            var projectEntities = await _repository.Project.GetByIdsAsync(ids, trackChanges);
            if (ids.Count() != projectEntities.Count())
            {
                _logger.LogWarn($"Mismatch between requested and retrieved IDs in {nameof(GetByIdsAsync)}.");
                return Enumerable.Empty<ProjectDto>();
            }

            return _mapper.Map<IEnumerable<ProjectDto>>(projectEntities);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong in the {nameof(GetByIdsAsync)} service method {ex}");
            return Enumerable.Empty<ProjectDto>();
        }
    }

    public async Task<(IEnumerable<ProjectDto> projects, string ids)> CreateProjectCollectionAsync(IEnumerable<ProjectForCreationDto> projectCollection)
    {
        using var transaction = await _repository.BeginTransactionAsync(); // Start transaction
        try
        {
            if (projectCollection is null || !projectCollection.Any())
            {
                _logger.LogWarn($"Invalid project collection passed to {nameof(CreateProjectCollectionAsync)}.");
                return (Enumerable.Empty<ProjectDto>(), string.Empty);
            }

            var projectEntities = _mapper.Map<IEnumerable<Project>>(projectCollection);
            foreach (var project in projectEntities)
            {
                // Simulate a failure for demonstration purposes
                if (project.Name.Contains("Fail"))
                {
                    throw new Exception("Simulated failure: Invalid project name.");
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
        catch (Exception ex)
        {
            // Rollback transaction on failure
            await transaction.RollbackAsync();
            _logger.LogError($"Transaction failed in {nameof(CreateProjectCollectionAsync)}: {ex.Message}");
            throw; // Re-throw exception for controller to handle
        }
    }

    public async Task DeleteProjectAsync(Guid projectId, bool trackChanges)
    {
        try
        {
            var project = await _repository.Project.GetProjectAsync(projectId, trackChanges);
            if (project is null)
            {
                _logger.LogWarn($"Project with ID {projectId} not found in {nameof(DeleteProjectAsync)}.");
                return;
            }

            _repository.Project.DeleteProject(project);
            await _repository.SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong in the {nameof(DeleteProjectAsync)} service method {ex}");
            throw;
        }
    }

    public async Task UpdateProjectAsync(Guid projectId, ProjectForUpdateDto projectForUpdate, bool trackChanges)
    {
        try
        {
            var projectEntity = await _repository.Project.GetProjectAsync(projectId, trackChanges);
            if (projectEntity is null)
            {
                _logger.LogWarn($"Project with ID {projectId} not found in {nameof(UpdateProjectAsync)}.");
                return;
            }

            _mapper.Map(projectForUpdate, projectEntity);
            await _repository.SaveAsync();
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong in the {nameof(UpdateProjectAsync)} service method {ex}");
            throw;
        }
    }
}