namespace ProjectName.Models.Drafts;

public class PairDraftDto
{
    public Guid PairId { get; set; }
    public Guid ProjectId { get; set; }
    public DateTimeOffset SavedAt { get; set; }

    public StageSnapshot Stage { get; set; } = new();
    public TaskSnapshot Task { get; set; } = new();

    /// <summary>Ключ — Variant.Id.ToString("D"), значение — данные варианта.</summary>
    public Dictionary<string, VariantDraftDto> Variants { get; set; } = new();
}

public class VariantDraftDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }

    /// <summary>Ключ — AgreementRole.ToString(), значение — IAgreementBlock.ToDictionary().</summary>
    public Dictionary<string, object> Blocks { get; set; } = new();
}

public class StageSnapshot
{
    public string Name { get; set; } = string.Empty;
    public int OrderNumber { get; set; }
}

public class TaskSnapshot
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
}