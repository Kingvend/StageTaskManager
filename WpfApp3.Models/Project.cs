namespace ProjectName.Models;

/// <summary>
/// Проект из внешней БД. Read-only, Id — bigint.
/// </summary>
public class Project
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ProjectStatus Status { get; set; }
    public string Responsible { get; set; } = string.Empty;
}