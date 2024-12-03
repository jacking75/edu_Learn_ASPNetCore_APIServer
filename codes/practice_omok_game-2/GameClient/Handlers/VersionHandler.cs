namespace GameClient.Handlers;

public class VersionHandler : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		request.Headers.Add("AppVersion", "1.0.0");
		request.Headers.Add("MasterDataVersion", "1.0.0");
		return await base.SendAsync(request, cancellationToken);
	}
}
