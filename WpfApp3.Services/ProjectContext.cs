using ProjectName.Models;

namespace ProjectName.Services;

public class ProjectContext : IProjectContext
{
    public Project CurrentProject { get; private set; } = new();
    public void Initialize(Project project) => CurrentProject = project;
}