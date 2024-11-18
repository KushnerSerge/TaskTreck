using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace Repository;

internal sealed class ProjectRepository : RepositoryBase<Project>, IProjectRepository
{
    public ProjectRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Project>> GetAllProjectsAsync(bool trackChanges) =>
     await FindAll(trackChanges)
        .Include(p => p.Tasks)
        .OrderBy(c => c.Name)
        .ToListAsync();

    public async Task<Project?> GetProjectAsync(Guid projectId, bool trackChanges) =>
       await FindByCondition(c => c.Id.Equals(projectId), trackChanges)
    .SingleOrDefaultAsync();

    public void CreateProject(Project project) => Create(project);

    public async Task<IEnumerable<Project>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) =>
      await  FindByCondition(x => ids.Contains(x.Id), trackChanges)
    .ToListAsync();

    public void DeleteProject(Project project) => Delete(project);
}