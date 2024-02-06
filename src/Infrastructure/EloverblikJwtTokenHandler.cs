using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MitElforbrug.Infrastructure;


/// <summary>
/// AccessToken overtræder reglen omkring at en serverless function ikke må have nogen state.
/// Håndtering af access token bør refaktureres til egen service og token bør gennes i en Key Vault i Azure.
/// Men dette er en POC og der er kun én token...
/// </summary>
/// <param name="timeProvider"></param>
/// <param name="baseAddress"></param>
internal class EloverblikJwtTokenHandler(TimeProvider timeProvider, EloverblikHttpClientBaseAddress baseAddress) : DelegatingHandler
{
    private static string? _accessJwtToken;
    private readonly TimeProvider timeProvider = timeProvider;
    private readonly EloverblikHttpClientBaseAddress baseAddress = baseAddress;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await EnsureAccessJwtToken(timeProvider, cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessJwtToken);
        var response = await base.SendAsync(request, cancellationToken);
        return response;
    }

    private async Task EnsureAccessJwtToken(TimeProvider timeProvider, CancellationToken cancellationToken)
    {
        if (_accessJwtToken is not null) return;

        _accessJwtToken = await GetAccessJwtToken(cancellationToken);
        timeProvider.CreateTimer(
            callback: (_) => _accessJwtToken = null,
            state: null,
            dueTime: new TimeSpan(hours: 23, minutes: 59, seconds: 0),
            period: Timeout.InfiniteTimeSpan
        );
    }

    private async Task<string> GetAccessJwtToken(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseAddress}/token");
        request.Headers.Authorization = new AuthenticationHeaderValue(
           scheme: "Bearer",
           parameter: EloverblikMyAccessToken.Value
        );
        var response = await base.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<EloverblikResult<string>>(
            cancellationToken: cancellationToken
        );

        return result is null
            ? throw new ApplicationException()
            : result;
    }
}
