using AutoMapper;
using Contracts;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IProjectService> _projectService;
    private readonly Lazy<ITaskItemService> _taskItemService;

    public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper)
    {
        _projectService = new Lazy<IProjectService>(() =>
            new ProjectService(repositoryManager, logger, mapper));
        _taskItemService = new Lazy<ITaskItemService>(() =>
            new TaskItemService(repositoryManager, logger, mapper));
    }

    public IProjectService ProjectService => _projectService.Value;
    public ITaskItemService TaskItemService => _taskItemService.Value;
}