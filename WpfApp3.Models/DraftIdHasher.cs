using System.Security.Cryptography;
using System.Text;

namespace ProjectName.Models;

/// <summary>
/// Детерминированный Guid для имени файла черновика пары.
/// DraftId = MD5("{calculationId}|{stageId}|{taskId}") → 16 байт → Guid.
/// </summary>
public static class DraftIdHasher
{
    /// <summary>DraftId расчёта. Детерминирован от ProjectId.</summary>
    public static Guid ComputeCalculationDraftId(long projectId)
        => Compute($"calculation|{projectId}");

    /// <summary>DraftId пары. Детерминирован от CalculationDraftId + Stage + Task.</summary>
    public static Guid ComputePairDraftId(Guid calculationDraftId, long stageId, long taskId)
        => Compute($"pair|{calculationDraftId:D}|{stageId}|{taskId}");

    private static Guid Compute(string input)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = System.Security.Cryptography.MD5.HashData(bytes);
        return new Guid(hash);
    }
}