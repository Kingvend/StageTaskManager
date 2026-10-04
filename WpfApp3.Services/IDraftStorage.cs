using ProjectName.Models;
using ProjectName.Models.Drafts;

namespace ProjectName.Services;

public interface IDraftStorage
{
    /// <summary>Создаёт calculation.json, если файла ещё нет. Без пар.</summary>
    Task SaveCalculationAsync(
        ProjectCalculation calculation, DateTimeOffset savedAt, CancellationToken ct = default);

    /// <summary>Перезаписывает {draftId}.json — данные пары со всеми вариантами.</summary>
    Task SavePairAsync(
        StageTaskPair pair, DateTimeOffset savedAt, CancellationToken ct = default);

    /// <summary>Читает calculation.json. null, если файла нет.</summary>
    Task<CalculationDraftDto?> TryLoadCalculationAsync(
        Guid calculationId, CancellationToken ct = default);

    /// <summary>Читает {draftId}.json. null, если файла нет.</summary>
    Task<PairDraftDto?> TryLoadPairAsync(
        Guid calculationId, Guid draftId, CancellationToken ct = default);
}