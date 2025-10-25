using SkinHolderDesktop.Utils;
using System.Net;
using System.Net.Http;

namespace SkinHolderDesktop.Core;

public class UnauthorizedHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            ProcessUtils.SaveErrorMessageToFile("Sesión expirada");
            ProcessUtils.RestartApplication();
        }

        return response;
    }
}