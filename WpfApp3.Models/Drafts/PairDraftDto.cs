namespace ProjectName.Models.Drafts;

/// <summary>Содержимое {pairGuid}.json — данные одной пары «этап-задача» со всеми вкладками.</summary>
public class PairDraftDto
{
    public Guid PairId { get; set; }
    public Guid ProjectId { get; set; }
    public DateTimeOffset SavedAt { get; set; }

    public StageSnapshot Stage { get; set; } = new();
    public TaskSnapshot Task { get; set; } = new();

    /// <summary>Ключ — AgreementRole.ToString(), значение — результат IAgreementBlock.ToDictionary().</summary>
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