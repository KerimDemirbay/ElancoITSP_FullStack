namespace TickSightings.Data;

public record CityInfo(
    string Name,
    string Region,
    double Latitude,
    double Longitude
);
public static class UkCities
{
    private static readonly CityInfo[] AllCities =
    [
        new("London",     "London",                  51.5074, -0.1278),
        new("Manchester", "North West",              53.4808, -2.2426),
        new("Liverpool",  "North West",              53.4084, -2.9916),
        new("Birmingham", "West Midlands",           52.4862, -1.8904),
        new("Nottingham", "East Midlands",           52.9548, -1.1581),
        new("Leicester",  "East Midlands",           52.6369, -1.1398),
        new("Leeds",      "Yorkshire and the Humber",53.8008, -1.5491),
        new("Sheffield",  "Yorkshire and the Humber",53.3811, -1.4701),
        new("Newcastle",  "North East",              54.9783, -1.6178),
        new("Bristol",    "South West",              51.4545, -2.5879),
        new("Southampton","South East",               50.9097, -1.4044),
        new("Cardiff",    "Wales",                   51.4816, -3.1791),
        new("Glasgow",    "Scotland",                55.8642, -4.2518),
        new("Edinburgh",  "Scotland",                55.9533, -3.1883),
    ];

    public static IReadOnlyList<CityInfo> All => AllCities;

    public static CityInfo? Find(string? name) =>
    AllCities.FirstOrDefault(c => c.Name.Equals(name?.Trim(), StringComparison.OrdinalIgnoreCase));
}
