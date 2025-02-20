using ContentDLL.Scenario;
using ContentDLL;
using System.Text.Json;
using ContentDLL.DTO;


namespace ContentDLL;

public class HttpRequest
{
    public static async Task<(ErrorCode, TRes)> Request<TRes>(DummyObject dummy, 
                            string sceneName, 
                            string requestAPI, string jsonRequest) where TRes : BaseResponse
    {
        ScenarioResultRecorder.StartScene(sceneName);
        var sceneMessage = $"{sceneName}. UserID: {dummy.UserID}";


        var _client = new HttpClient();
        
        ErrorCode error = ErrorCode.None;
        TRes responseDTO = null;

        try
        {
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(requestAPI, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                responseDTO = JsonSerializer.Deserialize<TRes>(jsonResponse);

                if(responseDTO == null)
                {
                    error = ErrorCode.JsonDeserializeFailed;
                }
                else
                {
                    if (responseDTO.Result != ErrorCode.None)
                    {
                        error = responseDTO.Result;
                    }
                }                
            }
            else
            {
                error = ErrorCode.HiveCreateAccountRequestFaile;
            }
        }
        catch (Exception ex)
        {
            sceneMessage = ex.Message;
            error = ErrorCode.HiveCreateAccountException;
        }
        finally
        {
            ScenarioResultRecorder.EndScene(error, sceneMessage);
        }

        return (error, responseDTO);
    }
}


