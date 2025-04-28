using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using GameShared.DTO;
using GameClient.Services;

namespace GameClient.Pages;

public partial class Register
{
    [Inject]
    protected AuthService AuthService { get; set; } = null!;
    [Inject]
    protected NavigationManager Navigation { get; set; }
    [Inject]
    protected IToastService ToastService { get; set; }

    protected HiveRegisterRequest User { get; set; } = new HiveRegisterRequest();

    private async Task HandleRegisterAsync()
    {
        try
        {
            var response = await AuthService
                .RegisterAsync(User.Email, User.Password);

            if (ErrorCode.None != response)
            {
                HandleInvalidResponse(response);
            }
            else
            {
                ToastService.ShowSuccess("Account created successfully!");
                RedirectToLogin();
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError(ex.Message);
        }

    }

    private void RedirectToLogin()
    {
        Navigation.NavigateTo("/login");
    }

    private void HandleInvalidResponse(ErrorCode error)
    {
        ToastService.ShowError($"Failed to create account. Error Code:{error}");
    }

}
