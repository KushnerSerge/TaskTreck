using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts;

public interface IRepositoryManager
{
    IProjectRepository Project { get; }
    ITaskItemRepository TaskItem { get; }
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task SaveAsync();
}
