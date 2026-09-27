using ProjectName.Models;

namespace ProjectName.Wpf.ViewModels.AgreementBlocks;

public sealed class TechnicalSpecialistAgreementViewModel : AgreementBlockViewModel
{
    public override AgreementRole Role => AgreementRole.TechnicalSpecialist;
    public override string DisplayName => "Технический специалист";

    public TechnicalSpecialistAgreementViewModel(ApprovalInfo? existing = null) : base(existing) { }

    // Здесь позже появятся специфичные для роли поля.
}