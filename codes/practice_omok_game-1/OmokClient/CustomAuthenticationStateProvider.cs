using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Console.WriteLine("Getting current authentication state.");
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public void NotifyUserLoggedIn(string username)
    {
        var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.Name, username)
        }, "apiauth_type");

        _currentUser = new ClaimsPrincipal(identity);
        Console.WriteLine($"User '{username}' logged in.");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public void NotifyUserLoggedOut()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        Console.WriteLine("User logged out.");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }
}
