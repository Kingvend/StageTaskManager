namespace ProjectName.Models;

public class Stage
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OrderNumber { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public StageStatus Status { get; set; }

    public List<ProjectTask> Tasks { get; set; } = new();
}