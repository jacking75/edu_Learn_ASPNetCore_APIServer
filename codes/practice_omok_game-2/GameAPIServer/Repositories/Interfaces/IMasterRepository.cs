using GameAPIServer.Models.MasterDb;

namespace GameAPIServer.Repositories.Interfaces
{
	public interface IMasterRepository : IDisposable
	{
		public VersionDAO _version { get; }
		public List<Item> _items { get; }
		public List<Money> _money { get; }
		public List<Reward> _rewards { get; }
		public List<Attendance> _attendances { get; }

		public Task<bool> Load();
		public int GetAttendanceRewardCode(int attendanceCode, int attendanceCount);
		public List<(Item, int)> GetRewardItems(int rewardCode);

		public Item? GetItem(int itemId);

	}
}
