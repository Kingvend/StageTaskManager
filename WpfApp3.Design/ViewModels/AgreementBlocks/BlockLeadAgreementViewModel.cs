using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ProjectName.Models;

namespace ProjectName.Wpf.ViewModels.AgreementBlocks;

public sealed class BlockLeadAgreementViewModel : AgreementBlockViewModel
{
    private static readonly string[] DefaultIndicators = { "Выручка", "Прибыль", "Затраты" };

    private readonly BlockLeadBlock _model;
    private readonly List<int> _years;

    public override AgreementRole Role => AgreementRole.BlockLead;
    public override string DisplayName => "Руководитель блока";

    public ObservableCollection<int> Years { get; } = new();
    public ObservableCollection<BusinessPlanRowViewModel> Rows { get; } = new();

    public BlockLeadAgreementViewModel(BlockLeadBlock model, int startYear, int endYear) : base(model)
    {
        _model = model;
        _years = Enumerable.Range(startYear, endYear - startYear + 1).ToList();

        foreach (var y in _years) Years.Add(y);

        BuildRows();
    }

    private void BuildRows()
    {
        Rows.Clear();

        foreach (var indicator in DefaultIndicators)
        {
            var row = new BusinessPlanRowViewModel(indicator);

            foreach (var year in _years)
                row.Cells.Add(new BusinessPlanCellViewModel(_model, indicator, year));

            Rows.Add(row);
        }
    }

    public override void LoadFrom(IReadOnlyDictionary<string, object> data)
    {
        base.LoadFrom(data);

        // Модель обновилась — пересобираем ячейки, чтобы UI показал новые значения.
        BuildRows();
    }
}