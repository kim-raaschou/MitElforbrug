using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MitElforbrug.Infrastructure;

internal class EloverblikJwtTokenHandler(TimeProvider timeProvider, Uri baseAddress) : DelegatingHandler
{
    private static string? _accessJwtToken;

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
        var request = GetHttpRequestMessage(baseAddress);
        var response = await base.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<EloverblikResult<string>>(cancellationToken) switch
        {
            null => throw new ApplicationException(),
            EloverblikResult<string> tokenResponse => tokenResponse
        };
    }

    static HttpRequestMessage GetHttpRequestMessage(Uri baseAddress)
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tokens/MinToken.txt");
        var token = File.ReadAllText(path);
        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseAddress}/token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }
}
