using MitElforbrug.Infrastructure;
using static System.String;

namespace MitElforbrug.Core.UseCases;

public record VisMålerOplysningerResponse(
    string HovedmålepunktId,
    string Kundeoplysninger,
    string Leverandør,
    IEnumerable<string> Undermålepunkter
);

public class VisMålerOplysningerHandler(EloverblikHttpClient httpClient)
{
    private EloverblikHttpClient HttpClient { get; } = httpClient;

    public async Task<VisMålerOplysningerResponse> Handle()
    {
        EloverblikMeteringpointsResponse response = await HttpClient.HentHovedmålepunkt();

        return new VisMålerOplysningerResponse(
            Leverandør: response.BalanceSupplierName,
            Kundeoplysninger: response.FormaterAdresse(),
            HovedmålepunktId: response.MeteringPointId,
            Undermålepunkter: response.ChildMeteringPoints.Select(x => x.MeteringPointId)
        );
    }
}

file static class EloverblikMeteringpointsResponseExtension{
    public static string FormaterAdresse(this EloverblikMeteringpointsResponse response)
        => $"""
            {FormaterKundenavne(response)}
            {response.StreetName} {response.BuildingNumber}
            {response.Postcode} {response.CityName}
            """;

    private static string FormaterKundenavne(EloverblikMeteringpointsResponse response)
        => IsNullOrEmpty(response.SecondConsumerPartyName)
            ? response.FirstConsumerPartyName
            : $"{response.FirstConsumerPartyName} og {response.SecondConsumerPartyName}";
} 