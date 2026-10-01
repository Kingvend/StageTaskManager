using ProjectName.Models;
using ProjectName.Models.Drafts;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectName.Services;

public class LocalJsonDraftStorage : IDraftStorage
{
    private const string AppFolderName = "ProjectName";   // папка программы в «Документах»
    private const string DraftsFolderName = "drafts";
    private const string ProjectFileName = "project.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter() }
    };

    private static string RootFolder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        AppFolderName,
        DraftsFolderName);

    private static string ProjectFolder(Guid projectId)
        => Path.Combine(RootFolder, projectId.ToString("D"));

    public async Task SaveProjectAsync(Project project, DateTimeOffset savedAt, CancellationToken ct = default)
    {
        var folder = ProjectFolder(project.Id);
        Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, ProjectFileName);

        // project.json пишется один раз — при первом сохранении черновика.
        if (File.Exists(path)) return;

        var dto = new ProjectDraftDto
        {
            ProjectId = project.Id,
            SavedAt = savedAt,
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Status = project.Status,
            Responsible = project.Responsible,
            AvailableVariants = project.AvailableVariants
        .Select(v => new VariantSnapshot { Id = v.Id, Name = v.Name, Order = v.Order })
        .ToList(),
        };

        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, dto, JsonOptions, ct);
    }

    public async Task SavePairAsync(
    Project project,
    Stage stage,
    ProjectTask task,
    DateTimeOffset savedAt,
    CancellationToken ct = default)
    {
        var folder = ProjectFolder(project.Id);
        Directory.CreateDirectory(folder);

        var variantsDict = task.Variants.ToDictionary(
            keySelector: kv => kv.Key.Id.ToString("D"),
            elementSelector: kv => new VariantDraftDto
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
            PairId = task.PairId,
            ProjectId = project.Id,
            SavedAt = savedAt,
            Stage = new StageSnapshot { Name = stage.Name, OrderNumber = stage.OrderNumber },
            Task = new TaskSnapshot
            {
                Name = task.Name,
                Description = task.Description,
                Status = task.Status,
            },
            Variants = variantsDict,
        };

        var path = Path.Combine(folder, $"{task.PairId:D}.json");

        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, dto, JsonOptions, ct).ConfigureAwait(false);
    }

    public async Task<PairDraftDto?> TryLoadPairAsync(
    Guid projectId, Guid pairId, CancellationToken ct = default)
    {
        var path = Path.Combine(ProjectFolder(projectId), $"{pairId:D}.json");
        if (!File.Exists(path)) return null;

        await using var stream = File.OpenRead(path);
        var dto = await JsonSerializer.DeserializeAsync<PairDraftDto>(stream, JsonOptions, ct)
            .ConfigureAwait(false);
        if (dto is null) return null;

        foreach (var vd in dto.Variants.Values)
        {
            vd.Blocks = vd.Blocks.ToDictionary(
                kv => kv.Key,
                kv => kv.Value is JsonElement el
                    ? (object)ConvertBlock(el)
                    : kv.Value);
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