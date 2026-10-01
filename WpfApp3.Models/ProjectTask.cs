namespace ProjectName.Models;

public class ProjectTask
{
    public Guid PairId { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }

    /// <summary>Согласования по вариантам исполнения.</summary>
    public Dictionary<Variant, TaskModel> Variants { get; set; } = new();

    public bool IsFullyAgreed =>
        Variants.Count > 0 && Variants.Values.All(vm => vm.IsFullyAgreed);
}