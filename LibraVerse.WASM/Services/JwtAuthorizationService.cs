using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace LibraVerse.WASM.Services;

public class JwtAuthorizationService(IJSRuntime jsRuntime) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await jsRuntime.InvokeAsync<string>("localStorage.getItem", cancellationToken, "authToken");

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}