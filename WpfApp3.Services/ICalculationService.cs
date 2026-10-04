using ProjectName.Models;

namespace ProjectName.Services;

/// <summary>
/// Операции с нашим доменом. Реализация — заглушка; в будущем — вызов эндпоинтов.
/// </summary>
public interface ICalculationService
{
    /// <summary>
    /// Ищет расчёт по проекту. Если найден — возвращает его вместе со списком пар
    /// (базовые поля: Id, CalculationId, StageId, TaskId, Status; без VariantData).
    /// Если не найден — null.
    /// </summary>
    Task<ProjectCalculation?> GetByProjectAsync(long projectId, CancellationToken ct = default);

    /// <summary>Ленивая загрузка пары с заполненным VariantData по её PairId из БД.</summary>
    Task<StageTaskPair?> GetPairAsync(Guid pairId, CancellationToken ct = default);

    /// <summary>Сохраняет метаданные расчёта. Возвращает CalculationId.</summary>
    Task<Guid> SaveCalculationAsync(ProjectCalculation calculation, CancellationToken ct = default);

    /// <summary>Сохраняет пару. Возвращает PairId.</summary>
    Task<Guid> SavePairAsync(StageTaskPair pair, CancellationToken ct = default);

    /// <summary>Отправляет расчёт на рассмотрение.</summary>
    Task SendForReviewAsync(Guid calculationId, CancellationToken ct = default);
}