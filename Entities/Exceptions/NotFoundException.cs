using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions;

public abstract class NotFoundException : Exception
{
    public string Type { get; set; }
    public string Detail { get; set; }
    public string Title { get; set; }
    public string Instance { get; set; }

    protected NotFoundException(string message):base(message)
       
    {
        Type = "notfound-exception"; // Default Type URI for not found errors
        Detail = "There was an unexpected error while fetching the record.";
        Title = "Custom Project Exception";
        Instance = message;
    }
}