using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectName.Models;
using ProjectName.Services;
using TaskStatus = ProjectName.Models.TaskStatus;

namespace ProjectName.Wpf.ViewModels;

public partial class StageTaskPairViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;

    public Stage Stage { get; }
    public ProjectTask Task { get; }

    public string StageName => Stage.Name;
    public string TaskName => Task.Name;
    public TaskStatus Status => Task.Status;

    /// <summary>Уведомляет MainViewModel, что статус задачи изменился.</summary>
    public event EventHandler? StatusChanged;

    public StageTaskPairViewModel(Stage stage, ProjectTask task, IDialogService dialogService)
    {
        Stage = stage;
        Task = task;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private void OpenDetails()
    {
        if (_dialogService.ShowTaskDetails(Stage, Task))
        {
            OnPropertyChanged(nameof(Status));
            StatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}