namespace ProjectName.Models;

/// <summary>
/// Задача из внешней БД. Имя класса <c>Task</c> затеняет <c>System.Threading.Tasks.Task</c>,
/// поэтому в местах, где нужны оба, используется полное имя <c>System.Threading.Tasks.Task</c>.
/// </summary>
public class ProjectTask
{
    public long Id { get; set; }
    public long StageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}