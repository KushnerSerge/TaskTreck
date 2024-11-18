using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Configuration;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        // Relationships
        builder.HasOne(t => t.Project)
               .WithMany(p => p.Tasks)
               .HasForeignKey(t => t.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasData
        (
            // Tasks for "Website Redesign" Project
            new TaskItem
            {
                Id = new Guid("1ac6734e-0f7a-4a96-9a35-2d854b69c18f"),
                Title = "Create Wireframes",
                Description = "Design wireframes for all main pages.",
                Priority = 3,
                Status = "In Progress",
                ProjectId = new Guid("5f93e88d-8c58-4633-bef7-9206b2f7c34d")
            },
            new TaskItem
            {
                Id = new Guid("4ad8b845-292c-42ec-9332-61e4a743d8a9"),
                Title = "Develop Frontend",
                Description = "Implement frontend components using Angular.",
                Priority = 4,
                Status = "Not Started",
                ProjectId = new Guid("5f93e88d-8c58-4633-bef7-9206b2f7c34d")
            },

            // Tasks for "Mobile App Development" Project
            new TaskItem
            {
                Id = new Guid("15dd8b18-0282-44b9-b8b5-dad1b20b8e73"),
                Title = "Create API Endpoints",
                Description = "Develop RESTful APIs for the mobile app backend.",
                Priority = 5,
                Status = "In Progress",
                ProjectId = new Guid("4dba79d2-6e3b-41ea-a2f8-d511d9d36c54")
            },
            new TaskItem
            {
                Id = new Guid("bcf4c86a-b2d1-408f-8c86-b19cb5ec6498"),
                Title = "Test Mobile App Features",
                Description = "Perform QA testing for mobile app functionality.",
                Priority = 2,
                Status = "Not Started",
                ProjectId = new Guid("4dba79d2-6e3b-41ea-a2f8-d511d9d36c54")
            }
        );
    }
}
