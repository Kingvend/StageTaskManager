using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectName.Models;
using ProjectName.Services;

namespace ProjectName.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IExternalCatalogService _catalog;
    private readonly ICalculationService _calculationService;
    private readonly ICalculationContext _context;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ProjectCalculation? _currentCalculation;

    public ObservableCollection<StageTaskPairViewModel> PairItems { get; } = new();

    public MainViewModel(
        IExternalCatalogService catalog,
        ICalculationService calculationService,
        ICalculationContext context,
        IDialogService dialogService)
    {
        _catalog = catalog;
        _calculationService = calculationService;
        _context = context;
        _dialogService = dialogService;
    }

    // ----- Load -----

    [RelayCommand]
    private async Task LoadAsync()
    {
        // Демо: фиксированный projectId; в реальности — из аргументов/внешнего сервиса.
        const long projectId = 101;
        var variantIds = new long[] { 1001, 1002, 1003 };

        // 1. Внешние данные.
        var project = await _catalog.GetProjectAsync(projectId);
        if (project is null) return;

        var variants = await _catalog.GetVariantsAsync(variantIds);

        // 2. Расчёт из нашей БД.
        var calculation = await _calculationService.GetByProjectAsync(projectId);
        if (calculation is null)
        {
            calculation = new ProjectCalculation
            {
                Id = null,
                Project = project,
                AvailableVariants = variants.ToList(),
            };
            await FillPairsFromExternalAsync(calculation, variants);
        }
        else
        {
            // Обновляем кэш внешних данных.
            calculation.Project = project;
            calculation.AvailableVariants = variants.ToList();
            await MergePairsFromExternalAsync(calculation, variants);
        }

        _context.Set(calculation);
        CurrentCalculation = calculation;

        RebuildPairItems(calculation);
        SendForReviewCommand.NotifyCanExecuteChanged();
    }

    private async Task FillPairsFromExternalAsync(ProjectCalculation calc, IReadOnlyList<Variant> variants)
    {
        var stages = await _catalog.GetStagesAsync(calc.Project.Id);
        foreach (var stage in stages.OrderBy(s => s.OrderNumber))
        {
            var tasks = await _catalog.GetTasksAsync(stage.Id);
            foreach (var task in tasks)
            {
                calc.Pairs.Add(CreatePair(calc, stage, task, variants));
            }
        }
    }

    private async Task MergePairsFromExternalAsync(ProjectCalculation calc, IReadOnlyList<Variant> variants)
    {
        var stages = await _catalog.GetStagesAsync(calc.Project.Id);
        foreach (var stage in stages.OrderBy(s => s.OrderNumber))
        {
            var tasks = await _catalog.GetTasksAsync(stage.Id);
            foreach (var task in tasks)
            {
                var existing = calc.Pairs.FirstOrDefault(
                    p => p.StageId == stage.Id && p.TaskId == task.Id);
                if (existing is null)
                    calc.Pairs.Add(CreatePair(calc, stage, task, variants));
            }
        }
    }

    private static StageTaskPair CreatePair(
        ProjectCalculation calc, Stage stage, ProjectTask task, IReadOnlyList<Variant> variants)
    {
        var pair = new StageTaskPair
        {
            Id = Guid.Empty,
            CalculationId = calc.Id,
            StageId = stage.Id,
            TaskId = task.Id,
            Stage = stage,
            Task = task,
            AvailableVariants = variants.ToList(),
            Status = PairStatus.NotStarted,
        };
        if (calc.Id is not null)
            pair.DraftId = DraftIdHasher.Compute(calc.Id.Value, stage.Id, task.Id);
        return pair;
    }

    private void RebuildPairItems(ProjectCalculation calc)
    {
        PairItems.Clear();
        foreach (var pair in calc.Pairs.OrderBy(p => p.Stage.OrderNumber).ThenBy(p => p.Task.Id))
        {
            var vm = new StageTaskPairViewModel(pair, _dialogService, _calculationService);
            vm.StatusChanged += OnPairStatusChanged;
            PairItems.Add(vm);
        }
    }

    private void OnPairStatusChanged(object? sender, EventArgs e)
        => SendForReviewCommand.NotifyCanExecuteChanged();

    // ----- Save calculation -----

    [RelayCommand]
    private async Task SaveCalculationAsync()
    {
        if (CurrentCalculation is null) return;

        var id = await _calculationService.SaveCalculationAsync(CurrentCalculation);

        // После сохранения расчёта можно вычислить DraftId для всех пар.
        foreach (var pair in CurrentCalculation.Pairs)
        {
            pair.CalculationId = id;
            pair.DraftId = DraftIdHasher.Compute(id, pair.StageId, pair.TaskId);
        }

        // Обновим карточки — теперь доступны «Открыть детали» и «Сохранить черновик».
        foreach (var vm in PairItems)
            vm.OpenDetailsCommand.NotifyCanExecuteChanged();
    }

    // ----- Send for review -----

    private bool CanSendForReview()
        => CurrentCalculation?.Pairs.Count > 0
        && CurrentCalculation.Pairs.All(p => p.Status == PairStatus.Completed);

    [RelayCommand(CanExecute = nameof(CanSendForReview))]
    private async Task SendForReviewAsync()
    {
        if (CurrentCalculation?.Id is null) return;

        await _calculationService.SendForReviewAsync(CurrentCalculation.Id.Value);

        _dialogService.ShowMessage(
            "Расчёт отправлен на рассмотрение.",
            "Отправка");
    }
}