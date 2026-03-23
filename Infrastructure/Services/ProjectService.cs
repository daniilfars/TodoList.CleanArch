using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Application.DTOs.CreateDTOs;
using Application.DTOs.ResponseDTOs;
using Application.DTOs.UpdateDTOs;
using Application.Interfaces;

namespace Infrastructure.Services;

public class ProjectService : IProjectService
{
    private readonly IAppDbContext db;

    public ProjectService(IAppDbContext context)
    {
        db = context; 
    }
    public async Task<ResponseProjectDto> CreateProjectAsync(CreateProjectDto createProject)
    {
        var userExists = await db.Users.AnyAsync(u => u.Id == createProject.UserId);
        if (!userExists)
            throw new InvalidOperationException("Пользователь с указанным id не существует.");

        Project project = new Project
        {
            Name = createProject.Name,
            Description = createProject.Description,
            CreatedAt = DateTime.UtcNow,
            UserId = createProject.UserId
        };

        await db.Projects.AddAsync(project);
        await db.SaveChangesAsync();

        return new ResponseProjectDto 
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            UserId = project.UserId
        };
    }

    public async Task<bool> DeleteProjectAsync(int id)
    {
        Project? project = await db.Projects.FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return false;

        db.Projects.Remove(project);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<ResponseProjectDto>> GetAllProjectsAsync()
    {
        return await db.Projects.AsNoTracking().OrderByDescending(p => p.CreatedAt).Select(project => new ResponseProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            UserId = project.UserId
        }).ToListAsync();
    }

    public async Task<ResponseProjectDto?> GetProjectByIdAsync(int id)
    {
        Project? project = await db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        if(project == null)
            return null;

        return new ResponseProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            UserId = project.UserId
        };
    }

    public async Task<ResponseProjectDto?> UpdateProjectAsync(int id, UpdateProjectDto updateProject)
    {
        Project? project = await db.Projects.FindAsync(id);

        if (project == null)
            return null;

        if (!string.IsNullOrWhiteSpace(updateProject.Name))
            project.Name = updateProject.Name;

        if (!string.IsNullOrWhiteSpace(updateProject.Description))
            project.Description = updateProject.Description;


        await db.SaveChangesAsync();

        return new ResponseProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            UserId = project.UserId
        };
    }

    public async Task<IEnumerable<ResponseProjectDto>> GetProjectsByUserIdAsync(int userId)
    {
        return await db.Projects
            .Where(p => p.UserId == userId)
            .AsNoTracking()
            .Select(p => new ResponseProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId
            })
            .ToListAsync();
    }
}