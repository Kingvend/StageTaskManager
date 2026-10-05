using ProjectName.Models;
using System.Collections.ObjectModel;

namespace ProjectName.Wpf.ViewModels.AgreementBlocks;

public sealed class TechnicalSpecialistAgreementViewModel : AgreementBlockViewModel
{
    private readonly TechnicalSpecialistBlock _model;

    public override AgreementRole Role => AgreementRole.TechnicalSpecialist;
    public override string DisplayName => "Технический специалист";

    public ObservableCollection<string> Options { get; } = new()
    {
        "Низкий", "Средний", "Высокий"
    };

    public string Parameter1
    {
        get => _model.Parameter1;
        set { if (_model.Parameter1 == value) return; _model.Parameter1 = value; OnPropertyChanged(); }
    }

    public string Parameter2
    {
        get => _model.Parameter2;
        set { if (_model.Parameter2 == value) return; _model.Parameter2 = value; OnPropertyChanged(); }
    }

    public string Parameter3
    {
        get => _model.Parameter3;
        set { if (_model.Parameter3 == value) return; _model.Parameter3 = value; OnPropertyChanged(); }
    }

    public string SelectedOption
    {
        get => _model.SelectedOption;
        set { if (_model.SelectedOption == value) return; _model.SelectedOption = value; OnPropertyChanged(); }
    }

    public TechnicalSpecialistAgreementViewModel(TechnicalSpecialistBlock model) : base(model)
        => _model = model;

    public override void LoadFrom(IReadOnlyDictionary<string, object> data)
    {
        base.LoadFrom(data);
        OnPropertyChanged(nameof(Parameter1));
        OnPropertyChanged(nameof(Parameter2));
        OnPropertyChanged(nameof(Parameter3));
        OnPropertyChanged(nameof(SelectedOption));
    }
}
