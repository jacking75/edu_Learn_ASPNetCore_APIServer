using System.Net.Http.Json;
namespace GameClient.Providers;

public class AttendanceProvider
{
	private readonly IHttpClientFactory _httpClientFactory;
	private IEnumerable<AttendanceInfo>? _attendanceInfos;


	public AttendanceProvider(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task<ErrorCode> AttendAsync()
	{
		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsync("/attendance", null);

			if (!response.IsSuccessStatusCode)
			{
				return ErrorCode.AttendanceUpdateBadRequest;
			}

			var result = await response.Content.ReadFromJsonAsync<AttendanceResponse>();
			return result.Result;
		}
		catch (Exception e)
		{
			return ErrorCode.AttendanceUpdateException;
		}
	}

	public void SetAttendanceInfos(IEnumerable<AttendanceInfo>? attendanceInfos)
	{
		_attendanceInfos = attendanceInfos;
	}

	public IEnumerable<AttendanceInfo>? GetAttendanceInfos()
	{
		return _attendanceInfos;
	}

	public AttendanceInfo? GetAttendanceInfo(int attendanceCode)
	{
		if (_attendanceInfos == null)
		{
			return null;
		}

		foreach (var info in _attendanceInfos)
		{
			if (info.AttendanceCode == attendanceCode)
			{
				return info;
			}
		}

		return null;
	}
}
