namespace ProjectName.Models.Drafts;

/// <summary>Содержимое calculation.json. Список пар не хранится.</summary>
public class CalculationDraftDto
{
    public Guid CalculationId { get; set; }
    public Guid DraftId { get; set; }
    public DateTimeOffset SavedAt { get; set; }

    public int StartYear { get; set; }
    public int EndYear { get; set; }

    public ProjectSnapshot Project { get; set; } = new();
    public List<VariantSnapshot> AvailableVariants { get; set; } = new();
}