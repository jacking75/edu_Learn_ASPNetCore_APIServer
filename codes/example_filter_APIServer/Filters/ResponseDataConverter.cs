/**
 * 
 * 클라이언트가 수신 받아야하는 Response DTO의 구조가 다음과 같을 때
 * {
 *     "seq": 1000,
 *     "userUid": 0,
 *     "sessionKey": "",
 *     "apiName": "",
 *     "apiVer": 1,
 *     "timestamp": 1687415716,
 *     "result": {
 *         "code": 200,
 *         "msg": "Sucess"
 *     },
 *     "returnData": {}
 * }
 * 
 * 공통 부분인 "seq", "userUid", "sessionKey", "apiName", "apiVer", "timestamp"는 Gateway 미들웨어에서 HttpContext.Items에 저장했던
 * 데이터를 읽어와 넣어준다.
 * 
 * 이후 returnData 부분은 각각의 컨트롤러가 반환하는 Response DTO를 넣어준다.
 * 
 */

using System;

using APIServer.Models.DTO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APIServer.Filters;

public class ResponseDataConverter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        var contextResult = context.Result as ObjectResult;

        var reqCommonDTO = context.HttpContext.Items[nameof(ReqCommonDTO)] as ReqCommonDTO;
        var returnData = contextResult.Value;
        var resultDTO = Convert(reqCommonDTO, returnData);

        // 클라이언트에게 반환해줄 DTO의 Value를 덮어쓴다.
        contextResult.Value = resultDTO;
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }

    private object Convert(ReqCommonDTO reqDTO, object originalData)
    {
        return new ResCommonDTO()
        {
            seq = reqDTO.seq,
            userUid = reqDTO.userUid,
            sessionKey = reqDTO.sessionKey,
            apiName = reqDTO.apiName,
            apiVer = reqDTO.apiVer,
            timestamp = GetTimestamp(),
            returnData = originalData
        };
    }

    private Int64 GetTimestamp()
    {
        var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);

        return (Int64)timeSpan.TotalSeconds;
    }
}