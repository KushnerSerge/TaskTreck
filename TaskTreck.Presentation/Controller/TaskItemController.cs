

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTreck.Presentation.Controller;
[Route("api/projects/{projectId}/taskItems")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
public class TaskItemsController : ControllerBase
{
    private readonly IServiceManager _service;

    public TaskItemsController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetTaskItemsForProject(Guid projectId)
    {
        var taskItems = await _service.TaskItemService.GetTaskItemsAsync(projectId, trackChanges: false);
        return Ok(taskItems);
    }

    [HttpGet("{id:guid}", Name = "GetTaskItemsForProject")]
    public async Task<IActionResult> GetTaskItemForProject(Guid projectId, Guid id)
    {
        var taskItem = await _service.TaskItemService.GetTaskItemAsync(projectId, id, trackChanges: false);
        return Ok(taskItem);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTaskItemForProject(Guid projectId, [FromBody] TaskItemForCreationDto taskItem)
    {
        if (taskItem is null)
            return BadRequest("TaskItemForCreationDto object is null");

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var taskItemToReturn = await _service.TaskItemService.CreateTaskItemForProjectAsync(projectId, taskItem, trackChanges: false);

        return CreatedAtRoute("GetTaskItemForProject", new { projectId, id = taskItemToReturn?.Id }, taskItemToReturn);

    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTaskItemForProject(Guid projectId, Guid id)
    {
        await _service.TaskItemService.DeleteTaskItemForProjectAsync(projectId, id, trackChanges: false);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTaskItemForProject(Guid projectId, Guid id, [FromBody] TaskItemForUpdateDto taskItem)
    {
        if (taskItem is null)
            return BadRequest("TaskItemForUpdateDto object is null.");

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        await _service.TaskItemService.UpdateTaskItemForProjectAsync(projectId, id, taskItem, projectTrackChanges: false, taskItemTrackChanges: true);
        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PartiallyUpdateTaskItemForProject(Guid projectId, Guid id, [FromBody] JsonPatchDocument<TaskItemForUpdateDto> patchDoc)
    {
        if (patchDoc is null)
            return BadRequest("The patch document sent from the client is null.");

        var result = await _service.TaskItemService.GetTaskItemForPatchAsync(projectId, id,
            projectTrackChanges: false, taskItemTrackChanges: true);
  
        patchDoc.ApplyTo(result.taskItemToPatch);

        TryValidateModel(result.taskItemToPatch);

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        await _service.TaskItemService.SaveChangesForPatchAsync(result.taskItemToPatch, result.taskItemEntity);

        return NoContent();

    }
}