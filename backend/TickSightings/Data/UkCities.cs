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
                new("York",           "Yorkshire and the Humber", 53.9620, -1.0837),
        new("Doncaster",      "Yorkshire and the Humber", 53.5227, -1.1324),
        new("Middlesbrough",  "North East",               54.5764, -1.2368),
        new("Preston",        "North West",               53.7606, -2.6985),
        new("Lancaster",      "North West",               54.0501, -2.8032),
        new("Blackpool",      "North West",               53.8209, -3.0515),
        new("Coventry",       "West Midlands",            52.4077, -1.5069),
        new("Stoke-on-Trent", "West Midlands",            53.0222, -2.1734),
        new("Worcester",      "West Midlands",            52.1929, -2.2211),
        new("Derby",          "East Midlands",            52.9247, -1.4781),
        new("Lincoln",        "East Midlands",            53.2348, -0.5384),
        new("Cambridge",      "East of England",          52.2049,  0.1197),
        new("Norwich",        "East of England",          52.6289,  1.2933),
        new("Ipswich",        "East of England",          52.0571,  1.1528),
        new("Peterborough",   "East of England",          52.5725, -0.2425),
        new("Luton",          "East of England",          51.8795, -0.4174),
        new("Chelmsford",     "East of England",          51.7370,  0.4736),
        new("Colchester",     "East of England",          51.8897,  0.8993),
        new("Oxford",         "South East",               51.7520, -1.2576),
        new("Reading",        "South East",               51.4538, -0.9738),
        new("Milton Keynes",  "South East",               52.0430, -0.7541),
        new("Portsmouth",     "South East",               50.7968, -1.0912),
        new("Canterbury",     "South East",               51.2797,  1.0789),
        new("Exeter",         "South West",               50.7226, -3.5326),
        new("Plymouth",       "South West",               50.3700, -4.1421),
        new("Gloucester",     "South West",               51.8646, -2.2469),
        new("Swindon",        "South West",               51.5615, -1.7855),
        new("Swansea",        "Wales",                    51.6214, -3.9436),
        new("Wrexham",        "Wales",                    53.0479, -2.9948),
        new("Stirling",       "Scotland",                 56.1155, -3.9342),
        new("Belfast",        "Northern Ireland",         54.5973, -5.9301),
    ];

    public static IReadOnlyList<CityInfo> All => AllCities;
    public static IEnumerable<string> Regions => AllCities.Select(c => c.Region).Distinct();

    public static string? FindRegion(string? region) =>
    Regions.FirstOrDefault(r => r.Equals(region?.Trim(), StringComparison.OrdinalIgnoreCase));
    public static CityInfo? Find(string? name) =>
    AllCities.FirstOrDefault(c => c.Name.Equals(name?.Trim(), StringComparison.OrdinalIgnoreCase));
}
