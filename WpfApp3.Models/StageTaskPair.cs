namespace ProjectName.Models;

/// <summary>
/// Пара «этап-задача» — основная единица нашего домена.
/// </summary>
public class StageTaskPair
{
    /// <summary>Guid из нашей БД. Guid.Empty, пока пара не сохранена.</summary>
    public Guid Id { get; set; } = Guid.Empty;

    /// <summary>Детерминированный Guid для имени файла черновика. null, пока нет CalculationId.</summary>
    public Guid? DraftId { get; set; }

    /// <summary>Ссылка на расчёт. null, пока расчёт не сохранён в БД.</summary>
    public Guid? CalculationId { get; set; }

    public long StageId { get; set; }
    public long TaskId { get; set; }

    /// <summary>Кэш внешней сущности (имя, номер).</summary>
    public Stage Stage { get; set; } = new();

    /// <summary>Кэш внешней сущности (имя, описание).</summary>
    public ProjectTask Task { get; set; } = new();

    /// <summary>Подмножество вариантов из расчёта, применимых к этой паре.</summary>
    public List<Variant> AvailableVariants { get; set; } = new();

    /// <summary>Заполнение по вариантам. Грузится лениво (из БД или черновика).</summary>
    public Dictionary<Variant, TaskModel> VariantData { get; set; } = new();

    public PairStatus Status { get; set; } = PairStatus.NotStarted;

    /// <summary>Все блоки всех вариантов согласованы.</summary>
    public bool IsFullyAgreed =>
        VariantData.Count > 0 && VariantData.Values.All(vm => vm.IsFullyAgreed);

    public DateTimeOffset? UpdatedAt { get; set; }
}