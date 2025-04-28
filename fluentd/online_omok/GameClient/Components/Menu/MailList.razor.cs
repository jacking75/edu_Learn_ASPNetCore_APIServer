using GameClient.Models;
using GameClient.Services;
using GameShared.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace GameClient.Components.Menu;

public partial class MailList
{
	[Inject]
	private IToastService ToastService { get; set; }
	[Inject]
	private MailService MailService { get; set; }
	[Inject]
	private DataLoadService DataLoadService { get; set; }

	private List<MailInfo> UserMailList { get; set; } = new List<MailInfo>();

	private List<MailInfo>? _currentList => UserMailList?.Where(e => e.MailType == CurrentMailType).ToList();

	private MailInfo? _selectedMail { get; set; } = null;

	private MailType CurrentMailType { get; set; } = MailType.System;

	protected override async Task OnInitializedAsync()
	{
		await RefreshMail();
	}

	private async Task ReadMail(Int64 mailUid)
	{
		try
		{
			var result = await MailService.ReadMailAsync(mailUid);

			if (result == ErrorCode.None)
			{
				_selectedMail = UserMailList.FirstOrDefault(e => e.Uid == mailUid);
				await RefreshMail();
				StateHasChanged();
			}
			else
			{
				ToastService.ShowError(result.ToString());
			}
		}
		catch (Exception ex)
		{
			ToastService.ShowError(ex.Message);
		}
	}

	private async Task ReceiveMail(Int64 mailUid)
	{
		try
		{
			var result = await MailService.ReceiveMailAsync(mailUid);

			if (result == ErrorCode.None)
			{
				await RefreshMail();
				await DataLoadService.LoadUserDataAsync();
				ToastService.ShowSuccess("Received mail");
			}
			else
			{
				ToastService.ShowError(result.ToString());
			}
		}
		catch (Exception ex)
		{
			ToastService.ShowError(ex.Message);
		}
	}

	private async Task DeleteMail(Int64 mailUid)
	{
		try
		{
			var result = await MailService.DeleteMailAsync(mailUid);

			if (result == ErrorCode.None)
			{
				await RefreshMail();
			}
			else
			{
				ToastService.ShowError(result.ToString());
			}
		}
		catch (Exception ex)
		{
			ToastService.ShowError(ex.Message);
		}
	}

	private bool IsMailOpened()
	{
		return _selectedMail != null;
	}

	private void CloseMail()
	{
		_selectedMail = null;
		StateHasChanged();
	}

	private void ChangeMailType(MailType type)
	{
		CurrentMailType = type;
		StateHasChanged();
	}

	private async Task RefreshMail()
	{
		try
		{
			UserMailList.Clear();

			var result = await MailService.GetMailsAsync();

			if (result == ErrorCode.None)
			{
				UserMailList = MailService.MailList;
			}
			else
			{
				ToastService.ShowError(result.ToString());
			}

			StateHasChanged();
		}
		catch (Exception ex)
		{
			ToastService.ShowError(ex.Message);
		}

	}
}