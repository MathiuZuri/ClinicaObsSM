using System.Net.Http.Headers;

namespace Clinica.WASM.Services.Auth;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly TokenStorageService _tokenStorageService;

    public AuthHeaderHandler(TokenStorageService tokenStorageService)
    {
        _tokenStorageService = tokenStorageService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenStorageService.ObtenerTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}