using System;
using System.Threading.Tasks;

using APIServer.ModelDB;
using APIServer.ModelReqRes;
using APIServer.Services;
using APIServer.Services.MasterData;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class AttendanceCheck : ControllerBase
{
    private readonly ILogger<AttendanceCheck> _logger;

    private readonly IGameDb _gameDb;

    private readonly MasterDataManager _masterDataMgr;

    public AttendanceCheck(ILogger<AttendanceCheck> logger, IGameDb gameDb, MasterDataManager masterDataMgr)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDataMgr = masterDataMgr;
    }


    [HttpPost]
    public async Task<AttendanceCheckResponse> Post(AttendanceCheckRequest request)
    {
        var response = new AttendanceCheckResponse();
        var userInfo = (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;
        var userId = userInfo.UserId;


        // 유저의 출석부 읽어오기
        var attendanceBook = await LoadAttendanceBook(userInfo.UserId);
        if (attendanceBook is null)
        {
            response.Result = ErrorCode.NotExistUserAttendanceBook;
            return response;
        }


        // 이미 최대 날짜까지 출석 체크를 끝낸 유저인가?
        var lastAttendanceDay = attendanceBook.last_attendance_day;
        if (IsAlreadyMaxDay(lastAttendanceDay) == true)
        {
            response.Result = ErrorCode.InvalidAttendanceDay;
            return response;
        }


        // 출석일 갱신
        var (error, updatedAttendanceDay) = await UpdateAttendanceDay(userId, lastAttendanceDay);
        if (error != ErrorCode.None)
        {
            response.Result = ErrorCode.FailedUpdateUserAttendanceBook;
            return response;
        }


        // 출석 보상 지급
        if (await SendRewardMail(userId, updatedAttendanceDay) == false)
        {
            await Rollback(userId, attendanceBook);
            response.Result = ErrorCode.FailedAddUserMail;
            return response;
        }




        return response;
    }


    private async Task<UserAttendanceBook> LoadAttendanceBook(Int64 userId)
    {
        var (_, loadedData) = await _gameDb.GetUserAttendanceBookNotToday(userId, DateTime.Now.Date);

        if (loadedData is null)
        {
        }

        return loadedData;
    }


    private bool IsAlreadyMaxDay(Int16 day) => DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) == day;


    private readonly Int16 FirstDay = 1;
    private bool IsFirstAttendance(Int16 day) => FirstDay == day;


    private async Task<(ErrorCode, Int16)> UpdateAttendanceDay(Int64 userId, Int16 lastAttendanceDay)
    {
        var updatedDay = ++lastAttendanceDay;

        if (IsFirstAttendance(updatedDay) == true)
        {
            return (await _gameDb.ResetUserAttendanceBook(userId), updatedDay);
        }

        return (await _gameDb.UpdateUserAttendanceDay(userId, updatedDay), updatedDay);
    }


    private readonly Int16 AttendanceRewardExpireDay = 30;
    private UserMail CreateRewardMail(AttendanceReward rewardInfo)
    {
        return new UserMail()
        {
            mail_type = (Int16)MasterDataCode.MailCode.이벤트상품,
            mail_title = $"{rewardInfo.days} 일차 보상입니다.",
            item_code = rewardInfo.item_code,
            item_count = rewardInfo.item_count,
            expire_date = DateTime.Now.AddDays(AttendanceRewardExpireDay),
        };
    }


    private async Task<bool> SendRewardMail(Int64 userId, Int16 day)
    {
        var rewardInfo = _masterDataMgr.GetAttendanceReward(day);
        var rewardMail = CreateRewardMail(rewardInfo);

        if (await _gameDb.AddUserMail(userId, rewardMail) == false)
        {
            return false;
        }

        return true;
    }


    private async Task Rollback(Int64 userId, UserAttendanceBook originAttendanceBook)
    {
        await _gameDb.UpdateUserAttendanceDayAndDate(userId, originAttendanceBook.last_attendance_day, originAttendanceBook.last_update_date);
    }



}