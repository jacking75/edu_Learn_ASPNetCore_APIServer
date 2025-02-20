
namespace ContentDLL.Scenario;

public abstract class ScenarioBase
{
    public string Name = "";
    
    public GameScenarioConfig _gameConfig;


    public abstract Task<bool> Action(DummyManager dummyMgr);

}


