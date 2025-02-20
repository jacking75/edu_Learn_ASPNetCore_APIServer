using System.Text.Json;


namespace ContentDLL.Scenario;

public class ScenarioRunner
{
    private DummyManager _dummyMgr = new();
    
    private DateTime _startTime;
    private DateTime _endTime;

    GameScenarioConfig _config;


    public void Init(string parameters)
    {
        var options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true
        };
        GameScenarioConfig config = JsonSerializer.Deserialize<GameScenarioConfig>(parameters, options);
        if (config == null)
        {
            Console.WriteLine("[ERROR] ScenarioRunner: StartScenario: GameScenarioConfig is null");
            return;
        }


        if(config.IsLogWriteConsole == 1)
        {
            ScenarioResultRecorder.Init(true);
        }
        else
        {
            ScenarioResultRecorder.Init(false);
        }

        _config = config;
        _dummyMgr.Init(config);
    }
    
    
    public async Task<bool> Run()
    {
        bool isSuccess = false;

        Prepare();

        try
        {            
            var scenarioList = ScenarioList.Create();

            foreach(var scenario in scenarioList)
            {
                var result = await scenario.Action(_dummyMgr);

                Console.WriteLine($"{scenario.Name} - 테스트 성공");
            }

            isSuccess = true;
            Console.WriteLine($"모든 테스트 성공 !!!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex} 이유로 테스트가 중단되었습니다.");
        }
        finally
        {
            Done(); // 테스트 종료 작업
        }

        return isSuccess;
    }

    
    private void Prepare()
    {
        _startTime = DateTime.Now;
    }

    private void Done()
    {
        _endTime = DateTime.Now;
    }

    
}
