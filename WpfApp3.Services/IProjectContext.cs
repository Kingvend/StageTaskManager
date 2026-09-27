using ProjectName.Models;

namespace ProjectName.Services;

/// <summary>
/// Общий контекст приложения: хранит загруженный проект,
/// чтобы разные ViewModel'и работали с одними и теми же экземплярами.
/// </summary>
public interface IProjectContext
{
    Project CurrentProject { get; }
    void Initialize(Project project);
}