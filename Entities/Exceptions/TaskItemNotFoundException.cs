using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions;

public class TaskItemNotFoundException : NotFoundException
{
    public TaskItemNotFoundException(Guid taskItemId)
        : base($"TaskItem with id: {taskItemId} doesn't exist in the database.")
    {
    }
}