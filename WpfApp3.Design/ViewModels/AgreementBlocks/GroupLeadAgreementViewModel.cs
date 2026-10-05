using ProjectName.Models;

namespace ProjectName.Wpf.ViewModels.AgreementBlocks;

public sealed class GroupLeadAgreementViewModel : AgreementBlockViewModel
{
    public override AgreementRole Role => AgreementRole.GroupLead;
    public override string DisplayName => "Руководитель группы";

    public GroupLeadAgreementViewModel(GroupLeadBlock model) : base(model) { }
}
