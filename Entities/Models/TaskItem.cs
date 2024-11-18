using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models;

public class TaskItem
{
    [Column("TaskItemId")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Task title is a required field.")]
    [MaxLength(100, ErrorMessage = "Maximum length for the Title is 100 characters.")]
    public string? Title { get; set; }

    [MaxLength(500, ErrorMessage = "Maximum length for the Description is 500 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Priority is a required field.")]
    [Range(1, 5, ErrorMessage = "Priority must be between 1 and 5.")]
    public int Priority { get; set; }

    [Required(ErrorMessage = "Status is a required field.")]
    [MaxLength(20, ErrorMessage = "Maximum length for the Status is 20 characters.")]
    public string? Status { get; set; }

    [ForeignKey(nameof(Project))]
    public Guid ProjectId { get; set; }

    // **Navigational Property**: Links this TaskItem to its Project
    public Project? Project { get; set; }
}
