using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectName.Models;
using ProjectName.Services;

namespace ProjectName.Wpf.ViewModels;

public partial class StageTaskPairViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;
    private readonly ICalculationService _calculationService;

    public StageTaskPair Pair { get; }

    public string StageName => Pair.Stage.Name;
    public string TaskName => Pair.Task.Name;
    public PairStatus Status => Pair.Status;

    public event EventHandler? StatusChanged;

    public StageTaskPairViewModel(
        StageTaskPair pair,
        IDialogService dialogService,
        ICalculationService calculationService)
    {
        Pair = pair;
        _dialogService = dialogService;
        _calculationService = calculationService;
    }

    [RelayCommand]
    private async Task OpenDetailsAsync()
    {
        _dialogService.ShowTaskDetails(Pair);

        OnPropertyChanged(nameof(Status));
        SavePairCommand.NotifyCanExecuteChanged();
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool CanSavePair() => Pair.Status == PairStatus.Completed;

    [RelayCommand(CanExecute = nameof(CanSavePair))]
    private async Task SavePairAsync()
    {
        await _calculationService.SavePairAsync(Pair);
        _dialogService.ShowMessage(
            $"Данные пары «{Pair.Stage.Name} / {Pair.Task.Name}» сохранены в БД.",
            "Сохранение");
    }
}