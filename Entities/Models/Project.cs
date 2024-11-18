using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models;

public class Project
{
    [Column("ProjectId")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Project name is a required field.")]
    [MaxLength(50, ErrorMessage = "Maximum length for the Name is 50 characters.")]
    public string? Name { get; set; }

    [MaxLength(200, ErrorMessage = "Maximum length for the Description is 200 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Start date is a required field.")]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    // **Navigational Property**: One-to-Many Relationship with TaskItem
    public ICollection<TaskItem>? Tasks { get; set; }
}

