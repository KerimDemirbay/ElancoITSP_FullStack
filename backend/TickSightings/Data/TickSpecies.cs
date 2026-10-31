namespace TickSightings.Data;

public record SpeciesInfo(string Name, string LatinName);

public static class TickSpecies
{
    private static readonly SpeciesInfo[] AllSpecies =
    [
        new("Marsh tick",           "Ixodes apronophorus"),
        new("Southern rodent tick", "Ixodes acuminatus"),
        new("Passerine tick",       "Dermacentor frontalis"),
        new("Fox/badger tick",      "Ixodes canisuga"),
        new("Tree-hole tick",       "Ixodes arboricola"),
    ];

    public static IReadOnlyList<SpeciesInfo> All => AllSpecies;

    public static SpeciesInfo? Find(string? name) =>
    AllSpecies.FirstOrDefault(s => s.Name.Equals(name?.Trim(), StringComparison.OrdinalIgnoreCase));

    public static SpeciesInfo? FindByLatin(string? latinName) =>
    AllSpecies.FirstOrDefault(s => s.LatinName.Equals(latinName?.Trim(), StringComparison.OrdinalIgnoreCase));
}