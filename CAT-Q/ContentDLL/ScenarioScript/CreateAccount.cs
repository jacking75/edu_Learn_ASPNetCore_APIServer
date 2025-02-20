using ContentDLL.Scenario;
using System.Text.Json;


namespace ContentDLL.ScenarioScript;

// 시나리오 설명:
// 1. 더미가 서버에 접속하고 연결을 유지한다.
// 2. 시나리오가 종료되면 연결을 해제한다.


public class CreateAccount : ScenarioBase
{
    public override async Task<bool> Action(DummyManager dummyMgr)
    {
        var scenarionName = "CreateAccount";
        ScenarioResultRecorder.StartScenario(scenarionName);
        
        string scenarioMessage = "";
        bool isSuccess = false;

        try
        {
            _gameConfig = dummyMgr.ScenarioConfig;
            var dummy = dummyMgr.GetDummyByIndex(0);
            
            // Scene 1 - 성공적으로 계정 생성을 한다
            var result = await SceneHiveCreateAccount(dummy);
            if (result != ErrorCode.None)
            {
                ScenarioResultRecorder.EndScenario(scenarionName, isSuccess, "fail");
                return false;
            }

            isSuccess = true;
        }
        catch(Exception ex)
        {
            scenarioMessage = $"Error during OnlyConnectScenario: {ex.Message}";
        }
        finally
        {
            ScenarioResultRecorder.EndScenario(scenarionName, isSuccess, scenarioMessage);
        }   

        return isSuccess;        
    }


    async Task<ErrorCode> SceneHiveCreateAccount(DummyObject dummy)
    {
        string api = "/CreateAccount";
        var sceneName = "CreateAccount - Success ver.";        
        var sceneMessage = $"CreateAccount. UserID: {dummy.UserID}";

        
        var requestAPI = _gameConfig.ServerUrl + api;

        var registerRequest = new DTO.CreateHiveAccountRequest
        {
            UserID = dummy.UserID,
            Password = "5GZF7OFY05P4TT"
        };

        var jsonRequest = JsonSerializer.Serialize(registerRequest);

        var (errorCode, response) = await HttpRequest.Request<DTO.CreateHiveAccountResponse>(dummy, sceneName, requestAPI, jsonRequest);
        
        return errorCode;
    }
   

}

