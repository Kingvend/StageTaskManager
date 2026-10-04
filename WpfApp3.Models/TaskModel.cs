namespace ProjectName.Models;

/// <summary>
/// Согласования по одному варианту внутри пары. Программная сущность,
/// сериализуется в JSON-колонку таблицы «Заполнение варианта».
/// </summary>
public class TaskModel
{
    public Variant Variant { get; set; } = new();

    public Dictionary<AgreementRole, IAgreementBlock> Blocks { get; set; } = new();

    public bool IsFullyAgreed =>
        Blocks.Count > 0 && Blocks.Values.All(b => b.IsAgreed);
}