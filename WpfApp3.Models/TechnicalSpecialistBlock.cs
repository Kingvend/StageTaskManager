namespace ProjectName.Models;

public class TechnicalSpecialistBlock : AgreementBlock
{
    public string Parameter1 { get; set; } = string.Empty;
    public string Parameter2 { get; set; } = string.Empty;
    public string Parameter3 { get; set; } = string.Empty;
    public string SelectedOption { get; set; } = string.Empty;

    public override Dictionary<string, object> ToDictionary()
    {
        var d = base.ToDictionary();
        d["Parameter1"] = Parameter1;
        d["Parameter2"] = Parameter2;
        d["Parameter3"] = Parameter3;
        d["SelectedOption"] = SelectedOption;
        return d;
    }

    public override void LoadFrom(IReadOnlyDictionary<string, object> data)
    {
        base.LoadFrom(data);
        Parameter1 = GetString(data, "Parameter1");
        Parameter2 = GetString(data, "Parameter2");
        Parameter3 = GetString(data, "Parameter3");
        SelectedOption = GetString(data, "SelectedOption");
    }
}