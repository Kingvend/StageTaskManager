namespace ProjectName.Models;

public class BlockLeadBlock : AgreementBlock
{
    public List<BusinessPlanEntry> BusinessPlan { get; set; } = new();

    public override Dictionary<string, object> ToDictionary()
    {
        var d = base.ToDictionary();

        // Сохраняем только заполненные ячейки. Пустые пропускаем.
        d["BusinessPlan"] = BusinessPlan
            .Where(e => e.Value.HasValue)
            .GroupBy(e => e.Indicator)
            .Select(g => (object)new Dictionary<string, object>
            {
                ["Indicator"] = g.Key,
                ["Values"] = g.ToDictionary(
                    e => e.Year.ToString(),
                    e => (object)e.Value!.Value),
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

            var indicator = GetString(dict, "Indicator");
            if (string.IsNullOrEmpty(indicator)) continue;

            if (!dict.TryGetValue("Values", out var v) || v is not IReadOnlyDictionary<string, object> values)
                continue;

            foreach (var kv in values)
            {
                if (!int.TryParse(kv.Key, out var year)) continue;

                var value = ToNullableDecimal(kv.Value);
                if (!value.HasValue) continue;   // защита от мусора

                BusinessPlan.Add(new BusinessPlanEntry
                {
                    Indicator = indicator,
                    Year = year,
                    Value = value,
                });
            }
        }
    }

    private static decimal? ToNullableDecimal(object? v) => v switch
    {
        null => null,
        decimal m => m,
        int i => i,
        long l => l,
        double db => (decimal)db,
        string s => decimal.TryParse(s, out var r) ? r : null,
        _ => null,
    };
}