using System;
using System.Threading.Tasks;

using APIServer.ModelDB;
using APIServer.ModelReqRes;
using APIServer.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class GetAttendanceBook : ControllerBase
{
    readonly ILogger<GetAttendanceBook> _logger;

    readonly IGameDb _gameDb;

    public GetAttendanceBook(ILogger<GetAttendanceBook> logger, IGameDb gameDb)
    {
        _logger = logger;
        _gameDb = gameDb;
    }

    [HttpPost]
    public async Task<GetAttendanceBookResponse> Post(GetAttendanceBookRequest request)
    {
        var response = new GetAttendanceBookResponse();
        var authUser = (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;
        var userId = authUser.UserId;


        // 유저 출석부 읽어오기
        var attendanceBook = await LoadAttendanceBook(userId);
        if (attendanceBook is null)
        {
            response.Result = ErrorCode.NotExistUserAttendanceBook;
            return response;
        }


        // 출석 체크한 적이 없다면 리턴
        if (attendanceBook.last_attendance_day == 0)
        {
            return response;
        }


        // 출석부 초기화 여부 확인
        if (IsTimeOverFromLastDate(attendanceBook.last_update_date) == true
            || IsTimeOverFromStartDate(attendanceBook.start_update_date) == true)
        {
            // 출석부 초기화
            if (await ResetAttendanceBook(userId) == false)
            {
                response.Result = ErrorCode.FailedUpdateUserAttendanceBook;
                return response;
            }

            response.LastAttendanceDay = 0;
        }

        response.LastAttendanceDay = attendanceBook.last_attendance_day;




        return response;
    }


    private readonly Int16 LastAttendanceTimeOver = 1;
    private bool IsTimeOverFromLastDate(DateTime date)
    {
        var timeSpan = DateTime.Now - date;
        if ((Int32)(timeSpan.TotalDays) > LastAttendanceTimeOver)
        {
            return true;
        }

        return false;
    }


    private readonly Int16 StartAttendanceTimeOver = 1;
    private bool IsTimeOverFromStartDate(DateTime date)
    {
        var timeSpan = DateTime.Now - date;
        if ((Int32)(timeSpan.TotalDays) > StartAttendanceTimeOver)
        {
            return true;
        }

        return false;
    }


    private async Task<UserAttendanceBook> LoadAttendanceBook(Int64 userId)
    {
        var (error, loadedData) = await _gameDb.GetUserAttendanceBook(userId);
        if (error != ErrorCode.None)
        {
        }

        return loadedData;
    }


    private async Task<bool> ResetAttendanceBook(Int64 userId)
    {
        if (await _gameDb.ResetUserAttendanceBook(userId) != ErrorCode.None)
        {
            return false;
        }

        return true;
    }



}