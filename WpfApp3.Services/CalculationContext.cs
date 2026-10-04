using ProjectName.Models;

namespace ProjectName.Services;

public class CalculationContext : ICalculationContext
{
    public ProjectCalculation? Current { get; private set; }

    public void Set(ProjectCalculation calculation) => Current = calculation;
    public void Clear() => Current = null;
}