using System.Net.Http.Json;
using System.Text.Json;

namespace MitElforbrug.Infrastructure;

public class EloverblikHttpClientBaseAddress: Uri {
    private const string uri = "https://api.eloverblik.dk/customerapi/api";  
    public EloverblikHttpClientBaseAddress() : base(uri){ }
}

/// <summary>
/// Find informationer om energinet her: https://energinet.dk/data-om-energi/datahub/
/// Swagger api: https://api.eloverblik.dk/customerapi/index.html. Her findes også info omkring login
/// og oprettelse af access token. Kræver NemId login.
/// </summary>
/// <param name="httpClient"></param>
public class EloverblikHttpClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient httpClient = httpClient;

    public async Task<EloverblikMeteringpointsResponse> HentHovedmålepunkt()
    {
        var response = await httpClient.GetFromJsonAsync<EloverblikResult<EloverblikMeteringpointsResponse[]>>(
            requestUri: "api/meteringpoints/meteringpoints?includeAll=true"
        );

        return response?.Result?.Length > 0
            ? response.Result.Single()
            : throw new ApplicationException();
    }

    public async Task<HentMåleraflæsningerResponse> HentMåleraflæsninger(HentMåleraflæsningerRequest request)
    {
        var (fraDato, tilDato, _) = request;
        var response = await httpClient.PostAsync(
            requestUri: $"api/meterdata/gettimeseries/{fraDato:yyyy-MM-dd}/{tilDato:yyyy-MM-dd}/Hour",
            content: JsonContent.Create(new
            {
                MeteringPoints = new { MeteringPoint = request.Målepunkter.ToArray() }
            })
        );

        response.EnsureSuccessStatusCode();

        var node = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var måleraflæsninger = ExtractPeriodNodes(node).SelectMany(periodNode =>
        {
            var period = JsonSerializer.Deserialize<Period>(
                json: periodNode.GetRawText(), 
                options: serializerOptions
            );

            return period!.Point.Select(point =>
            {
                return new Måleraflæsning(
                    Tidspunkt: period!.TimeInterval.Start.AddHours(point.Position),
                    ForbrugKwh: point.Quantity);
            });
        });

        return new HentMåleraflæsningerResponse(Måleraflæsninger: måleraflæsninger.ToArray());
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