using ProjectName.Models;

namespace ProjectName.Services;

/// <summary>
/// Источник внешних справочных данных (Project / Stage / Task / Variant).
/// Read-only. Реализация — заглушка; в будущем — вызов внешней БД/сервиса.
/// </summary>
public interface IExternalCatalogService
{
    Task<Project?> GetProjectAsync(long projectId, CancellationToken ct = default);

    Task<IReadOnlyList<Stage>> GetStagesAsync(long projectId, CancellationToken ct = default);

    Task<IReadOnlyList<ProjectTask>> GetTasksAsync(long stageId, CancellationToken ct = default);

    Task<IReadOnlyList<Variant>> GetVariantsAsync(
        IReadOnlyList<long> variantIds, CancellationToken ct = default);
}