namespace ProjectName.Models;

public enum ProjectStatus { NotStarted, InProgress, Completed, OnHold, Cancelled }
public enum StageStatus { NotStarted, InProgress, Completed, OnHold, Cancelled }

/// <summary>Статус согласования пары «этап-задача».</summary>
public enum PairStatus { NotStarted, InProgress, Completed }

/// <summary>Роль, от имени которой выступает вкладка согласования.</summary>
public enum AgreementRole
{
    TechnicalSpecialist,
    GroupLead,
    BlockLead
}