namespace ProjectName.Models;

public class TaskModel
{
    public Variant Variant { get; set; } = new();

    /// <summary>Согласования по ролям для данного варианта.</summary>
    public Dictionary<AgreementRole, IAgreementBlock> Blocks { get; set; } = new();

    public bool IsFullyAgreed =>
        Blocks.Count > 0 && Blocks.Values.All(b => b.IsAgreed);
}