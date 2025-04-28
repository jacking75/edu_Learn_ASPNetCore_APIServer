namespace GameShared.DTO;

public class MasterDataRequest;

public class MasterDataResponse : ErrorCodeDTO
{
	public LoadedMasterData MasterData { get; set; }
}

public class LoadedMasterData
{
	public IEnumerable<Item>? Items { get; set; }
	public IEnumerable<Reward>? Rewards { get; set; }
	public IEnumerable<Attendance>? Attendances { get; set; }
}
