using System.Net.Http.Json;

namespace MitElforbrug;

/// <summary>
/// https://www.energidataservice.dk/tso-electricity/Elspotprices
/// </summary>
public record EnerginetElsporprisRequest(DateOnly Start, DateOnly End, string PriceArea);

public record EnerginetElsporprisRecordResponse(EnerginetElspotprisResponse[] Records);

/// <summary>
/// SpotPriceDKK er prisen pr MWh. Divider med 1000 for KWh 
/// </summary>
/// <see cref="https://www.energidataservice.dk/tso-electricity/Elspotprices#metadata-info"/>
/// <param name="HourUTC"></param>
/// <param name="SpotPriceDKK"></param>
public record EnerginetElspotprisResponse(
    DateTime HourUTC,
    decimal SpotPriceDKK,
    decimal Tarrif = 0
)
{
    public decimal SamletPris => SpotPriceDKK + Tarrif;
}

public class EnerginetHttpClientBaseAddress : Uri
{
    private const string uri = "https://api.energidataservice.dk";
    public EnerginetHttpClientBaseAddress() : base(uri) { }
}

public class EnerginetHttpClient(HttpClient httpClient)
{
    private readonly HttpClient httpClient = httpClient;

    public async Task<Dictionary<DateTime, decimal>> HentHistoriskeElspotpriser(EnerginetElsporprisRequest request)
    {
        string queryParams = await CreateQueryParams(request);

        var response = await httpClient.GetFromJsonAsync<EnerginetElsporprisRecordResponse>(
            requestUri: $"dataset/Elspotprices?{queryParams}"
        );

        return response is null || response.Records.Length < 1
            ? throw new ApplicationException()
            : response.Records.ToDictionary(key => key.HourUTC, SportprisIKWh);

        decimal SportprisIKWh(EnerginetElspotprisResponse spotpris)
            => spotpris.SpotPriceDKK / 1000;
    }

    private static async Task<string> CreateQueryParams(EnerginetElsporprisRequest request)
    {
        var priceAreaJsonContent = JsonContent.Create(new { request.PriceArea });
        var formUrlEncodedContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "start"   , request.Start.ToString("yyyy-MM-dd") },
                { "end"     , request.End.ToString("yyyy-MM-dd") },
                { "filter"  , await priceAreaJsonContent.ReadAsStringAsync() },
                { "sort"    , "HourDK asc" }
            });

        return await formUrlEncodedContent.ReadAsStringAsync();
    }
}
