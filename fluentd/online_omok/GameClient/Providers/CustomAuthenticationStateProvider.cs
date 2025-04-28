using GameClient.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace GameClient.Providers;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
    private bool _authenticated = false;
	private DataLoadService _dataLoadService;


	public CustomAuthenticationStateProvider(DataLoadService dataLoadService)
	{
		_dataLoadService = dataLoadService;
	}
	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
        _authenticated = false;

        try
		{
			var result = await _dataLoadService.LoadUserDataAsync();

			if (false == result)
			{
				return new AuthenticationState(_anonymousUser);
			}

			result = await _dataLoadService.LoadMasterDataAsync();

			if (false == result)
			{
				return new AuthenticationState(_anonymousUser);
			}

			_authenticated = true;
			return new AuthenticationState(new ClaimsPrincipal
				(new ClaimsIdentity([new Claim(ClaimTypes.Name, "User")], "auth")));

		}
		catch (Exception e)
		{
			_authenticated = false;
			Console.WriteLine(e.Message);
			return new AuthenticationState(_anonymousUser);
        }
	}

}
