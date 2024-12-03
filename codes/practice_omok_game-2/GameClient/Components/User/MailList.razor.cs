using GameClient.Providers;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace GameClient.Components.User;

public partial class MailList
{
	[Inject]
	private IToastService ToastService { get; set; }
	[Inject]
	private MailStateProvider MailStateProvider { get; set; }
	[Inject]
	private InventoryStateProvider InventoryStateProvider { get; set; }

	private List<MailInfo>? _list { get; set; } = null;

	private List<MailInfo>? _currentList => _list?.Where(e => e.Type == CurrentMailType).ToList();

	private MailDisplayData? _selectedMail { get; set; } = null;

	private MailType CurrentMailType { get; set; } = MailType.System;

	protected override async Task OnInitializedAsync()
	{
		await RefreshMail();
	}

	private async Task ReadMail(Int64 mailUid)
	{
		try
		{
			var (result, mail) = await MailStateProvider.ReadMailAsync(mailUid);

			if (result == ErrorCode.None)
			{
				_selectedMail = mail;
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
			var result = await MailStateProvider.ReceiveMailAsync(mailUid);

			if (result == ErrorCode.None)
			{
				await RefreshMail();
				await InventoryStateProvider.GetUserItemsAsync();
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
			var result = await MailStateProvider.DeleteMailAsync(mailUid);

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
			var (result, list) = await MailStateProvider.GetMailsAsync();

			if (result == ErrorCode.None)
			{
				this._list = list;
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
}
