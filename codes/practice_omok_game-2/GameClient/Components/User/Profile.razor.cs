

using GameClient.Providers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components;

namespace GameClient.Components.User;

public partial class Profile
{

	private UserInfo? UserInfo { get; set; }

	private MailInfo? SendMail { get; set; } = null;

	private bool _isEditMode { get; set; } = false;
	private bool _isMailMode { get; set; } = false;

	[Inject]
	IToastService ToastService { get; set; }

	[Inject]
	MailStateProvider MailStateProvider { get; set; }

	[Inject]
	AuthenticationStateProvider CookieStateProvider { get; set; }

	protected override void OnInitialized()
	{
		UserInfo = ((CookieStateProvider)CookieStateProvider).AuthenticatedUser;
	}

	private bool IsMailMode()
	{
		return _isMailMode;
	}

	private bool IsEditMode()
	{
		return _isEditMode;
	}

	private void ToggleEditMode()
	{
		_isEditMode = !_isEditMode;

		if (false == _isEditMode)
		{
			_ = SaveNickname();
		}
	}
	private void StartMailMode()
	{
		SendMail = new MailInfo()
		{
			SendUid = UserInfo.Uid,
			ReceiveUid = UserInfo.Uid,
			Title = "Send Mail",
			Content = ""
		};

		_isMailMode = true;
	}

	private void QuitMailMode()
	{
		_isMailMode = false;
		SendMail = null;
	}

	private async Task SendMailToUser()
	{
		if (null == SendMail ||
			false == _isMailMode)
			return;

		try
		{
			var result = await MailStateProvider.SendMailAsync(SendMail);

			if (result == ErrorCode.None)
			{
				ToastService.ShowSuccess("Mail sent successfully");
				await MailStateProvider.GetMailsAsync();
			}
			else
			{
				ToastService.ShowError(result.ToString());
			}
		}
		catch(Exception e)
		{
			ToastService.ShowError(e.Message);
		}
		finally 
		{ 
			_isMailMode = false;
			SendMail = null;
		}

	}

	private async Task SaveNickname()
	{
		try
		{
			var result = await ((CookieStateProvider)CookieStateProvider).UpdateNicknameAsync(UserInfo.Nickname);

			if (result == ErrorCode.None)
			{
				ToastService.ShowSuccess("Nickname updated");
			}
			else
			{
				ToastService.ShowError(result.ToString());
			}
		}
		catch(Exception e)
		{
			ToastService.ShowError(e.Message);
		}
	}
}
