using ProjectName.Models;

namespace ProjectName.Services;

/// <summary>
/// TODO: заменить на реальный вызов внешней БД/сервиса.
/// Сейчас — детерминированные демо-данные с фиксированными long Id,
/// чтобы черновики между запусками оставались валидными.
/// </summary>
public class ExternalCatalogService : IExternalCatalogService
{
    private const long DemoProjectId = 101;

    private static readonly long[] AllVariantIds = { 1001, 1002, 1003 };


    private static readonly Variant[] AllVariants =
    {
        new() { Id = 1001, Name = "Минимальный",  Order = 1 },
        new() { Id = 1002, Name = "Оптимальный",  Order = 2 },
        new() { Id = 1003, Name = "Максимальный", Order = 3 },
    };

    private static readonly Stage[] Stages =
    {
        new()
        {
            Id = 201, ProjectId = DemoProjectId,
            Name = "Аналитика", Description = "Сбор и анализ требований",
            OrderNumber = 1,
            StartDate = new DateTime(2024, 01, 15), EndDate = new DateTime(2024, 03, 01),
            Status = StageStatus.Completed,
        },
        new()
        {
            Id = 202, ProjectId = DemoProjectId,
            Name = "Разработка", Description = "Реализация модулей",
            OrderNumber = 2,
            StartDate = new DateTime(2024, 03, 02), EndDate = new DateTime(2024, 09, 01),
            Status = StageStatus.InProgress,
        },
    };

    private static readonly ProjectTask[] Tasks =
    {
        new() { Id = 301, StageId = 201, Name = "Интервью с заказчиком", Description = "..." },
        new() { Id = 302, StageId = 201, Name = "ТЗ",                    Description = "..." },
        new() { Id = 303, StageId = 202, Name = "Модуль склад",          Description = "..." },
        new() { Id = 304, StageId = 202, Name = "Модуль продажи",        Description = "..." },
    };

    public Task<Project?> GetProjectAsync(long projectId, CancellationToken ct = default)
    {
        if (projectId != DemoProjectId) return Task.FromResult<Project?>(null);

        var project = new Project
        {
            Id = DemoProjectId,
            Name = "Внедрение ERP",
            Description = "Пилотный проект внедрения ERP в подразделении",
            StartDate = new DateTime(2024, 01, 15),
            EndDate = new DateTime(2024, 12, 20),
            Status = ProjectStatus.InProgress,
            Responsible = "Иванов И.И.",
        };
        return Task.FromResult<Project?>(project);
    }

    public static class DemoIds
    {
        public const long ProjectId = 101;
        public static readonly Guid CalculationId = Guid.Parse("c0c0c0c0-0000-0000-0000-000000000001");
        public static readonly long[] VariantIds = { 1001, 1002, 1003 };
    }

    public Task<IReadOnlyList<Stage>> GetStagesAsync(long projectId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Stage>>(Stages.Where(s => s.ProjectId == projectId).ToList());

    public Task<IReadOnlyList<ProjectTask>> GetTasksAsync(long stageId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<ProjectTask>>(Tasks.Where(t => t.StageId == stageId).ToList());

    public Task<IReadOnlyList<Variant>> GetVariantsAsync(
        IReadOnlyList<long> variantIds, CancellationToken ct = default)
    {
        var set = variantIds.ToHashSet();
        IReadOnlyList<Variant> result = AllVariants.Where(v => set.Contains(v.Id)).ToList();
        return Task.FromResult(result);
    }
}