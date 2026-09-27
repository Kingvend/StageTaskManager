using ProjectName.Models;
using TaskStatus = ProjectName.Models.TaskStatus;

namespace ProjectName.Services;

public class ProjectService : IProjectService
{
    // Фиксированный guid, чтобы демо-данные всегда писались в одну и ту же папку черновиков.
    private static readonly Guid DemoProjectId = Guid.Parse("b1e0c1a2-1111-2222-3333-444455556666");
    private static readonly Guid Pair1Id = Guid.Parse("7f5a1111-aaaa-bbbb-cccc-1122334455aa");
    private static readonly Guid Pair2Id = Guid.Parse("8c1b2222-dddd-eeee-ffff-1122334455bb");
    private static readonly Guid Pair3Id = Guid.Parse("9d2c3333-aaaa-bbbb-cccc-1122334455cc");

    public Project GetCurrentProject()
    {
        return new Project
        {
            Id = DemoProjectId,
            Name = "Внедрение ERP",
            Description = "Пилотный проект внедрения ERP в подразделении",
            StartDate = new DateTime(2024, 01, 15),
            EndDate = new DateTime(2024, 12, 20),
            Status = ProjectStatus.InProgress,
            Responsible = "Иванов И.И.",
            Stages = new List<Stage>
            {
                new()
                {
                    Name = "Аналитика", Description = "Сбор и анализ требований",
                    OrderNumber = 1,
                    StartDate = new DateTime(2024, 01, 15),
                    EndDate   = new DateTime(2024, 03, 01),
                    Status = StageStatus.Completed,
                    Tasks = new List<ProjectTask>
                    {
                        new() { PairId = Pair1Id, Name = "Интервью с заказчиком", Description = "...", Status = TaskStatus.Completed },
                        new() { PairId = Pair2Id, Name = "ТЗ", Description = "...", Status = TaskStatus.Completed },
                    }
                },
                new()
                {
                    Name = "Разработка", Description = "Реализация модулей",
                    OrderNumber = 2,
                    StartDate = new DateTime(2024, 03, 02),
                    EndDate   = new DateTime(2024, 09, 01),
                    Status = StageStatus.InProgress,
                    Tasks = new List<ProjectTask>
                    {
                        new() { PairId = Pair3Id, Name = "Модуль склад", Description = "...", Status = TaskStatus.InProgress },
                    }
                },
            }
        };
    }

    public Task SaveAsync(Project project, CancellationToken ct = default)
        => Task.CompletedTask;
}