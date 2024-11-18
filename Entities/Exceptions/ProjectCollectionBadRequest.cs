using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions;

public sealed class ProjectCollectionBadRequest : BadRequestException
{
    public ProjectCollectionBadRequest()
        : base("Project collection sent from a client is null.")
    {
    }
}
