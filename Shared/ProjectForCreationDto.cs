using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared;

public record ProjectForCreationDto(
    string Name,
    string? Description,
    DateTime StartDate,
    DateTime? EndDate,
    IEnumerable<TaskItemForCreationDto> Tasks
);