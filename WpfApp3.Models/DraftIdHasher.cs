using System.Security.Cryptography;
using System.Text;

namespace ProjectName.Models;

/// <summary>
/// Детерминированный Guid для имени файла черновика пары.
/// DraftId = MD5("{calculationId}|{stageId}|{taskId}") → 16 байт → Guid.
/// </summary>
public static class DraftIdHasher
{
    public static Guid Compute(Guid calculationId, long stageId, long taskId)
    {
        var input = $"{calculationId:D}|{stageId}|{taskId}";
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = MD5.HashData(bytes);
        return new Guid(hash);
    }
}