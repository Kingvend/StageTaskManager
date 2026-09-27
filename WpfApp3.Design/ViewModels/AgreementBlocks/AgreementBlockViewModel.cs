using CommunityToolkit.Mvvm.ComponentModel;
using ProjectName.Models;

namespace ProjectName.Wpf.ViewModels.AgreementBlocks;

public abstract partial class AgreementBlockViewModel : ObservableObject, IAgreementBlock
{
    public abstract AgreementRole Role { get; }
    public abstract string DisplayName { get; }

    [ObservableProperty]
    private bool _isAgreed;

    [ObservableProperty]
    private string _comment = string.Empty;

    protected AgreementBlockViewModel(ApprovalInfo? existing)
    {
        if (existing is null) return;
        IsAgreed = existing.IsAgreed;
        Comment = existing.Comment;
    }

    public virtual Dictionary<string, object> ToDictionary() => new()
    {
        ["Role"] = Role.ToString(),
        ["IsAgreed"] = IsAgreed,
        ["Comment"] = Comment,
    };

    public virtual void LoadFrom(IReadOnlyDictionary<string, object> data)
    {
        if (data.TryGetValue("IsAgreed", out var ia) && ia is bool b)
            IsAgreed = b;

        if (data.TryGetValue("Comment", out var c) && c is string s)
            Comment = s;
    }
}