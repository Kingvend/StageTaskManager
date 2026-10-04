using ProjectName.Models;

namespace ProjectName.Services;

/// <summary>
/// Общий контекст приложения: хранит активный расчёт,
/// чтобы все ViewModel'и работали с одними и теми же экземплярами.
/// </summary>
public interface ICalculationContext
{
    ProjectCalculation? Current { get; }
    void Set(ProjectCalculation calculation);
    void Clear();
}