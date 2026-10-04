namespace ProjectName.Models;

/// <summary>Программная сущность, в БД отдельных таблиц не имеет.</summary>
public class AgreementBlock : IAgreementBlock
{
    public AgreementRole Role { get; set; }
    public bool IsAgreed { get; set; }
    public string Comment { get; set; } = string.Empty;

    public Dictionary<string, object> ToDictionary() => new()
    {
        ["Role"] = Role.ToString(),
        ["IsAgreed"] = IsAgreed,
        ["Comment"] = Comment,
    };

    public void LoadFrom(IReadOnlyDictionary<string, object> data)
    {
        if (data.TryGetValue("IsAgreed", out var ia) && ia is bool b) IsAgreed = b;
        if (data.TryGetValue("Comment", out var c) && c is string s) Comment = s;
    }
}