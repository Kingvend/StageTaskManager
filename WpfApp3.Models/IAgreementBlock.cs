namespace ProjectName.Models;

/// <summary>
/// Контракт блока согласования. Используется и в доменной модели, и в VM-обёртках.
/// </summary>
public interface IAgreementBlock
{
    AgreementRole Role { get; }
    bool IsAgreed { get; set; }
    string Comment { get; set; }

    Dictionary<string, object> ToDictionary();
    void LoadFrom(IReadOnlyDictionary<string, object> data);
}