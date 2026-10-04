using ProjectName.Models;

namespace ProjectName.Wpf.ViewModels.AgreementBlocks;

public sealed class TechnicalSpecialistAgreementViewModel : AgreementBlockViewModel
{
    public override AgreementRole Role => AgreementRole.TechnicalSpecialist;
    public override string DisplayName => "Технический специалист";

    public TechnicalSpecialistAgreementViewModel(IAgreementBlock model) : base(model) { }
}

public sealed class GroupLeadAgreementViewModel : AgreementBlockViewModel
{
    public override AgreementRole Role => AgreementRole.GroupLead;
    public override string DisplayName => "Руководитель группы";

    public GroupLeadAgreementViewModel(IAgreementBlock model) : base(model) { }
}

public sealed class BlockLeadAgreementViewModel : AgreementBlockViewModel
{
    public override AgreementRole Role => AgreementRole.BlockLead;
    public override string DisplayName => "Руководитель блока";

    public BlockLeadAgreementViewModel(IAgreementBlock model) : base(model) { }
}