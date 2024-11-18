using Contracts;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IProjectRepository> _projectRepository;
    private readonly Lazy<ITaskItemRepository> _taskItemRepository;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        _projectRepository = new Lazy<IProjectRepository>(() => new ProjectRepository(repositoryContext));
        _taskItemRepository = new Lazy<ITaskItemRepository>(() => new TaskItemRepository(repositoryContext));
    }

    public IProjectRepository Project => _projectRepository.Value;
    public ITaskItemRepository TaskItem => _taskItemRepository.Value;
    public async Task<IDbContextTransaction> BeginTransactionAsync() => await _repositoryContext.BeginTransactionAsync();


    public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
}