using ProjectName.Models;
using TaskStatus = ProjectName.Models.TaskStatus;

namespace ProjectName.Services;

public class ProjectService : IProjectService
{
    private static readonly Guid DemoProjectId = Guid.Parse("b1e0c1a2-1111-2222-3333-444455556666");

    private static readonly Variant MinVariant = new()
    { Id = Guid.Parse("a0000001-0000-0000-0000-000000000001"), Name = "Минимальный", Order = 1 };
    private static readonly Variant OptVariant = new()
    { Id = Guid.Parse("a0000002-0000-0000-0000-000000000002"), Name = "Оптимальный", Order = 2 };
    private static readonly Variant MaxVariant = new()
    { Id = Guid.Parse("a0000003-0000-0000-0000-000000000003"), Name = "Максимальный", Order = 3 };

    private static readonly Guid Pair1Id = Guid.Parse("7f5a1111-aaaa-bbbb-cccc-1122334455aa");
    private static readonly Guid Pair2Id = Guid.Parse("8c1b2222-dddd-eeee-ffff-1122334455bb");
    private static readonly Guid Pair3Id = Guid.Parse("9d2c3333-aaaa-bbbb-cccc-1122334455cc");

    public Project GetCurrentProject()
    {
        var project = new Project
        {
            Id = DemoProjectId,
            Name = "Внедрение ERP",
            Description = "Пилотный проект внедрения ERP в подразделении",
            StartDate = new DateTime(2024, 01, 15),
            EndDate = new DateTime(2024, 12, 20),
            Status = ProjectStatus.InProgress,
            Responsible = "Иванов И.И.",
            AvailableVariants = new List<Variant> { MinVariant, OptVariant, MaxVariant },
        };

        project.Stages.Add(new Stage
        {
            Name = "Аналитика",
            Description = "Сбор и анализ требований",
            OrderNumber = 1,
            StartDate = new DateTime(2024, 01, 15),
            EndDate = new DateTime(2024, 03, 01),
            Status = StageStatus.Completed,
            Tasks = new List<ProjectTask>
            {
                BuildTask(Pair1Id, "Интервью с заказчиком", "...", TaskStatus.Completed, project.AvailableVariants),
                BuildTask(Pair2Id, "ТЗ", "...", TaskStatus.Completed, project.AvailableVariants),
            }
        });

        project.Stages.Add(new Stage
        {
            Name = "Разработка",
            Description = "Реализация модулей",
            OrderNumber = 2,
            StartDate = new DateTime(2024, 03, 02),
            EndDate = new DateTime(2024, 09, 01),
            Status = StageStatus.InProgress,
            Tasks = new List<ProjectTask>
            {
                BuildTask(Pair3Id, "Модуль склад", "...", TaskStatus.InProgress, project.AvailableVariants),
            }
        });

        return project;
    }

    private static ProjectTask BuildTask(
        Guid pairId, string name, string description, TaskStatus status, List<Variant> variants)
    {
        var task = new ProjectTask
        {
            PairId = pairId,
            Name = name,
            Description = description,
            Status = status,
        };

        foreach (var variant in variants)
        {
            var taskModel = new TaskModel { Variant = variant };
            foreach (var role in Enum.GetValues<AgreementRole>())
                taskModel.Blocks[role] = new AgreementBlock { Role = role };

            task.Variants[variant] = taskModel;
        }

        return task;
    }

    public Task SaveAsync(Project project, CancellationToken ct = default)
        => Task.CompletedTask;
}