using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MitElforbrug.Functions
{
    public class TestTrigger
    {
        private readonly ILogger<TestTrigger> _logger;

        public TestTrigger(ILogger<TestTrigger> logger)
        {
            _logger = logger;
        }

        [Function("TestTrigger")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
