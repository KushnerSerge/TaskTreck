using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(bool trackChanges);
    Task<ProjectDto?> GetProjectAsync(Guid projectId, bool trackChanges);
    Task<ProjectDto?> CreateProjectAsync(ProjectForCreationDto project);
    Task<IEnumerable<ProjectDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
    Task<(IEnumerable<ProjectDto> projects, string ids)> CreateProjectCollectionAsync
        (IEnumerable<ProjectForCreationDto> projectCollection);
    Task DeleteProjectAsync(Guid projectId, bool trackChanges);
    Task UpdateProjectAsync(Guid projectid, ProjectForUpdateDto projectForUpdate, bool trackChanges);
}
