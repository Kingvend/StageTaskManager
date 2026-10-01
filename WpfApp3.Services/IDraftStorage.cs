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
        DateTimeOffset savedAt,
        CancellationToken ct = default);

    Task<PairDraftDto?> TryLoadPairAsync(Guid projectId, Guid pairId, CancellationToken ct = default);
}