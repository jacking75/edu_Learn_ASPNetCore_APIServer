
namespace ContentDLL.Scenario;

public class ScenarioResultRecorder
{
    static Int32 _scenarioCount = 0;
    static bool _isWriteConsole = true;
    static List<ScenasrioResult> _scenasrioResults = new();


    static public void Init(bool isWriteConsole)
    {
        _isWriteConsole = isWriteConsole;
    }

    static public void StartScenario(string name)
    {
        ++_scenarioCount;
                
        var result = new ScenasrioResult
        {
            Name = name,            
        };

        _scenasrioResults.Add(result);

        var logMessage = $"[{_scenarioCount}] Start Scenario - {name}";
        ConsoleWrite(logMessage);
    }

    static public void EndScenario(string name, bool isSuccess, string message)
    {
        var _scenasrioResult = _scenasrioResults.Last();
        _scenasrioResult.IsSuccess = isSuccess;
        _scenasrioResult.IsCompleted = true;
        _scenasrioResult.Message = message;

        var logMessage = $"End Scenario - {name}. isSuccess:{isSuccess}, {message}";
        ConsoleWrite(logMessage);
    }



    static public void StartScene(string name)
    {
        var result = new SceneResult
        {
            Name = name,
        };
        
        var scenasrioResult = _scenasrioResults.Last();
        scenasrioResult.SceneResults.Add(result);

        ConsoleWrite($" - Start Scene [{name}]");
    }

    static public void EndScene(ErrorCode errorCode, string message ) 
    {
        var scenasrioResult = _scenasrioResults.Last();
        var sceneResult = scenasrioResult.SceneResults.Last();

        sceneResult.IsCompleted = true;
        sceneResult.ErrorCode = errorCode;
        sceneResult.Message = message;  
        
        ConsoleWrite($"   End Scene. ErrorCode:{errorCode}, {message}");
    }


    static void ConsoleWrite(string message)
    {
        if (_isWriteConsole)
        {
            Console.WriteLine(message);
        }
    }


    
}


