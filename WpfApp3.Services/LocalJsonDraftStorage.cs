using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectName.Models;
using ProjectName.Models.Drafts;

namespace ProjectName.Services;

public class LocalJsonDraftStorage : IDraftStorage
{
    private const string AppFolderName = "ProjectName";
    private const string DraftsFolderName = "drafts";
    private const string CalculationFile = "calculation.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter() }
    };

    private static string RootFolder =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            AppFolderName,
            DraftsFolderName);

    private static string CalculationFolder(long projectId)
    {
        var draftId = DraftIdHasher.ComputeCalculationDraftId(projectId);
        return Path.Combine(RootFolder, draftId.ToString("D"));
    }

    // ----- Save: calculation -----

    public async Task SaveCalculationAsync(
        ProjectCalculation calculation, DateTimeOffset savedAt, CancellationToken ct = default)
    {
        var projectId = calculation.Project.Id;
        var draftId = DraftIdHasher.ComputeCalculationDraftId(projectId);

        var folder = CalculationFolder(projectId);
        Directory.CreateDirectory(folder);

        var dto = new CalculationDraftDto
        {
            DraftId = draftId,
            CalculationId = calculation.Id ?? Guid.Empty,
            SavedAt = savedAt,
            Project = new ProjectSnapshot { /* … */ },
            AvailableVariants = calculation.AvailableVariants
                .OrderBy(v => v.Order)
                .Select(v => new VariantSnapshot { Id = v.Id, Name = v.Name, Order = v.Order })
                .ToList(),
        };

        var path = Path.Combine(folder, CalculationFile);

        // Файл не перезаписываем, если он уже есть, НО синхронизируем CalculationId —
        // он может появиться позже (после «Сохранить расчёт»).
        if (File.Exists(path))
        {
            await using var read = File.OpenRead(path);
            var existing = await JsonSerializer
                .DeserializeAsync<CalculationDraftDto>(read, JsonOptions, ct).ConfigureAwait(false);
            if (existing is not null && existing.CalculationId == dto.CalculationId)
                return;
        }

        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, dto, JsonOptions, ct).ConfigureAwait(false);
    }

    // ----- Save: pair -----

    public async Task SavePairAsync(
        ProjectCalculation calculation, StageTaskPair pair,
        DateTimeOffset savedAt, CancellationToken ct = default)
    {
        var projectId = calculation.Project.Id;
        var calculationDraftId = DraftIdHasher.ComputeCalculationDraftId(projectId);
        var pairDraftId = DraftIdHasher.ComputePairDraftId(
            calculationDraftId, pair.StageId, pair.TaskId);

        var folder = CalculationFolder(projectId);
        Directory.CreateDirectory(folder);

        var variantsDict = pair.VariantData.ToDictionary(
            kv => kv.Key.Id,
            kv => new VariantDraftDto
            {
                Id = kv.Key.Id,
                Name = kv.Key.Name,
                Order = kv.Key.Order,
                Blocks = kv.Value.Blocks.ToDictionary(
                    bk => bk.Key.ToString(),
                    bk => (object)bk.Value.ToDictionary()),
            });

        var dto = new PairDraftDto
        {
            PairId = pair.Id,
            DraftId = pairDraftId,
            CalculationId = calculation.Id ?? Guid.Empty,
            SavedAt = savedAt,
            Status = pair.Status,
            Stage = new StageSnapshot
            {
                Id = pair.Stage.Id,
                Name = pair.Stage.Name,
                OrderNumber = pair.Stage.OrderNumber,
            },
            Task = new TaskSnapshot
            {
                Id = pair.Task.Id,
                Name = pair.Task.Name,
                Description = pair.Task.Description,
            },
            Variants = variantsDict,
        };

        var path = Path.Combine(folder, $"{pairDraftId:D}.json");
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, dto, JsonOptions, ct).ConfigureAwait(false);
    }

    // ----- Load: calculation -----

    public async Task<CalculationDraftDto?> TryLoadCalculationAsync(
        long projectId, CancellationToken ct = default)
    {
        var expectedDraftId = DraftIdHasher.ComputeCalculationDraftId(projectId);
        var path = Path.Combine(CalculationFolder(projectId), CalculationFile);
        if (!File.Exists(path)) return null;

        await using var stream = File.OpenRead(path);
        var dto = await JsonSerializer
            .DeserializeAsync<CalculationDraftDto>(stream, JsonOptions, ct).ConfigureAwait(false);

        // Защита от устаревших файлов после смены алгоритма хеширования.
        if (dto is null || dto.DraftId != expectedDraftId) return null;
        return dto;
    }

    // ----- Load: pair -----

    public async Task<PairDraftDto?> TryLoadPairAsync(
        long projectId, long stageId, long taskId, CancellationToken ct = default)
    {
        var calculationDraftId = DraftIdHasher.ComputeCalculationDraftId(projectId);
        var pairDraftId = DraftIdHasher.ComputePairDraftId(calculationDraftId, stageId, taskId);

        var path = Path.Combine(CalculationFolder(projectId), $"{pairDraftId:D}.json");
        if (!File.Exists(path)) return null;

        await using var stream = File.OpenRead(path);
        var dto = await JsonSerializer
            .DeserializeAsync<PairDraftDto>(stream, JsonOptions, ct).ConfigureAwait(false);
        if (dto is null) return null;

        foreach (var vd in dto.Variants.Values)
            vd.Blocks = vd.Blocks.ToDictionary(
                kv => kv.Key,
                kv => kv.Value is JsonElement el ? (object)ConvertBlock(el) : kv.Value);

        return dto;
    }

    private static Dictionary<string, object> ConvertBlock(JsonElement el)
    {
        var result = new Dictionary<string, object>();
        foreach (var prop in el.EnumerateObject())
        {
            result[prop.Name] = prop.Value.ValueKind switch
            {
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.String => prop.Value.GetString() ?? string.Empty,
                JsonValueKind.Number => prop.Value.TryGetInt64(out var l) ? l : prop.Value.GetDouble(),
                JsonValueKind.Null => string.Empty,
                _ => prop.Value.GetRawText()
            };
        }
        return result;
    }
}