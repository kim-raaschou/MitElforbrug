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
    EnerginetElsporprisResponse[] Elsporpriser,
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

        var elforbrugTask = EloverblikHttpClient.HentMåleraflæsninger(
            request : new HentMåleraflæsningerRequest(
                FraDato: request.Start,
                TilDato: request.End,
                Målepunkter: request.Målepunkter)
        );

        Task.WaitAll(elspotpriserTask, elforbrugTask);
        var (elspotpriser, elforbrug) = (await elspotpriserTask, await elforbrugTask);

        return new VisElforbrugOgSpotpriserResponse(
            Elsporpriser: [.. elspotpriser],
            Elforbrug: [.. elforbrug.Måleraflæsninger]
        );
    }
}