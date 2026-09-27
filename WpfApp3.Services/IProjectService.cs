using ProjectName.Models;

namespace ProjectName.Services;

public interface IProjectService
{
    Project GetCurrentProject();
    Task SaveAsync(Project project, CancellationToken ct = default);
}