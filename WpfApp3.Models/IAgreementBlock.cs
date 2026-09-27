namespace ProjectName.Models;

public interface IAgreementBlock
{
    AgreementRole Role { get; }
    bool IsAgreed { get; set; }
    string Comment { get; set; }

    Dictionary<string, object> ToDictionary();

    /// <summary>Восстанавливает состояние блока из словаря (после чтения черновика).</summary>
    void LoadFrom(IReadOnlyDictionary<string, object> data);
}