namespace ProjectName.Models;

public class ProjectTask
{
    public Guid PairId { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }

    public List<ApprovalInfo> Approvals { get; set; } = new();
}