using GameClient.Models;
using GameClient.Providers;
using GameClient.Services;
using GameShared.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components;

namespace GameClient.Components.Menu;

public partial class Profile
{
	private UserInfo? UserInfo { get; set; }
	private MailInfo? SendMail { get; set; } = null;
	private bool _isEditMode { get; set; } = false;
	private bool _isMailMode { get; set; } = false;

	[Inject]
	IToastService ToastService { get; set; }
	[Inject]
	MailService MailService { get; set; }
	[Inject]
	DataLoadService DataLoadService { get; set; }

	protected override void OnInitialized()
	{
		UserInfo = DataLoadService.UserData?.UserInfo;
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
			var result = await MailService.SendMailAsync(UserInfo.Uid, SendMail.Title, SendMail.Content);

			if (result == ErrorCode.None)
			{
				ToastService.ShowSuccess("Mail sent successfully");
				await MailService.GetMailsAsync();
			}
			else
			{
				ToastService.ShowError(result.ToString());
			}
		}
		catch (Exception e)
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
		if (null == UserInfo)
			return;

		try
		{
			var result = await DataLoadService.UpdateNicknameAsync(UserInfo.Nickname);

			if (result == ErrorCode.None)
			{
				ToastService.ShowSuccess("Nickname updated");
			}
			else
			{
				ToastService.ShowError(result.ToString());
			}
		}
		catch (Exception e)
		{
			ToastService.ShowError(e.Message);
		}
	}
}
