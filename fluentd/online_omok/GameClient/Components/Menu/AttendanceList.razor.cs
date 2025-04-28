using GameClient.Providers;
using GameClient.Services;
using GameShared.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace GameClient.Components.Menu;

public partial class AttendanceList
{
	public int AttendanceCount { get; set; }
	public Attendance Attendance { get; set; }

	[Inject]
	 IToastService ToastService { get; set; }
	[Inject]
	LoadingStateProvider LoadingStateProvider { get; set; }
	[Inject]
	AttendanceService AttendanceService { get; set; }
	[Inject]
	DataLoadService DataLoadService { get; set; }
	[Inject]
	MailService MailService { get; set; }

	protected override void OnInitialized()
	{
		if (null == DataLoadService.LoadedAttendance)
			return;

		Attendance = DataLoadService.LoadedAttendance;
		AttendanceCount = DataLoadService.GetAttendanceCount(Attendance.AttendanceCode);
	}

	private async Task AttendAsync()
	{
		try
		{
			LoadingStateProvider.SetLoading(true);

			var response = await AttendanceService.AttendAsync();

			if (ErrorCode.None != response)
			{
				ToastService.ShowError(response.ToString());
			}
			else
			{
				await MailService.GetMailsAsync();
				await DataLoadService.LoadUserDataAsync();
				Attendance = DataLoadService.LoadedAttendance;
				AttendanceCount = DataLoadService.GetAttendanceCount(Attendance.AttendanceCode);
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