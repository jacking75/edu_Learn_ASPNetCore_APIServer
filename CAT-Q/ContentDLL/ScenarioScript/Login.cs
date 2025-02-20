using ContentDLL.Scenario;
using System.Text.Json;

namespace ContentDLL;


public class Login : ScenarioBase
{
    public override async Task<bool> Action(DummyManager dummyMgr)
    {
        var scenarionName = "Login";
        ScenarioResultRecorder.StartScenario(scenarionName);

        string scenarioMessage = "";
        bool isSuccess = false;

        try
        {
            _gameConfig = dummyMgr.ScenarioConfig;
            var dummy = dummyMgr.GetDummyByIndex(0);
            
            // Scene 1 - 성공적으로 계정 생성을 한다
            var result = await SceneLogin(dummy);
            if (result != ErrorCode.None)
            {
                ScenarioResultRecorder.EndScenario(scenarionName, isSuccess, "fail");
                return false;
            }

            isSuccess = true;
        }
        catch (Exception ex)
        {
            scenarioMessage = $"Error during OnlyConnectScenario: {ex.Message}";
        }
        finally
        {
            ScenarioResultRecorder.EndScenario(scenarionName, isSuccess, scenarioMessage);
        }

        return isSuccess;
    }

    async Task<ErrorCode> SceneLogin(DummyObject dummy)
    {
        string api = "/Login";
        var sceneName = "Login - Success ver.";
        var sceneMessage = $"Login. UserID: {dummy.UserID}";

        var requestAPI = _gameConfig.ServerUrl + api;

        var registerRequest = new DTO.LoginHiveRequest
        {
            UserID = dummy.UserID,
            Password = "5GZF7OFY05P4TT"
        };

        var jsonRequest = JsonSerializer.Serialize(registerRequest);

        var (errorCode, response) = await HttpRequest.Request<DTO.LoginHiveResponse>(dummy, sceneName, requestAPI, jsonRequest);

        return errorCode;
    }


   
}



