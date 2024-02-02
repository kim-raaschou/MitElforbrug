using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MinEltavle.Core.Domain;
using MitElforbrug.Core;

namespace MitElforbrug.Infrastructure;

public record Period(
    PeriodTimeInterval TimeInterval, 
    PeriodPoint[] Point
);

public record PeriodTimeInterval(DateTime Start);
public record PeriodPoint(int Position, decimal Quantity)
{
    [JsonPropertyName("out_Quantity.quantity")]
    public decimal Quantity { get; } = Quantity;
}

public class EloverblikAdapter(HttpClient httpClient)
{
    private readonly HttpClient httpClient = httpClient;

    public async Task<HovedMålepunkt> HentMålepunkter()
    {
        var requestUri = "api/meteringpoints/meteringpoints?includeAll=true";
        var response = await httpClient.GetFromJsonAsync<EloverblikResult<EloverblikMeteringpointsResponse[]>>(requestUri);

        if (response is null || response.Result.Length == 0) throw new ApplicationException();

        var meteringpointsResponse = response.Result.First();
        return new HovedMålepunkt(
            HovedmålepunktId: meteringpointsResponse.MeteringPointId,
            Adresse: new(
                Vejnavn: meteringpointsResponse.StreetName,
                Husnummer: meteringpointsResponse.BuildingNumber,
                By: meteringpointsResponse.CityName,
                Postnummer: meteringpointsResponse.Postcode,
                Etage: meteringpointsResponse.FloorId,
                Dør: meteringpointsResponse.RoomId
            ),
            Undermålepunkter: meteringpointsResponse.ChildMeteringPoints
                .Select(child => child.MeteringPointId)
                .ToImmutableArray()
        );
    }

    public async Task<IEnumerable<ElForbrug>> HentMåleraflæsninger(HentMåleraflæsningerRequest request)
    {
        var response = await httpClient.PostAsync(
            requestUri: $"api/meterdata/gettimeseries/{request.FraDato:yyyy-MM-dd}/{request.TilDato:yyyy-MM-dd}/Day",
            content: JsonContent.Create(new
            {
                MeteringPoints = new
                {
                    MeteringPoint = request.Målepunkter.ToArray()
                }
            }));

        response.EnsureSuccessStatusCode();

        var node = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        if (node is null) throw new ApplicationException();

        return node.RootElement.GetProperty("result")[0]
            .GetProperty("MyEnergyData_MarketDocument")
            .GetProperty("TimeSeries")[0]
            .GetProperty("Period")
            .EnumerateArray()
            .Select(periodNode =>
            {
                var period = JsonSerializer.Deserialize<Period>(
                    json: periodNode.GetRawText(),
                    options: new JsonSerializerOptions(JsonSerializerDefaults.Web)
                );

                return new ElForbrug(
                    Tidspunkt: period!.TimeInterval.Start.AddHours(period.Point[0].Position),
                    ForbrugKwh: period.Point[0].Quantity);
            });
    }
}