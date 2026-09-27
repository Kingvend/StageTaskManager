using ProjectName.Models;

namespace ProjectName.Wpf.ViewModels.AgreementBlocks;

public sealed class BlockLeadAgreementViewModel : AgreementBlockViewModel
{
    public override AgreementRole Role => AgreementRole.BlockLead;
    public override string DisplayName => "Руководитель блока";

    public BlockLeadAgreementViewModel(ApprovalInfo? existing = null) : base(existing) { }
}