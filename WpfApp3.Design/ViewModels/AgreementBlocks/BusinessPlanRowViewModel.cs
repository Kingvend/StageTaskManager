using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectName.Wpf.ViewModels.AgreementBlocks;

public class BusinessPlanRowViewModel : ObservableObject
{
    private string _indicator = string.Empty;
    public string Indicator
    {
        get => _indicator;
        set { if (_indicator == value) return; _indicator = value; OnPropertyChanged(); }
    }

    public ObservableCollection<BusinessPlanCellViewModel> Cells { get; } = new();

    public BusinessPlanRowViewModel(string indicator) => _indicator = indicator;
}