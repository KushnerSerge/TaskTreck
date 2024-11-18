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
        var projects = await _service.ProjectService.GetAllProjectsAsync(trackChanges: false);
        return Ok(projects);

    }

    [HttpGet("{id:guid}", Name = "ProjectById")]
    public async Task<IActionResult> GetProject(Guid id)
    {
        var project = await _service.ProjectService.GetProjectAsync(id, trackChanges: false);
        return Ok(project);
    }

    [HttpGet("collection/({ids})", Name = "ProjectCollection")]
    public async Task<IActionResult> GetProjectCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
    {
        var projects = await _service.ProjectService.GetByIdsAsync(ids, trackChanges: false);
        return Ok(projects);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] ProjectForCreationDto project)
    {
        if (project is null)
            return BadRequest("ProjectForCreationDto object is null");

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var createdProject = await _service.ProjectService.CreateProjectAsync(project);
        return CreatedAtRoute("ProjectById", new { id = createdProject?.Id }, createdProject);
    }

    [HttpPost("collection")]
    public async Task<IActionResult> CreateProjectCollection([FromBody] IEnumerable<ProjectForCreationDto> projectCollection)
    {
         var result = await _service.ProjectService.CreateProjectCollectionAsync(projectCollection);
         return CreatedAtRoute("ProjectCollection", new { result.ids }, result.projects);   
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
       await _service.ProjectService.DeleteProjectAsync(id, trackChanges: false);
       return NoContent();    
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] ProjectForUpdateDto project)
    {
        if (project is null)
            return BadRequest("ProjectForUpdateDto object is null");

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        await _service.ProjectService.UpdateProjectAsync(id, project, trackChanges: true);
            return NoContent();
    }
}