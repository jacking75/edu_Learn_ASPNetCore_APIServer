using GameClient.Providers;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace GameClient.Components.User;

public partial class AttendanceList
{
	private Attendance? _attendance;
	private AttendanceInfo? _current;

	[Inject]
	private AttendanceProvider AttendanceProvider { get; set; }
	[Inject]
	private GameContentProvider GameContentProvider { get; set; }
	[Inject]
	private LoadingStateProvider LoadingStateProvider { get; set; }
	[Inject]
	private MailStateProvider MailStateProvider { get; set; }

	[Inject]
	private IToastService ToastService { get; set; }


	protected override void OnInitialized()
	{
		if (null == GameContentProvider.GameData ||
			null == GameContentProvider.GameData.Attendances)
				return;

		_attendance = GameContentProvider.GameData.Attendances.First();
		_current = AttendanceProvider.GetAttendanceInfo(_attendance.AttendanceCode);
	}

	private async Task AttendAsync()
	{
		try
		{
			LoadingStateProvider.SetLoading(true);

			if (null == _attendance)
				return;
	
			var response = await AttendanceProvider.AttendAsync();
	
			if (ErrorCode.None != response)
			{
				ToastService.ShowError(response.ToString());
			}
			else
			{
				await MailStateProvider.GetMailsAsync();
				ToastService.ShowSuccess("Attendance success!");
			}
		}
		catch (Exception ex)
		{
			ToastService.ShowError(ex.Message);
		}
		finally
		{
			LoadingStateProvider.SetLoading(false);
		}

	}
}
