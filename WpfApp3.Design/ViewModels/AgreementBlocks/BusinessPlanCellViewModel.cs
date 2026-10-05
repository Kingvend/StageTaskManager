using CommunityToolkit.Mvvm.ComponentModel;
using ProjectName.Models;

namespace ProjectName.Wpf.ViewModels.AgreementBlocks;

public class BusinessPlanCellViewModel : ObservableObject
{
    private readonly BusinessPlanEntry _entry;

    public int Year => _entry.Year;

    public decimal Value
    {
        get => _entry.Value;
        set
        {
            if (_entry.Value == value) return;
            _entry.Value = value;
            OnPropertyChanged();
        }
    }

    public BusinessPlanCellViewModel(BusinessPlanEntry entry) => _entry = entry;
}