using System.Net.Http.Headers;
using System.Net.Http.Json;
using Google.Protobuf.WellKnownTypes;

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
        var token =MyToken.Value;//  File.ReadAllText(path);
        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseAddress}/token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }
}

file static class MyToken{
    public static string Value => "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0b2tlblR5cGUiOiJDdXN0b21lckFQSV9SZWZyZXNoIiwidG9rZW5pZCI6IjdjYzE4ZWQzLThkNzgtNDVmMS05YzU5LTQ1MmVlODllMjRlOCIsIndlYkFwcCI6IkN1c3RvbWVyQXBwIiwidmVyc2lvbiI6IjIiLCJpZGVudGl0eVRva2VuIjoiQ0krdGhtaU5mQU1hRWMyaGkwbnBuUUZLRlp0K0J0YXlUZkY4YnhGR3A3KytNWXVHa1FxdHgwUmd1VVFVYXU2K3luZmF3bmJibWw1a1lmbFFQSjVwWG9Yb1FXdHV1WnR3TDA3UCtZL3c0NEUzTVNQSS90bUpCNU5LMnQ4MjZxYXo2Z3FEQVVRY3NlTjVsOUl1WDZaZ2ZSWkhkTU9kdVBVclJtWm01MmM2K29VVjBHVG1EWXEyVTBMR1lEYWUwUzVTOFp2blRYK2VLcC8zSDAzUFVWaEkwTmgyRnA3Mmh0eXNPT2lTck10bVArMVRRN1NGQXlxdnI5Y1VXa3Q4Q3dwVGV5N3hIcjFhRjNWUnlpSTdvUUdKZ2pvVk9YZzVFQzBKM21BMzJWRDByRW1VV3BZbi83dUdNVXRUUzEyZGhub1hScW5kbVNKWkVkc0xZVkZlUlZiZmlZOWFTd0xJNUlHLzhxeDBNQ0VSb0JVTnd5dktmZGgwMVptcWhzU0dlQ1cwbFJSb2FCWkpYNG5XcFRvcTF1OE9YdGw1b2Y4QS93MjZlMlByaEw5c0hWalpIUW1DWU5pRzVHUXRSaUJZbUw3cExnME15QURxUWdrQ3diNTF4RlhwalcrQXA3TklYbUxUOEFJZXJUb2VZalh3RFVHL2s4Y21uZEtZVENFVjBwRWNGUzdzUTZFNDBsdWc2TDdlaDlYQi8zRFpOUVQrR29FZDEwb2pNa3A2ZG5QQi9ya1ZhcTl4TFhMcDNxNnFRMjJyUlV2bG0rajNLZ3E2WjlveTNIbFVocjBHaHo3TzJMQkJXNGVxMU1LbWt2QWFOYURGdE9hWUI4WnQrUk1ld1ZKOURSeU8yYmpEa0g1WEdUczZHNGRKRmtnT0YrTldqVjk4TGZkLy9vcVdleUIzYnZuQnM0SGVyTDdEdmlaVmVZeUFtUTQ1WFJoaHFWV3Fsdk5pZ3ZQbFA3OGVHWE1UVnA3czk1dWgwdnlZVjlpV1FCMFZHa0c5Vmg0UTRXbHJzQlVVelpqaXB6TFM5VTFXOTc5S2srVkNoSmtoWEhvZE1mZEIvQmx2ZXoxU0xIc3RlclZwdUk1a29TQVZzTDlva0prZkkvaU9VZkNyc0Mva0RuM3Mrb0hRbzh3TEtic24wRmZoYlF6SGdtbnljNFRHTlNyVkt2K2pFVjIyeXpjNzEyNWVQdG45czkyb0NRQnZDQmtzZVVObkpsY0hVbGFnc3BYSk5UOTJaNWVCOWRNcU9EeStWd2FSVzQ1OTNYaG13UEptamV2Y2szT2tEMExGRVdrRXpnZ0hRVld5M0pPWVdNSDlraEl2NTlDV2xoUXNJelF4QktUcHBkdmR5a2Z2enFGN1l4RFNnZncwMXNqQVF1LzRMcW51b0xlN0hrN2tWZ0d3ZmJPK2VleDQySWVpU05TaGcxUytIM056WEhpYWt2d0pWTGQ5ODI1TTNJT0o3bDJiWjZ6c0NQYUpCd3p0bW9PTzVNTEJ5UWFCd3RCZE9hODF3TWZUVkVCT3lhcXM0YlBtQjdrcytLRmY2eGNZSm5KZ0t4TjNKTEJGcmQwRUdnNWZPcjhEaGZSWnBmeEwwRmVXdU9DNmlCbFBTdTV3N2ZYVG8vdlJMVWwwT3MzY3crR0xyMEpwM24zVjRsbjEzdVBEbjIwTVpsWDZnbmpkRDF1elBMUDUrMGVCOG1mdzdwNU9xenFEcDdqdzl5ZlEvNGRKWkpneDN1M0hvVTEzK25kR2RBdWtIU2ZzNTY3bWs1Wld1MlI4THhVU3g0SHduNmYvSmZnRVRYSUJVVHI0MkNZeVNneWVSR0h6ZkVjcUVTSFE3WHJZSW5VRHloM2lIS2dhenNhNFhIYjNadWF1cHEwcVJXTm9sRVhERE83N2hZRjlBVVVBYzViUUp6LzE0Z2ExMmJScHczZnFyYTQzdmloMUhpY2UwOUk3ZUVmZkN0SGRyWXBvRE9mclZFTDl3NHUwTkZEbkg1ZVZkckM0TWhQOTFIME9NQzRqaHNVSjlRekxiTE9uWU9qbS9aWllraGkwZFlHOEwyQXRJemgwa1hVSStWTUhXaWVOYWtpN2pQQktjYkZVa0Z4RHh1VWZBTktlNTRBSmF1ck9QVC84S2E3TU1hNUh0TXBISjZWUjRaa1dzU3dqcW1pNTZjeGVqb0wwZFVNdTJiUWVsOXpITVZMSXFLS0E1UEJCNVJ0a2Y3OFQrd1dQVzd3VVJnVklQMTFCcFhuOWI0SjBkMXNKbDBtQWVGRlpPdm02ZGVMVGdsYTJuSmx2NlQvMGxpNlo3VFhBWTVHVDVBRWRlclg0dklTbm1uSzBLS2hGNzVreUFkS1d3UmJpSSsyOFdRUEh2Kzl6VFd0MlhJYTh3WTRsRWVPQ09QQlFleEw4WlRSTTlqTUhORGczaWNkV0ZmWk5CdVVaWklWVXF3d3ZIUG4zMFlybHJmK2ExejJmSkIrZnVDekRYM0VnVzIyKzZwdTRuVXlrTGttNWdvdDFQek9yR21EdjhHa213V3M4SzdVajVCZlJoTTVneTI5TFNRdlRraEptIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiS2ltIFJhYXNjaG91IE5pZWxzZW4iLCJsb2dpblR5cGUiOiJLZXlDYXJkIiwiYjNmIjoiaWxYL0ZjeG5DeEN6ZW5sN0JxdFR1NW1nNUp5NUREVVNQVUNwc3gvWkx3cz0iLCJwaWQiOiJQSUQ6OTIwOC0yMDAyLTItNzYwNzM3MzU1ODIyIiwidXNlcklkIjoiMTMwODEyIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiJQSUQ6OTIwOC0yMDAyLTItNzYwNzM3MzU1ODIyIiwiZXhwIjoxNzM3ODAzODI1LCJpc3MiOiJFbmVyZ2luZXQiLCJqdGkiOiI3Y2MxOGVkMy04ZDc4LTQ1ZjEtOWM1OS00NTJlZTg5ZTI0ZTgiLCJ0b2tlbk5hbWUiOiJlbHRva2VuX3ZyYW5nZWLDuGd2ZWoiLCJhdWQiOiJFbmVyZ2luZXQifQ.L-SeQB-pMSITWKL2PF2fWt88CB1zKd3T6ZVDET6_zvE";
}
