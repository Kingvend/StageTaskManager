namespace ProjectName.Models;

/// <summary>
/// Основная сущность нашего домена. Один проект = один расчёт.
/// </summary>
public class ProjectCalculation
{
    /// <summary>Guid из нашей БД. null, пока расчёт не сохранён.</summary>
    public Guid? Id { get; set; }

    /// <summary>Кэш внешнего проекта (все поля).</summary>
    public Project Project { get; set; } = new();

    /// <summary>Кэш доступных вариантов расчёта (из внешнего сервиса).</summary>
    public List<Variant> AvailableVariants { get; set; } = new();

    public int StartYear { get; set; }
    public int EndYear { get; set; }

    /// <summary>
    /// Все пары расчёта. Базовые поля заполнены при загрузке,
    /// <c>VariantData</c> — лениво, при открытии окна деталей.
    /// </summary>
    public List<StageTaskPair> Pairs { get; set; } = new();

    public DateTimeOffset? UpdatedAt { get; set; }
}