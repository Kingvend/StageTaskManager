using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectName.Models;
using ProjectName.Models.Drafts;
using ProjectName.Services;
using ProjectName.Wpf.ViewModels.AgreementBlocks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectName.Wpf.ViewModels;

public partial class TaskDetailsViewModel : ObservableObject
{
    private readonly ICalculationContext _context;
    private readonly ICalculationService _calculationService;
    private readonly IDraftStorage _draftStorage;

    private bool _initialized;

    public StageTaskPair Pair { get; }

    public string StageName => Pair.Stage.Name;
    public string TaskName => Pair.Task.Name;

    public ObservableCollection<RoleTabViewModel> RoleTabs { get; } = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    private bool _canConfirm;

    public event EventHandler<bool>? RequestClose;

    public TaskDetailsViewModel(
        ICalculationContext context,
        ICalculationService calculationService,
        IDraftStorage draftStorage,
        StageTaskPair pair)
    {
        _context = context;
        _calculationService = calculationService;
        _draftStorage = draftStorage;
        Pair = pair;

        BuildTabs();
        RecalculateCanConfirm();
    }

    // ----- Сборка вкладок -----

    private void BuildTabs()
    {
        var roles = new (AgreementRole Role, string Display)[]
        {
            (AgreementRole.TechnicalSpecialist, "Технический специалист"),
            (AgreementRole.GroupLead,           "Руководитель группы"),
            (AgreementRole.BlockLead,           "Руководитель блока"),
        };

        foreach (var (role, display) in roles)
        {
            var roleTab = new RoleTabViewModel(role, display);

            foreach (var variant in Pair.AvailableVariants.OrderBy(v => v.Order))
            {
                // Гарантируем, что под этот вариант в памяти есть TaskModel и блок роли.
                if (!Pair.VariantData.TryGetValue(variant, out var taskModel))
                {
                    taskModel = new TaskModel { Variant = variant };
                    Pair.VariantData[variant] = taskModel;
                }
                if (!taskModel.Blocks.TryGetValue(role, out var modelBlock))
                {
                    modelBlock = new AgreementBlock { Role = role };
                    taskModel.Blocks[role] = modelBlock;
                }

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

    // ----- Реакция на изменения -----

    private void OnBlockPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IAgreementBlock.IsAgreed))
            RecalculateCanConfirm();

        // Изменились данные пары → статус переходит в InProgress, если был Completed или NotStarted.
        if (Pair.Status != PairStatus.InProgress)
            Pair.Status = PairStatus.InProgress;
    }

    private void RecalculateCanConfirm()
        => CanConfirm = RoleTabs.Count > 0
                     && RoleTabs.All(t => t.Variants.Count > 0)
                     && RoleTabs.SelectMany(t => t.Variants).All(v => v.Block.IsAgreed);

    private bool HasDataInMemory() =>
        Pair.VariantData.Values.Any(vm =>
            vm.Blocks.Values.Any(b => b.IsAgreed || !string.IsNullOrEmpty(b.Comment)));

    // ----- Ленивая инициализация -----

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        _initialized = true;

        // In-memory всегда побеждает: если в сессии уже что-то заполнено — не трогаем.
        if (HasDataInMemory()) return;

        // Из БД (ленивая загрузка пары с VariantData).
        StageTaskPair? fromDb = null;
        if (Pair.Id != Guid.Empty)
            fromDb = await _calculationService.GetPairAsync(Pair.Id);

        // Из черновика.
        PairDraftDto? draft = null;
        if (Pair.CalculationId is not null && Pair.DraftId is not null)
            draft = await _draftStorage.TryLoadPairAsync(Pair.CalculationId.Value, Pair.DraftId.Value);

        // Выбор более свежего источника.
        if (fromDb is not null && draft is not null)
        {
            if ((draft.SavedAt) >= (fromDb.UpdatedAt ?? DateTimeOffset.MinValue))
                ApplyDraft(draft);
            else
                ApplyDbPair(fromDb);
        }
        else if (draft is not null)
        {
            ApplyDraft(draft);
        }
        else if (fromDb is not null)
        {
            ApplyDbPair(fromDb);
        }

        RecalculateCanConfirm();
    }

    private void ApplyDbPair(StageTaskPair source)
    {
        foreach (var (variant, taskModel) in source.VariantData)
        {
            if (!Pair.VariantData.TryGetValue(variant, out var target)) continue;
            foreach (var (role, block) in taskModel.Blocks)
            {
                if (!target.Blocks.TryGetValue(role, out var targetBlock)) continue;
                targetBlock.LoadFrom(block.ToDictionary());
            }
        }

        // Обновим VM-обёртки.
        ReloadBlockViewModels();
    }

    private void ApplyDraft(PairDraftDto dto)
    {
        foreach (var roleTab in RoleTabs)
        {
            foreach (var variantBlock in roleTab.Variants)
            {
                if (!dto.Variants.TryGetValue(variantBlock.Variant.Id, out var vd)) continue;
                if (!vd.Blocks.TryGetValue(roleTab.Role.ToString(), out var data)) continue;
                if (data is not Dictionary<string, object> dict) continue;

                variantBlock.Block.LoadFrom(dict);
            }
        }
    }

    private void ReloadBlockViewModels()
    {
        foreach (var roleTab in RoleTabs)
            foreach (var variantBlock in roleTab.Variants)
            {
                variantBlock.Block.LoadFrom(variantBlock.Block.ToDictionary());
            }
    }

    // ----- Команды -----

    /// <summary>Сохраняет черновик пары. Доступно, только если расчёт сохранён в БД.</summary>
    [RelayCommand]
    private async Task SaveDraftAsync()
    {
        if (Pair.CalculationId is null || Pair.DraftId is null) return;
        if (_context.Current is null) return;

        var savedAt = DateTimeOffset.UtcNow;

        // calculation.json — создаётся один раз, при первом сохранении черновика.
        // Метод внутри уже проверяет File.Exists и не перезаписывает существующий файл.
        await _draftStorage.SaveCalculationAsync(_context.Current, savedAt);

        // {PairDraftId}.json — перезаписывается каждый раз.
        await _draftStorage.SavePairAsync(Pair, savedAt);
    }

    [RelayCommand(CanExecute = nameof(CanConfirm))]
    private async Task ConfirmAsync()
    {
        await SaveDraftAsync();
        Pair.Status = PairStatus.Completed;
        RequestClose?.Invoke(this, true);
    }
}