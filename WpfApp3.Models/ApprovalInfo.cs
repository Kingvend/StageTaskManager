namespace ProjectName.Models;

public class ApprovalInfo
{
    public AgreementRole Role { get; set; }
    public bool IsAgreed { get; set; }
    public string Comment { get; set; } = string.Empty;
}