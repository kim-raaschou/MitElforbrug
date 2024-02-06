using System.Net.Http.Json;
using System.Text.Json;

namespace MitElforbrug.Infrastructure;

public record HentMåleraflæsningerRequest(
    DateOnly FraDato,
    DateOnly TilDato,
    string[] Målepunkter
);

public record ElForbrug(
    DateTime Tidspunkt,     
    decimal ForbrugKwh
);


public class EloverblikHttpClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient httpClient = httpClient;

    public async Task<EloverblikMeteringpointsResponse> HentHovedmålepunkt()
    {
        var requestUri = "api/meteringpoints/meteringpoints?includeAll=true";
        var response = await httpClient.GetFromJsonAsync<EloverblikResult<EloverblikMeteringpointsResponse[]>>(requestUri);

        return response?.Result?.Length > 0
            ? response.Result.Single()
            : throw new ApplicationException();
    }

    public async Task<IEnumerable<ElForbrug>> HentMåleraflæsninger(HentMåleraflæsningerRequest request)
    {
        var response = await httpClient.PostAsync(
            requestUri: $"api/meterdata/gettimeseries/{request.FraDato:yyyy-MM-dd}/{request.TilDato:yyyy-MM-dd}/Hour",
            content: JsonContent.Create(new
            {
                MeteringPoints = new { MeteringPoint = request.Målepunkter.ToArray() }
            })
        );

        response.EnsureSuccessStatusCode();

        var node = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return ExtractPeriodNodes(node).SelectMany(periodNode =>
        {
            var period = JsonSerializer.Deserialize<Period>(
                json: periodNode.GetRawText(),
                options: serializerOptions
            );

            return period!.Point.Select(point =>
            {
                return new ElForbrug(
                    Tidspunkt: period!.TimeInterval.Start.AddHours(point.Position),
                    ForbrugKwh: point.Quantity);
            });
        });
    }

    private static JsonElement.ArrayEnumerator ExtractPeriodNodes(JsonDocument node)
        => node is null
            ? throw new ApplicationException()
            : node.RootElement.GetProperty("result")[0]
                .GetProperty("MyEnergyData_MarketDocument")
                .GetProperty("TimeSeries")[0]
                .GetProperty("Period")
                .EnumerateArray();
}