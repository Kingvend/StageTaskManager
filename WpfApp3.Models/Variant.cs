namespace ProjectName.Models;

public sealed class Variant : IEquatable<Variant>
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }

    public bool Equals(Variant? other) => other is not null && Id == other.Id;
    public override bool Equals(object? obj) => Equals(obj as Variant);
    public override int GetHashCode() => Id.GetHashCode();
}