using System.Formats.Tar;
using System.IO.Compression;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using MitElforbrug.Infrastructure;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace MitElforbrug.Features;

public record VisElforbrugOgSpotpriserFunction(VisElforbrugOgSpotpriserHandler Handler)
{
    [Function("VisElforbrugOgSpotpriser")]
    public async Task<IActionResult> Run(
        [HttpTrigger(authLevel: AuthorizationLevel.Anonymous, methods: "post")] HttpRequest req,
        [FromBody] VisElforbrugOgSpotpriserRequest request)
        => new OkObjectResult(await Handler.Handle(request));
}

public record VisElforbrugOgSpotpriserRequest(
    DateOnly Start,
    DateOnly End,
    string[] Målepunkter
);

public record VisElforbrugOgSpotpriserResponse(
    EnerginetElspotprisResponse[] Elsporpriser,
    Måleraflæsning[] Elforbrug
);

public record VisElforbrugOgSpotpriserHandler(
    EloverblikHttpClient EloverblikHttpClient,
    EnerginetHttpClient EnerginetHttpClient
)
{
    public async Task<VisElforbrugOgSpotpriserResponse> Handle(VisElforbrugOgSpotpriserRequest request)
    {

        var elspotpriserTask = EnerginetHttpClient.HentHistoriskeElspotpriser(
            request: new EnerginetElsporprisRequest(
                Start: request.Start,
                End: request.End,
                PriceArea: "DK1")
        );

        var tarifferTask = EloverblikHttpClient.HentTariffer(request.Målepunkter.First());

        var elforbrugTask = EloverblikHttpClient.HentMåleraflæsninger(
            request: new HentMåleraflæsningerRequest(
                FraDato: request.Start,
                TilDato: request.End,
                Målepunkter: request.Målepunkter)
        );

        Task.WaitAll(elspotpriserTask, tarifferTask, elforbrugTask);

        var elspotpriser = await elspotpriserTask;
        var tariffer = await tarifferTask;
        var elforbrug = await elforbrugTask;

        elspotpriser = elspotpriser.Select(spot => spot with
        {
            Tarrif = tariffer.GetValueOrDefault(TimeOnly.FromDateTime(spot.HourUTC))
        });

        var elspotpriser__ = (await elspotpriserTask).ToDictionary(key => key.HourUTC, value => value.SpotPriceDKK);

        var elforbrug__ = elforbrug.Måleraflæsninger.Select(forbrug =>
        {
            var tariff = tariffer.GetValueOrDefault(TimeOnly.FromDateTime(forbrug.Tidspunkt));
            var spotpris = elspotpriser__.GetValueOrDefault(forbrug.Tidspunkt)!;

            return forbrug with { 
                SpotPriceDKK = spotpris, 
                Tarrif = tariff
                };
        });
        return new VisElforbrugOgSpotpriserResponse(
            Elsporpriser: [.. elspotpriser],
            Elforbrug: [.. elforbrug__]
        );
    }
}