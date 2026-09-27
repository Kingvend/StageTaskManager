using System;
using System.Collections.Generic;
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
    private bool _suppressSync;   // чтобы не дёргать SyncToApprovals во время InitializeAsync

    public Stage Stage { get; }
    public ProjectTask Task { get; }

    public string StageName => Stage.Name;
    public string TaskName => Task.Name;

    public TechnicalSpecialistAgreementViewModel TechnicalSpecialist { get; }
    public GroupLeadAgreementViewModel GroupLead { get; }
    public BlockLeadAgreementViewModel BlockLead { get; }

    public IReadOnlyList<IAgreementBlock> Blocks { get; }

    [ObservableProperty]
    private IAgreementBlock? _selectedBlock;

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

        TechnicalSpecialist = new TechnicalSpecialistAgreementViewModel(FindApproval(AgreementRole.TechnicalSpecialist));
        GroupLead = new GroupLeadAgreementViewModel(FindApproval(AgreementRole.GroupLead));
        BlockLead = new BlockLeadAgreementViewModel(FindApproval(AgreementRole.BlockLead));

        Blocks = new IAgreementBlock[] { TechnicalSpecialist, GroupLead, BlockLead };
        SelectedBlock = Blocks[0];

        foreach (var block in Blocks)
            ((ObservableObject)block).PropertyChanged += OnBlockPropertyChanged;

        RecalculateCanConfirm();
    }

    private ApprovalInfo? FindApproval(AgreementRole role)
        => Task.Approvals.FirstOrDefault(a => a.Role == role);

    // ----- Инициализация: подтягиваем черновик, если в памяти данных нет -----

    /// <summary>
    /// Вызывается из окна после Loaded. Логика:
    /// 1. Если в памяти (Task.Approvals) уже есть данные — используем их (это сессионное состояние).
    /// 2. Иначе пробуем прочитать {pairGuid}.json — данные попадают и в блоки, и в Task.Approvals.
    /// 3. Иначе оставляем пустые значения (в будущем здесь будет загрузка из БД).
    /// </summary>
    public async Task InitializeAsync()
    {
        if (Task.Approvals.Count > 0) return;

        var draft = await _draftStorage.TryLoadPairAsync(_context.CurrentProject.Id, Task.PairId);
        if (draft is null || draft.Blocks.Count == 0) return;

        _suppressSync = true;
        foreach (var block in Blocks)
        {
            if (draft.Blocks.TryGetValue(block.Role.ToString(), out var data) &&
                data is Dictionary<string, object> dict)
            {
                block.LoadFrom(dict);
            }
        }
        _suppressSync = false;

        SyncToApprovals();
    }

    // ----- Реакция на изменения во вкладках -----

    private void OnBlockPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IAgreementBlock.IsAgreed))
            RecalculateCanConfirm();

        if (!_suppressSync)
            SyncToApprovals();
    }

    private void RecalculateCanConfirm()
        => CanConfirm = Blocks.All(b => b.IsAgreed);

    /// <summary>Переносит состояние блоков в Task.Approvals (in-memory persist).</summary>
    private void SyncToApprovals()
    {
        Task.Approvals.Clear();
        foreach (var block in Blocks)
        {
            var d = block.ToDictionary();
            Task.Approvals.Add(new ApprovalInfo
            {
                Role = block.Role,
                IsAgreed = (bool)d["IsAgreed"],
                Comment = (string)d["Comment"],
            });
        }
    }

    // ----- Команды -----

    [RelayCommand]
    private async Task SaveDraftAsync()
    {
        var savedAt = DateTimeOffset.UtcNow;
        SyncToApprovals();
        await _draftStorage.SaveProjectAsync(_context.CurrentProject, savedAt);
        await _draftStorage.SavePairAsync(_context.CurrentProject, Stage, Task, Blocks, savedAt);
    }

    [RelayCommand(CanExecute = nameof(CanConfirm))]
    private void Confirm()
    {
        SyncToApprovals();
        Task.Status = TaskStatus.Completed;
        RequestClose?.Invoke(this, true);
    }

    // ----- Автосохранение при закрытии окна -----

    /// <summary>
    /// Синхронный вызов из Window.Closing. Не используем async — WPF Closing не поддерживает await.
    /// Файлы маленькие, локальная запись быстрая.
    /// </summary>
    public void SaveDraftOnClose()
    {
        try
        {
            var savedAt = DateTimeOffset.UtcNow;
            SyncToApprovals();

            _draftStorage.SaveProjectAsync(_context.CurrentProject, savedAt).GetAwaiter().GetResult();
            _draftStorage.SavePairAsync(_context.CurrentProject, Stage, Task, Blocks, savedAt).GetAwaiter().GetResult();
        }
        catch
        {
            // По требованию — пока не уведомляем пользователя.
        }
    }
}