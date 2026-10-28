using TickSightings.Models;
namespace TickSightings.Services;

public class ElancoApiClient(HttpClient http)
{

    public async Task<IReadOnlyList<ElancoSighting>> GetAllSightingsAsync(CancellationToken cancellationToken= default) 
    {
        var sightings = await http.GetFromJsonAsync<List<ElancoSighting>>("tick-sightings", cancellationToken);
        //get an empty list instead of a null sightings variable
        return sightings ?? [];
    }
}