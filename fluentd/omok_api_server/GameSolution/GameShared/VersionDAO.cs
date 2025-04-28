namespace GameShared;

public class VersionDAO
{
	public string app_version { get; set; } = "";
	public string master_data_version { get; set; } = "";

	public static readonly string Table = "version";
}