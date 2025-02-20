
namespace ContentDLL.Scenario;

public class GameScenarioConfig
{
    public Int32 IsLogWriteConsole { get; set; } = 0;

    public Int32 DummyCount { get; set; } = 0;

    public string ServerUrl { get; set; }
    
    public string HiveDBConnectionString { get; set; }
    
}
