using CommunityToolkit.Mvvm.ComponentModel;
using ProjectName.Models;

namespace ProjectName.Wpf.ViewModels.AgreementBlocks;

/// <summary>
/// VM-обёртка над доменным IAgreementBlock. Чтение/запись идут напрямую в модель,
/// поэтому отдельная синхронизация не нужна.
/// </summary>
public abstract partial class AgreementBlockViewModel : ObservableObject, IAgreementBlock
{
    private readonly IAgreementBlock _model;

    public abstract AgreementRole Role { get; }
    public abstract string DisplayName { get; }

    public bool IsAgreed
    {
        get => _model.IsAgreed;
        set
        {
            if (_model.IsAgreed == value) return;
            _model.IsAgreed = value;
            OnPropertyChanged();
        }
    }

    public string Comment
    {
        get => _model.Comment;
        set
        {
            if (_model.Comment == value) return;
            _model.Comment = value;
            OnPropertyChanged();
        }
    }

    protected AgreementBlockViewModel(IAgreementBlock model) => _model = model;

    public Dictionary<string, object> ToDictionary() => _model.ToDictionary();

    public void LoadFrom(IReadOnlyDictionary<string, object> data)
    {
        _model.LoadFrom(data);
        OnPropertyChanged(nameof(IsAgreed));
        OnPropertyChanged(nameof(Comment));
    }
}