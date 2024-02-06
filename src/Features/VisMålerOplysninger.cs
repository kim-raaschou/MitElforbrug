using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using MitElforbrug.Infrastructure;
using static System.String;

namespace MitElforbrug.Features;

public record VisMålerOplysningerResponse(
    string HovedmålepunktId,
    string Kundeoplysninger,
    string Leverandør,
    IEnumerable<string> Undermålepunkter
);

public record VisMålerOplysningerFunction(VisMålerOplysningerHandler Handler)
{
    [Function("VisMaaleroplysninger")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        => new OkObjectResult(await Handler.Handle());
}

public record VisMålerOplysningerHandler(EloverblikHttpClient HttpClient)
{
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

file static class EloverblikMeteringpointsResponseExtension
{
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