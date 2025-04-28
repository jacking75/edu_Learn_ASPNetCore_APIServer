using GameShared.DTO;
using System.Net.Http.Json;

namespace GameClient.Services;

public class DataLoadService
{
    private readonly IHttpClientFactory _httpClientFactory;
	public LoadedUserData? UserData { get; private set; }
	public List<Item> LoadedItems { get; private set; }
    public List<Reward> LoadedRewards { get; private set; }  
	public Attendance LoadedAttendance { get; private set; }

	public DataLoadService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> LoadMasterDataAsync()
    {
        try
        {
            var gameClient = _httpClientFactory.CreateClient("Game");
            var response = await gameClient.PostAsync("/dataload/master", null);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MasterDataResponse>();

                if (result == null || ErrorCode.None != result.Result)
                {
                    return false;
                }

                if (result.MasterData.Items != null )
                {
					LoadedItems = result.MasterData.Items.ToList();
                }

                if (result.MasterData.Rewards != null)
                {
                    LoadedRewards = result.MasterData.Rewards.ToList();
                }

                if (result.MasterData.Attendances != null)
                {
                   LoadedAttendance = result.MasterData.Attendances.FirstOrDefault();
				}
                
				return true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

        return false;
    }

    public async Task<bool> LoadUserDataAsync()
    {
        try
        {
            var gameClient = _httpClientFactory.CreateClient("Game");
            var response = await gameClient.PostAsync("/dataload", null);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UserDataResponse>();

                if (result == null || ErrorCode.None != result.Result)
                {
                    return false;
                }

                UserData = result.UserData;
				return true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

        return false;
    }

	public async Task<(ErrorCode, UserInfo? user)> LoadUserInfo(Int64 uid)
	{
		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsJsonAsync("dataload/info", new UserInfoRequest
            {
				Uid = uid
			});

			if (!response.IsSuccessStatusCode)
			{
				return (ErrorCode.GetUserInfoFail, null);
			}

			var result = await response.Content.ReadFromJsonAsync<UserDataResponse>();

			if (null == result)
			{
				return (ErrorCode.GetUserInfoFail, null);
			}

			return (ErrorCode.None, result.UserData?.UserInfo);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return (ErrorCode.GetUserInfoException, null);
		}
	}
	public async Task<ErrorCode> UpdateNicknameAsync(string nickname)
	{
		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsJsonAsync("user/nickname", new NicknameUpdateRequest
			{
				Nickname = nickname
			});

			if (!response.IsSuccessStatusCode)
			{
				return ErrorCode.UpdateUserFailBadRequest;
			}

			var result = await response.Content.ReadFromJsonAsync<NicknameUpdateResponse>();
			return result.Result;
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return ErrorCode.UpdateUserException;
		}
	}

	public Item? GetItem(int itemId)
    {
        return LoadedItems.Find(LoadedItems => LoadedItems.ItemId == itemId);  
	}

	public List<(Item, int)> GetItemsFromRewardCode(int rewardCode)
	{
		var items = new List<(Item, int)>();

		if (null == LoadedItems || null == LoadedRewards)
			return items;

        var rewards = LoadedRewards.Where(x => x.RewardCode == rewardCode);
        foreach (var reward in rewards)
        {
			var template = LoadedItems.FirstOrDefault(x => x.ItemId == reward.ItemId);

			if (null == template)
				continue;

			items.Add((template, reward.ItemCount));
		}

        return items;
	}

	public int GetAttendanceCount(int attendanceCode)
	{
		if (null == UserData?.UserAttendanceInfo)
			return 0;

		return UserData.UserAttendanceInfo.FirstOrDefault(e => e.AttendanceCode == attendanceCode)?.AttendanceCount ?? 0;
	}
}
