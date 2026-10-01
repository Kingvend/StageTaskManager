using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectName.Models;
using ProjectName.Services;
using ProjectName.Wpf.ViewModels.AgreementBlocks;
using TaskStatus = ProjectName.Models.TaskStatus;

namespace ProjectName.Wpf.ViewModels;

public partial class TaskDetailsViewModel : ObservableObject
{
    private readonly IProjectContext _context;
    private readonly IDraftStorage _draftStorage;

    public Stage Stage { get; }
    public ProjectTask Task { get; }

    public string StageName => Stage.Name;
    public string TaskName => Task.Name;

    public ObservableCollection<RoleTabViewModel> RoleTabs { get; } = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    private bool _canConfirm;

    public event EventHandler<bool>? RequestClose;

    public TaskDetailsViewModel(
        IProjectContext context,
        IDraftStorage draftStorage,
        Stage stage,
        ProjectTask task)
    {
        _context = context;
        _draftStorage = draftStorage;
        Stage = stage;
        Task = task;

        BuildTabs();
        RecalculateCanConfirm();
    }

    private void BuildTabs()
    {
        // Порядок ролей в UI — фиксированный.
        var roles = new (AgreementRole Role, string Display)[]
        {
            (AgreementRole.TechnicalSpecialist, "Технический специалист"),
            (AgreementRole.GroupLead,           "Руководитель группы"),
            (AgreementRole.BlockLead,           "Руководитель блока"),
        };

        foreach (var (role, display) in roles)
        {
            var roleTab = new RoleTabViewModel(role, display);

            foreach (var variant in _context.CurrentProject.AvailableVariants.OrderBy(v => v.Order))
            {
                if (!Task.Variants.TryGetValue(variant, out var taskModel)) continue;
                if (!taskModel.Blocks.TryGetValue(role, out var modelBlock)) continue;

                var vmBlock = CreateBlockVm(role, modelBlock);
                ((ObservableObject)vmBlock).PropertyChanged += OnBlockPropertyChanged;

                roleTab.Variants.Add(new VariantBlockViewModel(variant, vmBlock));
            }

            RoleTabs.Add(roleTab);
        }
    }

    private static AgreementBlockViewModel CreateBlockVm(AgreementRole role, IAgreementBlock model) => role switch
    {
        AgreementRole.TechnicalSpecialist => new TechnicalSpecialistAgreementViewModel(model),
        AgreementRole.GroupLead => new GroupLeadAgreementViewModel(model),
        AgreementRole.BlockLead => new BlockLeadAgreementViewModel(model),
        _ => throw new ArgumentOutOfRangeException(nameof(role)),
    };

    private void OnBlockPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IAgreementBlock.IsAgreed))
            RecalculateCanConfirm();
    }

    private void RecalculateCanConfirm()
    {
        var anyBlocks = RoleTabs.Count > 0 && RoleTabs.All(t => t.Variants.Count > 0);
        CanConfirm = anyBlocks && RoleTabs
            .SelectMany(t => t.Variants)
            .All(v => v.Block.IsAgreed);
    }

    // ----- Инициализация: подтягиваем черновик, если в памяти пусто -----

    public async Task InitializeAsync()
    {
        if (HasAnyDataInMemory()) return;

        var draft = await _draftStorage.TryLoadPairAsync(_context.CurrentProject.Id, Task.PairId);
        if (draft is null || draft.Variants.Count == 0) return;

        foreach (var roleTab in RoleTabs)
        {
            foreach (var variantBlock in roleTab.Variants)
            {
                var variantKey = variantBlock.Variant.Id.ToString("D");
                if (!draft.Variants.TryGetValue(variantKey, out var vd)) continue;

                var roleKey = roleTab.Role.ToString();
                if (!vd.Blocks.TryGetValue(roleKey, out var data)) continue;
                if (data is not Dictionary<string, object> dict) continue;

                variantBlock.Block.LoadFrom(dict);
            }
        }

        RecalculateCanConfirm();
    }

    private bool HasAnyDataInMemory()
        => Task.Variants.Values.Any(vm =>
               vm.Blocks.Values.Any(b => b.IsAgreed || !string.IsNullOrEmpty(b.Comment)));

    // ----- Команды -----

    [RelayCommand]
    private async Task SaveDraftAsync()
    {
        var savedAt = DateTimeOffset.UtcNow;
        await _draftStorage.SaveProjectAsync(_context.CurrentProject, savedAt);
        await _draftStorage.SavePairAsync(_context.CurrentProject, Stage, Task, savedAt);
    }

    [RelayCommand(CanExecute = nameof(CanConfirm))]
    private void Confirm()
    {
        Task.Status = TaskStatus.Completed;
        RequestClose?.Invoke(this, true);
    }
}