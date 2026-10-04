namespace ProjectName.Models.Drafts;

/// <summary>Кэш проекта, который пишется и в calculation.json, и в пары.</summary>
public class ProjectSnapshot
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ProjectStatus Status { get; set; }
    public string Responsible { get; set; } = string.Empty;
}

public class VariantSnapshot
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class StageSnapshot
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int OrderNumber { get; set; }
}

public class TaskSnapshot
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}