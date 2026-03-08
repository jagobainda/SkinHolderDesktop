using SkinHolderDesktop.Utils;
using System.Net;
using System.Net.Http;

namespace SkinHolderDesktop.Core;

public class UnauthorizedHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        var isAuthenticatedRequest = request.Headers.Authorization is not null;

        if (response.StatusCode == HttpStatusCode.Unauthorized && isAuthenticatedRequest)
        {
            ProcessUtils.SaveErrorMessageToFile("Sesión expirada");
            ProcessUtils.RestartApplication();
        }

        return response;
    }
}