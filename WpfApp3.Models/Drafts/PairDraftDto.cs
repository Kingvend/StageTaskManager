namespace ProjectName.Models.Drafts;

/// <summary>Содержимое {draftId}.json — данные одной пары со всеми вариантами.</summary>
public class PairDraftDto
{
    /// <summary>Guid из БД. Guid.Empty, если пара ещё не сохранялась в БД.</summary>
    public Guid PairId { get; set; }

    public Guid DraftId { get; set; }
    public Guid CalculationId { get; set; }

    public DateTimeOffset SavedAt { get; set; }

    public StageSnapshot Stage { get; set; } = new();
    public TaskSnapshot Task { get; set; } = new();

    public PairStatus Status { get; set; }

    /// <summary>Ключ — Variant.Id. Значение — данные блока по этому варианту.</summary>
    public Dictionary<long, VariantDraftDto> Variants { get; set; } = new();
}

public class VariantDraftDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }

    /// <summary>Ключ — AgreementRole.ToString(), значение — IAgreementBlock.ToDictionary().</summary>
    public Dictionary<string, object> Blocks { get; set; } = new();
}