using ContentDLL.Scenario;
using ContentDLL.ScenarioScript;


namespace ContentDLL;

public class ScenarioList
{
    static public List<ScenarioBase> Create()
    {
        var scenarioList = new List<ScenarioBase>();
        scenarioList.Add(new CreateAccount());
        scenarioList.Add(new Login());


        Console.WriteLine($"시나리오 개수: {scenarioList.Count}");
        return scenarioList;
    }
}
