using CommunityToolkit.Mvvm.ComponentModel;
using ProjectName.Models;
using ProjectName.Wpf.ViewModels.AgreementBlocks;

namespace ProjectName.Wpf.ViewModels;

public partial class VariantBlockViewModel : ObservableObject
{
    public Variant Variant { get; }
    public string DisplayName => Variant.Name;

    public AgreementBlockViewModel Block { get; }

    public VariantBlockViewModel(Variant variant, AgreementBlockViewModel block)
    {
        Variant = variant;
        Block = block;
    }
}