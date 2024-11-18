using Microsoft.AspNetCore.Mvc;

using Service.Contracts;
using Shared;
using TaskTreck.Presentation.ModelBinders;


namespace TaskTreck.Presentation.Controller;

[Route("api/projects")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
public class ProjectsController : ControllerBase
{
    private readonly IServiceManager _service;

    public ProjectsController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        try
        {
            var projects = await _service.ProjectService.GetAllProjectsAsync(trackChanges: false);

            return Ok(projects);
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id:guid}", Name = "ProjectById")]
    public async Task<IActionResult> GetProject(Guid id)
    {
        try
        {
            var project = await _service.ProjectService.GetProjectAsync(id, trackChanges: false);
            return Ok(project);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("collection/({ids})", Name = "ProjectCollection")]
    public async Task<IActionResult> GetProjectCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
    {
        try
        {
            var projects = await _service.ProjectService.GetByIdsAsync(ids, trackChanges: false);
            return Ok(projects);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] ProjectForCreationDto project)
    {
        try
        {
            if (project is null)
                return BadRequest("ProjectForCreationDto object is null");

            var createdProject = await _service.ProjectService.CreateProjectAsync(project);
            return CreatedAtRoute("ProjectById", new { id = createdProject?.Id }, createdProject);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("collection")]
    public async Task<IActionResult> CreateProjectCollection([FromBody] IEnumerable<ProjectForCreationDto> projectCollection)
    {
        if (projectCollection is null || !projectCollection.Any())
            return BadRequest("Project collection is null or empty");
        try
        {
            var result = await _service.ProjectService.CreateProjectCollectionAsync(projectCollection);
            return CreatedAtRoute("ProjectCollection", new { result.ids }, result.projects);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        try
        {
            await _service.ProjectService.DeleteProjectAsync(id, trackChanges: false);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] ProjectForUpdateDto project)
    {
        try
        {
            if (project is null)
                return BadRequest("ProjectForUpdateDto object is null");

            await _service.ProjectService.UpdateProjectAsync(id, project, trackChanges: true);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}