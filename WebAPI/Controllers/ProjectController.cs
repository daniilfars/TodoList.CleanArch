using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Application.DTOs.CreateDTOs;
using Application.DTOs.ResponseDTOs;
using Application.DTOs.UpdateDTOs;
using Application.Interfaces;
using Application.RequestFeatures;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectService projectService;

    public ProjectController(IProjectService _projectService)
    {
        projectService = _projectService;
    }

    // GET: api/project
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<ResponsePaged<ResponseProjectDto>>> GetAll(ProjectQueryParameters parameters)
    {
        var items = await projectService.GetAllProjectsAsync(parameters);
        return Ok(items);
    }

    // GET: api/project/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseProjectDto>> GetById(int id)
    {
        var item = await projectService.GetProjectByIdAsync(id);
        if (item == null)
            return NotFound();
        return Ok(item);
    }

    // POST: api/project
    [HttpPost]
    public async Task<ActionResult<ResponseProjectDto>> Create(CreateProjectDto createDto)
    {
        try
        {
            var newItem = await projectService.CreateProjectAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = newItem.Id }, newItem);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/project/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProjectDto updateDto)
    {
        try
        {
            var updatedItem = await projectService.UpdateProjectAsync(id, updateDto);
            if (updatedItem == null)
                return NotFound();
            return Ok(updatedItem);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/project/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await projectService.DeleteProjectAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    // GET: api/project/my
    [HttpGet("my")]
    public async Task<ActionResult<ResponsePaged<ResponseProjectDto>>> GetMyProjects([FromQuery] ProjectQueryParameters parameters)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { message = "ID пользователя не найден в токене" });

        var projects = await projectService.GetProjectsAsync(parameters, userId);
        return Ok(projects);
    }
}