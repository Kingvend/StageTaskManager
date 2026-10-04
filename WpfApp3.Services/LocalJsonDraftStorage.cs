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

    private static string CalculationFolder(Guid calculationId)
        => Path.Combine(RootFolder, calculationId.ToString("D"));

    // ----- Save: calculation -----

    public async Task SaveCalculationAsync(
        ProjectCalculation calculation, DateTimeOffset savedAt, CancellationToken ct = default)
    {
        if (calculation.Id is null || calculation.Id == Guid.Empty)
            throw new InvalidOperationException(
                "Нельзя сохранить черновик расчёта, пока расчёт не сохранён в БД (Id отсутствует).");

        var folder = CalculationFolder(calculation.Id.Value);
        Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, CalculationFile);
        if (File.Exists(path)) return;

        var dto = new CalculationDraftDto
        {
            CalculationId = calculation.Id.Value,
            SavedAt = savedAt,
            Project = new ProjectSnapshot
            {
                Id = calculation.Project.Id,
                Name = calculation.Project.Name,
                Description = calculation.Project.Description,
                StartDate = calculation.Project.StartDate,
                EndDate = calculation.Project.EndDate,
                Status = calculation.Project.Status,
                Responsible = calculation.Project.Responsible,
            },
            AvailableVariants = calculation.AvailableVariants
                .OrderBy(v => v.Order)
                .Select(v => new VariantSnapshot { Id = v.Id, Name = v.Name, Order = v.Order })
                .ToList(),
        };

        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, dto, JsonOptions, ct).ConfigureAwait(false);
    }

    // ----- Save: pair -----

    public async Task SavePairAsync(
        StageTaskPair pair, DateTimeOffset savedAt, CancellationToken ct = default)
    {
        if (pair.CalculationId is null || pair.DraftId is null)
            throw new InvalidOperationException(
                "Нельзя сохранить черновик пары: расчёт ещё не сохранён в БД.");

        var folder = CalculationFolder(pair.CalculationId.Value);
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
            DraftId = pair.DraftId.Value,
            CalculationId = pair.CalculationId.Value,
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

        var path = Path.Combine(folder, $"{pair.DraftId.Value:D}.json");

        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, dto, JsonOptions, ct).ConfigureAwait(false);
    }

    // ----- Load: calculation -----

    public async Task<CalculationDraftDto?> TryLoadCalculationAsync(
        Guid calculationId, CancellationToken ct = default)
    {
        var path = Path.Combine(CalculationFolder(calculationId), CalculationFile);
        if (!File.Exists(path)) return null;

        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<CalculationDraftDto>(stream, JsonOptions, ct)
            .ConfigureAwait(false);
    }

    // ----- Load: pair -----

    public async Task<PairDraftDto?> TryLoadPairAsync(
        Guid calculationId, Guid draftId, CancellationToken ct = default)
    {
        var path = Path.Combine(CalculationFolder(calculationId), $"{draftId:D}.json");
        if (!File.Exists(path)) return null;

        await using var stream = File.OpenRead(path);
        var dto = await JsonSerializer.DeserializeAsync<PairDraftDto>(stream, JsonOptions, ct)
            .ConfigureAwait(false);
        if (dto is null) return null;

        // JsonElement → примитивы, чтобы VM-обёртки читали IsAgreed/Comment напрямую.
        foreach (var vd in dto.Variants.Values)
        {
            vd.Blocks = vd.Blocks.ToDictionary(
                kv => kv.Key,
                kv => kv.Value is JsonElement el ? (object)ConvertBlock(el) : kv.Value);
        }

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