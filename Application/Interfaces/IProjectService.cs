using Application.DTOs.CreateDTOs;
using Application.DTOs.UpdateDTOs;
using Application.DTOs.ResponseDTOs;

namespace Application.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ResponseProjectDto>> GetAllProjectsAsync();
    Task<ResponseProjectDto?> GetProjectByIdAsync(int id);
    Task<ResponseProjectDto> CreateProjectAsync(CreateProjectDto createProject);
    Task<ResponseProjectDto?> UpdateProjectAsync(int id, UpdateProjectDto updateProject);
    Task<bool> DeleteProjectAsync(int id);
    Task<IEnumerable<ResponseProjectDto>> GetProjectsByUserIdAsync(int userId);
}
