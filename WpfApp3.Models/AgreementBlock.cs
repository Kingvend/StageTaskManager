using ProjectName.Models;

public class AgreementBlock : IAgreementBlock
{
    public AgreementRole Role { get; set; }
    public bool IsAgreed { get; set; }
    public string Comment { get; set; } = string.Empty;

    public virtual Dictionary<string, object> ToDictionary() => new()
    {
        ["Role"] = Role.ToString(),
        ["IsAgreed"] = IsAgreed,
        ["Comment"] = Comment,
    };

    public virtual void LoadFrom(IReadOnlyDictionary<string, object> data)
    {
        if (data.TryGetValue("IsAgreed", out var ia) && ia is bool b) IsAgreed = b;
        if (data.TryGetValue("Comment", out var c) && c is string s) Comment = s;
    }

    protected static string GetString(IReadOnlyDictionary<string, object> data, string key)
        => data.TryGetValue(key, out var v) ? v?.ToString() ?? string.Empty : string.Empty;
}