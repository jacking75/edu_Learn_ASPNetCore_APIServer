using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using APIServer.ModelDB;
using APIServer.ModelReqRes;
using APIServer.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class GetMailList : ControllerBase
{
    private readonly Int32 PerPageCount = 20;

    private readonly ILogger<GetMailList> _logger;

    private readonly IGameDb _gameDb;


    public GetMailList(ILogger<GetMailList> logger, IGameDb gameDb)
    {
        _logger = logger;
        _gameDb = gameDb;
    }


    [HttpPost]
    public async Task<GetMailListResponse> Post(GetMailListRequest request)
    {
        var response = new GetMailListResponse();
        var userInfo = (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;


        // 우편함을 처음 열었을 때만 전체 페이지를 알려준다.
        if (IsFirstOpen(request.OpenPage) == true)
        {
            var mailCount = await _gameDb.GetUserMailCount(userInfo.UserId);
            response.PageCount = GetTotalPageCount(mailCount);

            // 메일이 없다면 여기서 리턴
            if (mailCount == 0)
            {
                return response;
            }
        }


        // 페이지에 해당하는 메일 리스트 출력
        var (error, mailList) = await LoadUserMailList(userInfo.UserId, request.OpenPage);
        if (error != ErrorCode.None)
        {
            response.Result = error;
            return response;
        }




        response.MailList = mailList;
        return response;
    }


    private bool IsFirstOpen(Int32 pageNumber) => (pageNumber == 1) ? true : false;


    private Int32 GetTotalPageCount(Int32 totalMailCount)
    {
        Int32 pageCount = totalMailCount / PerPageCount;
        if (pageCount == 0)
        {
            return 1;
        }

        if (totalMailCount % PerPageCount >= 1)
        {
            pageCount += 1;
        }

        return pageCount;
    }


    private async Task<(ErrorCode, List<UserMail>)> LoadUserMailList(Int64 userId, Int32 page)
    {
        var (error, loadedDatas) = await _gameDb.GetUserMailList(userId, page, PerPageCount);
        if (error != ErrorCode.None)
        {
        }

        return (error, loadedDatas);
    }



}
