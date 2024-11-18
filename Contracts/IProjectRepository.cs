using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllProjectsAsync(bool trackChanges);
    Task<Project?> GetProjectAsync(Guid projectId, bool trackChanges);
    void CreateProject(Project project);
    Task<IEnumerable<Project>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
    void DeleteProject(Project company);
}