using GameServer.Models;
using GameShared;

namespace GameServer.Repositories.Interfaces;

public interface IMasterDb
{
	public VersionDAO _version { get; }
	public List<Item> _items { get; }
	public List<Reward> _rewards { get; }
	public List<Attendance> _attendances { get; }

	public Task<bool> Load();
	public Item? GetItemById(int itemId);
	public List<(Item, int)> GetItemsByRewardCode(int rewardCode);
	public int GetRewardCodeByAttendance(int attendanceCode, int attendanceCount);
}
