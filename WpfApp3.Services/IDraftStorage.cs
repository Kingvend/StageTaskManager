using ProjectName.Models;
using ProjectName.Models.Drafts;

namespace ProjectName.Services;

public interface IDraftStorage
{
    Task SaveProjectAsync(Project project, DateTimeOffset savedAt, CancellationToken ct = default);

    Task SavePairAsync(
        Project project,
        Stage stage,
        ProjectTask task,
        IReadOnlyCollection<IAgreementBlock> blocks,
        DateTimeOffset savedAt,
        CancellationToken ct = default);

    /// <summary>Читает {pairGuid}.json, если он есть. Возвращает null, если файла нет.</summary>
    Task<PairDraftDto?> TryLoadPairAsync(Guid projectId, Guid pairId, CancellationToken ct = default);
}