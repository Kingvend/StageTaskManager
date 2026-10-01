namespace ProjectName.Models.Drafts;

public class ProjectDraftDto
{
    public Guid ProjectId { get; set; }
    public DateTimeOffset SavedAt { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ProjectStatus Status { get; set; }
    public string Responsible { get; set; } = string.Empty;

    public List<VariantSnapshot> AvailableVariants { get; set; } = new();
}

public class VariantSnapshot
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}