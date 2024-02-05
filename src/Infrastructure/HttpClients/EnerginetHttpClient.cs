using System.Net.Http.Json;

namespace MitElforbrug;

/// <summary>
/// https://www.energidataservice.dk/tso-electricity/Elspotprices
/// </summary>
public record EnerginetElsporprisRequest(DateOnly Start, DateOnly End, string PriceArea);

public record EnerginetElsporprisRecordResponse(EnerginetElsporprisResponse[] Records);

public record EnerginetElsporprisResponse(
    DateTime HourUTC,
    decimal SpotPriceDKK
);

public class EnerginetHttpClient(HttpClient httpClient)
{
    private readonly HttpClient httpClient = httpClient;

    public async Task<IEnumerable<EnerginetElsporprisResponse>> HentHistoriskeElspotpriser(EnerginetElsporprisRequest request)
    {
        string queryParams = await CreateQueryParams(request);

        var response = await httpClient.GetFromJsonAsync<EnerginetElsporprisRecordResponse>(
            requestUri: $"dataset/Elspotprices?{queryParams}"
        );

        return response is null || response.Records.Length < 1
            ? throw new ApplicationException()
            : response.Records;
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
