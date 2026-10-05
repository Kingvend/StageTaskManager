using ProjectName.Models;
using ProjectName.Models.Drafts;

public interface IDraftStorage
{
    /// <summary>Пишет calculation.json в папку drafts/{CalculationDraftId}/.</summary>
    Task SaveCalculationAsync(
        ProjectCalculation calculation, DateTimeOffset savedAt, CancellationToken ct = default);

    /// <summary>Пишет {PairDraftId}.json.</summary>
    Task SavePairAsync(
        ProjectCalculation calculation, StageTaskPair pair,
        DateTimeOffset savedAt, CancellationToken ct = default);

    /// <summary>Читает calculation.json. null, если файла нет или DraftId не совпадает.</summary>
    Task<CalculationDraftDto?> TryLoadCalculationAsync(
        long projectId, CancellationToken ct = default);

    /// <summary>Читает {PairDraftId}.json.</summary>
    Task<PairDraftDto?> TryLoadPairAsync(
        long projectId, long stageId, long taskId, CancellationToken ct = default);
}