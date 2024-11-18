using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskTreck.Migrations
{
    /// <inheritdoc />
    public partial class InitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                });

            migrationBuilder.CreateTable(
                name: "TaskItems",
                columns: table => new
                {
                    TaskItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItems", x => x.TaskItemId);
                    table.ForeignKey(
                        name: "FK_TaskItems_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "ProjectId", "Description", "EndDate", "Name", "StartDate" },
                values: new object[,]
                {
                    { new Guid("4dba79d2-6e3b-41ea-a2f8-d511d9d36c54"), "Develop a new mobile app for customer engagement.", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mobile App Development", new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("5f93e88d-8c58-4633-bef7-9206b2f7c34d"), "Revamp the company's main website for better UX.", new DateTime(2024, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Website Redesign", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "TaskItems",
                columns: new[] { "TaskItemId", "Description", "Priority", "ProjectId", "Status", "Title" },
                values: new object[,]
                {
                    { new Guid("15dd8b18-0282-44b9-b8b5-dad1b20b8e73"), "Develop RESTful APIs for the mobile app backend.", 5, new Guid("4dba79d2-6e3b-41ea-a2f8-d511d9d36c54"), "In Progress", "Create API Endpoints" },
                    { new Guid("1ac6734e-0f7a-4a96-9a35-2d854b69c18f"), "Design wireframes for all main pages.", 3, new Guid("5f93e88d-8c58-4633-bef7-9206b2f7c34d"), "In Progress", "Create Wireframes" },
                    { new Guid("4ad8b845-292c-42ec-9332-61e4a743d8a9"), "Implement frontend components using Angular.", 4, new Guid("5f93e88d-8c58-4633-bef7-9206b2f7c34d"), "Not Started", "Develop Frontend" },
                    { new Guid("bcf4c86a-b2d1-408f-8c86-b19cb5ec6498"), "Perform QA testing for mobile app functionality.", 2, new Guid("4dba79d2-6e3b-41ea-a2f8-d511d9d36c54"), "Not Started", "Test Mobile App Features" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_ProjectId",
                table: "TaskItems",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskItems");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
