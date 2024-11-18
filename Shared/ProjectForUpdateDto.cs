using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared;

public record ProjectForUpdateDto(
    string Name,
    string? Description,
    DateTime StartDate,
    DateTime? EndDate,
    IEnumerable<TaskItemForUpdateDto> Tasks
);
