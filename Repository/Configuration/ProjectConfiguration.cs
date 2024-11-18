using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Repository.Configuration;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Relationships
        builder.HasMany(p => p.Tasks)
               .WithOne(t => t.Project)
               .HasForeignKey(t => t.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasData
        (
            new Project
            {
                Id = new Guid("5f93e88d-8c58-4633-bef7-9206b2f7c34d"),
                Name = "Website Redesign",
                Description = "Revamp the company's main website for better UX.",
                StartDate = new DateTime(2024, 01, 15),
                EndDate = new DateTime(2024, 05, 15)
            },
            new Project
            {
                Id = new Guid("4dba79d2-6e3b-41ea-a2f8-d511d9d36c54"),
                Name = "Mobile App Development",
                Description = "Develop a new mobile app for customer engagement.",
                StartDate = new DateTime(2024, 02, 01),
                EndDate = new DateTime(2024, 08, 01)
            }
        );
    }
}