using Application.DTOs.CreateDTOs;
using Application.DTOs.UpdateDTOs;
using Application.DTOs.ResponseDTOs;
using Application.RequestFeatures;

namespace Application.Interfaces;

public interface ITaskService
{
    //Task<IEnumerable<ResponseTaskDto>> GetAllTasksAsync();
    Task<ResponseTaskDto?> GetTaskByIdAsync(int id);
    Task<ResponseTaskDto> CreateTaskAsync(CreateTaskDto createTask);
    Task<ResponseTaskDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTask);
    Task<bool> DeleteTaskAsync(int id);
    Task<ResponsePaged<ResponseTaskDto>> GetTasksAsync(TaskQueryParameters parameters, int userId);
}
