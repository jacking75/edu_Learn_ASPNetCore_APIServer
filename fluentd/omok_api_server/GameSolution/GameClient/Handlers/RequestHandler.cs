using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace GameClient.Handlers;

public class RequestHandler : DelegatingHandler
{
    protected ILocalStorageService LocalStorage { get; set; }

    public RequestHandler(ILocalStorageService localStorage)
	{
        LocalStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var token = await LocalStorage.GetItemAsync<string>("accesstoken", cancellationToken);
        var uid = await LocalStorage.GetItemAsync<string>("uid", cancellationToken);

        if (!string.IsNullOrEmpty(token))
		{
            request.Headers.Add("Authorization", $"Bearer {token}");
        }

        if (!string.IsNullOrEmpty(uid))
        {
            request.Headers.Add("Uid", uid);
        }

        request.Headers.Add("AppVersion", "1.0.0");
		request.Headers.Add("MasterDataVersion", "1.0.0");

		return await base.SendAsync(request, cancellationToken);
	}
}
