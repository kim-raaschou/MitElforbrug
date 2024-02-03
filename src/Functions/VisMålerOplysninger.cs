using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MitElforbrug.Core.UseCases;

namespace MitElforbrug.Functions;

public class VisMålerOplysninger(VisMålerOplysningerHandler visMålerOplysningerHandler, ILogger<VisMålerOplysninger> logger)
{
    private readonly VisMålerOplysningerHandler _visMålerOplysningerHandler = visMålerOplysningerHandler;
    private readonly ILogger<VisMålerOplysninger> _logger = logger;

    [Function("VisMaaleroplysninger")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        var måleroplysninger = await _visMålerOplysningerHandler.Handle();
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult(måleroplysninger);
    }
}
