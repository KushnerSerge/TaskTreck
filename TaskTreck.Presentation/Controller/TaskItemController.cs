using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
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
        try
        {
            var taskItems = await _service.TaskItemService.GetTaskItemsAsync(projectId, trackChanges: false);
            return Ok(taskItems);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{id:guid}", Name = "GetTaskItemsForProject")]
    public async Task<IActionResult> GetTaskItemForProject(Guid projectId, Guid id)
    {
        try
        {
            var taskItem = await _service.TaskItemService.GetTaskItemAsync(projectId, id, trackChanges: false);
            if (taskItem is null)
                return NotFound($"TaskItem with ID {id} for Project ID {projectId} was not found.");

            return Ok(taskItem);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTaskItemForProject(Guid projectId, [FromBody] TaskItemForCreationDto taskItem)
    {
        try
        {
            if (taskItem is null)
                return BadRequest("TaskItemForCreationDto object is null");

            var taskItemToReturn = await _service.TaskItemService.CreateTaskItemForProjectAsync(projectId, taskItem, trackChanges: false);

            if (taskItemToReturn is null)
                return NotFound($"Project with ID {projectId} was not found.");

            return CreatedAtRoute("GetTaskItemsForProject", new { projectId, id = taskItemToReturn.Id }, taskItemToReturn);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTaskItemForProject(Guid projectId, Guid id)
    {
        try
        {
            var taskItem = await _service.TaskItemService.GetTaskItemAsync(projectId, id, trackChanges: false);
            if (taskItem is null)
                return NotFound($"TaskItem with ID {id} for Project ID {projectId} was not found.");

           await _service.TaskItemService.DeleteTaskItemForProjectAsync(projectId, id, trackChanges: false);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTaskItemForProject(Guid projectId, Guid id, [FromBody] TaskItemForUpdateDto taskItem)
    {
        try
        {
            if (taskItem is null)
                return BadRequest("TaskItemForUpdateDto object is null.");

            var taskItemEntity = await _service.TaskItemService.GetTaskItemAsync(projectId, id, trackChanges: false);
            if (taskItemEntity is null)
                return NotFound($"TaskItem with ID {id} for Project ID {projectId} was not found.");

            await _service.TaskItemService.UpdateTaskItemForProjectAsync(projectId, id, taskItem, projectTrackChanges: false, taskItemTrackChanges: true);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PartiallyUpdateTaskItemForProject(Guid projectId, Guid id, [FromBody] JsonPatchDocument<TaskItemForUpdateDto> patchDoc)
    {
        try
        {
            if (patchDoc is null)
                return BadRequest("The patch document sent from the client is null.");

            var result = await _service.TaskItemService.GetTaskItemForPatchAsync(projectId, id, projectTrackChanges: false, taskItemTrackChanges: true);

            if (result.taskItemToPatch is null || result.taskItemEntity is null)
                return NotFound($"TaskItem with ID {id} for Project ID {projectId} was not found.");

            // Validate the patch document paths
            try
            {
                // Apply the patch
                patchDoc.ApplyTo(result.taskItemToPatch);
            }
            catch (JsonPatchException ex)
            {
                return BadRequest($"Invalid patch operation: {ex.Message}");
            }

            // Validate the patched object
            if (!TryValidateModel(result.taskItemToPatch))
            {
                return UnprocessableEntity(ModelState);
            }

            // Save the changes
            await _service.TaskItemService.SaveChangesForPatchAsync(result.taskItemToPatch, result.taskItemEntity);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }



}