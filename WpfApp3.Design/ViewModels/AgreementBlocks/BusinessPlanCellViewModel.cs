using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ProjectName.Models;

namespace ProjectName.Wpf.ViewModels.AgreementBlocks;

public class BusinessPlanCellViewModel : ObservableObject
{
    private readonly BlockLeadBlock _model;
    private readonly string _indicator;
    private readonly int _year;

    public int Year => _year;

    public decimal? Value
    {
        get => _model.BusinessPlan
            .FirstOrDefault(e => e.Indicator == _indicator && e.Year == _year)?.Value;

        set
        {
            var entry = _model.BusinessPlan
                .FirstOrDefault(e => e.Indicator == _indicator && e.Year == _year);

            if (value is null)
            {
                // Очистили ячейку — удаляем запись из модели.
                if (entry is not null)
                    _model.BusinessPlan.Remove(entry);
            }
            else if (entry is null)
            {
                _model.BusinessPlan.Add(new BusinessPlanEntry
                {
                    Indicator = _indicator,
                    Year = _year,
                    Value = value,
                });
            }
            else
            {
                entry.Value = value;
            }

            OnPropertyChanged();
        }
    }

    public BusinessPlanCellViewModel(BlockLeadBlock model, string indicator, int year)
    {
        _model = model;
        _indicator = indicator;
        _year = year;
    }
}