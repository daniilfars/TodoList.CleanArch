using Microsoft.EntityFrameworkCore;
using Domain.Models;

namespace Application.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Project> Projects { get; set; }
    DbSet<TaskItem> Tasks { get; set; }
    DbSet<Tag> Tags { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
