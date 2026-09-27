using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectName.Models;
using ProjectName.Services;
using TaskStatus = ProjectName.Models.TaskStatus;

namespace ProjectName.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IProjectService _projectService;
    private readonly IProjectContext _context;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private Project _currentProject = new();

    public ObservableCollection<StageTaskPairViewModel> TaskItems { get; } = new();

    public MainViewModel(IProjectService projectService,
                         IProjectContext context,
                         IDialogService dialogService)
    {
        _projectService = projectService;
        _context = context;
        _dialogService = dialogService;
    }

    // ----- Load -----

    [RelayCommand]
    private async Task LoadAsync()
    {
        CurrentProject = _projectService.GetCurrentProject();
        _context.Initialize(CurrentProject);

        TaskItems.Clear();
        foreach (var stage in CurrentProject.Stages.OrderBy(s => s.OrderNumber))
        {
            foreach (var task in stage.Tasks)
            {
                var pair = new StageTaskPairViewModel(stage, task, _dialogService);
                pair.StatusChanged += OnTaskStatusChanged;
                TaskItems.Add(pair);
            }
        }

        // После загрузки сразу пересчитаем доступность Save.
        SaveCommand.NotifyCanExecuteChanged();

        await Task.CompletedTask;
    }

    // ----- Save -----

    /// <summary>Кнопка активна, только если все задачи проекта в статусе Completed.</summary>
    private bool CanSave()
    {
        var allTasks = CurrentProject.Stages.SelectMany(s => s.Tasks).ToList();
        return allTasks.Count > 0 && allTasks.All(t => t.Status == TaskStatus.Completed);
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        // TODO: логика сохранения пока пропущена.
        await _projectService.SaveAsync(CurrentProject);
    }

    // ----- Реакция на изменение статуса задачи -----

    private void OnTaskStatusChanged(object? sender, EventArgs e)
        => SaveCommand.NotifyCanExecuteChanged();
}