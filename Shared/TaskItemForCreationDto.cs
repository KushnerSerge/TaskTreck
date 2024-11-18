using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared;

public record TaskItemForCreationDto(
    string Title,
    string? Description,
    int Priority,
    string Status
);