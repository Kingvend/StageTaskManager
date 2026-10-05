namespace ProjectName.Models;

public class BlockLeadBlock : AgreementBlock
{
    public List<BusinessPlanEntry> BusinessPlan { get; set; } = new();

    public override Dictionary<string, object> ToDictionary()
    {
        var d = base.ToDictionary();
        d["BusinessPlan"] = BusinessPlan
            .Select(e => (object)new Dictionary<string, object>
            {
                ["Indicator"] = e.Indicator,
                ["Year"] = e.Year,
                ["Value"] = e.Value,
            })
            .ToList();
        return d;
    }

    public override void LoadFrom(IReadOnlyDictionary<string, object> data)
    {
        base.LoadFrom(data);
        BusinessPlan.Clear();

        if (!data.TryGetValue("BusinessPlan", out var bp) || bp is not IEnumerable<object> list)
            return;

        foreach (var item in list)
        {
            if (item is not IReadOnlyDictionary<string, object> dict) continue;

            BusinessPlan.Add(new BusinessPlanEntry
            {
                Indicator = GetString(dict, "Indicator"),
                Year = GetInt(dict, "Year"),
                Value = GetDecimal(dict, "Value"),
            });
        }
    }

    private static int GetInt(IReadOnlyDictionary<string, object> d, string key)
    {
        if (!d.TryGetValue(key, out var v) || v is null) return 0;
        return v switch
        {
            int i => i,
            long l => (int)l,
            double db => (int)db,
            decimal m => (int)m,
            string s => int.TryParse(s, out var r) ? r : 0,
            _ => 0,
        };
    }

    private static decimal GetDecimal(IReadOnlyDictionary<string, object> d, string key)
    {
        if (!d.TryGetValue(key, out var v) || v is null) return 0m;
        return v switch
        {
            decimal m => m,
            int i => i,
            long l => l,
            double db => (decimal)db,
            string s => decimal.TryParse(s, out var r) ? r : 0m,
            _ => 0m,
        };
    }
}