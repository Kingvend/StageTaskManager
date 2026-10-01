using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ProjectName.Models;

namespace ProjectName.Wpf.ViewModels;

public partial class RoleTabViewModel : ObservableObject
{
    public AgreementRole Role { get; }
    public string DisplayName { get; }

    public ObservableCollection<VariantBlockViewModel> Variants { get; } = new();

    public RoleTabViewModel(AgreementRole role, string displayName)
    {
        Role = role;
        DisplayName = displayName;
    }
}