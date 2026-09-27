namespace ProjectName.Models;

public enum ProjectStatus { NotStarted, InProgress, Completed, OnHold, Cancelled }
public enum StageStatus { NotStarted, InProgress, Completed, OnHold, Cancelled }
public enum TaskStatus { NotStarted, InProgress, Completed, Blocked }

public enum AgreementRole
{
    TechnicalSpecialist,
    GroupLead,
    BlockLead
}