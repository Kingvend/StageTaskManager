using ProjectName.Models;
using static ProjectName.Services.ExternalCatalogService;

namespace ProjectName.Services;

/// <summary>
/// TODO: заменить на HTTP-вызовы эндпоинтов нашего backend.
/// Пока — in-memory заглушка, чтобы UI и сценарии работали end-to-end.
/// </summary>
public class CalculationService : ICalculationService
{
    private readonly IExternalCatalogService _catalog;
    private readonly ICalculationContext _context;

    // Заглушка «наша БД»: один расчёт на проект, пары в памяти.
    private readonly Dictionary<long, ProjectCalculation> _store = new();

    public CalculationService(IExternalCatalogService catalog, ICalculationContext context)
    {
        _catalog = catalog;
        _context = context;
    }

    public Task<ProjectCalculation?> GetByProjectAsync(long projectId, CancellationToken ct = default)
    {
        if (_store.TryGetValue(projectId, out var existing))
            return Task.FromResult<ProjectCalculation?>(existing);

        // Демо-заглушка: возвращаем «уже существующий» расчёт с фиксированным Id,
        // чтобы черновики между запусками оставались валидными.
        if (projectId == DemoIds.ProjectId)
        {
            var seed = new ProjectCalculation
            {
                Id = DemoIds.CalculationId,
                // Project и AvailableVariants заполнит MainViewModel при LoadAsync
            };
            _store[projectId] = seed;
            return Task.FromResult<ProjectCalculation?>(seed);
        }

        return Task.FromResult<ProjectCalculation?>(null);
    }

    public Task<StageTaskPair?> GetPairAsync(Guid pairId, CancellationToken ct = default)
    {
        // TODO: HTTP GET /calculations/pairs/{pairId}
        var pair = _store.Values
            .SelectMany(c => c.Pairs)
            .FirstOrDefault(p => p.Id == pairId);
        return Task.FromResult(pair);
    }

    public Task<Guid> SaveCalculationAsync(ProjectCalculation calculation, CancellationToken ct = default)
    {
        if (calculation.Id is null || calculation.Id == Guid.Empty)
            calculation.Id = Guid.NewGuid();

        calculation.UpdatedAt = DateTimeOffset.UtcNow;
        _store[calculation.Project.Id] = calculation;
        return Task.FromResult(calculation.Id.Value);
    }

    public Task<Guid> SavePairAsync(StageTaskPair pair, CancellationToken ct = default)
    {
        if (pair.Id == Guid.Empty)
            pair.Id = Guid.NewGuid();

        pair.UpdatedAt = DateTimeOffset.UtcNow;
        return Task.FromResult(pair.Id);
    }

    public Task SendForReviewAsync(Guid calculationId, CancellationToken ct = default)
    {
        // TODO: HTTP POST /calculations/{calculationId}/send-for-review
        return Task.CompletedTask;
    }
}