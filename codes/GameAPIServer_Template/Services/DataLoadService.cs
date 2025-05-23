﻿using GameAPIServer.Servicies.Interfaces;
using GameAPIServer.Models.DTO;
using System.Threading.Tasks;

namespace GameAPIServer.Servicies;

public class DataLoadService : IDataLoadService
{
    readonly IGameService _gameService;
    readonly IFriendService _friendService;
    readonly IUserService _userService;
    readonly IItemService _itemService;
    readonly IMailService _mailService;
    readonly IAttendanceService _attendanceService;

    public DataLoadService(IMailService mailService, IAttendanceService attendanceService, IUserService userService, IItemService itemService, IGameService gameService, IFriendService friendService)
    {
        _mailService = mailService;
        _attendanceService = attendanceService;
        _userService = userService;
        _itemService = itemService;
        _gameService = gameService;
        _friendService = friendService;
    }

    public async Task<(ErrorCode, DataLoadUserInfo)> LoadUserData(int uid)
    {
        DataLoadUserInfo loadData = new();
        (var errorCode, loadData.UserInfo) = await _userService.GetUserInfo(uid);
        if (errorCode != ErrorCode.None)
        {
            return (errorCode,null);
        }

        (errorCode, loadData.MoneyInfo) = await _userService.GetUserMoneyInfo(uid);
        if (errorCode != ErrorCode.None)
        {
            return (errorCode, null);
        }

        (errorCode, loadData.AttendanceInfo) = await _attendanceService.GetAttendanceInfo(uid);
        if (errorCode != ErrorCode.None)
        {
            return (errorCode, null);
        }

        return (ErrorCode.None, loadData);
    }

    public async Task<(ErrorCode, DataLoadGameInfo)> LoadGameData(int uid)
    {
        DataLoadGameInfo loadData = new();

        //TODO 게임 데이터 로딩을 구현한다
        await Task.CompletedTask;

        return (ErrorCode.None, loadData);
    }

    public async Task<(ErrorCode, DataLoadSocialInfo)> LoadSocialData(int uid)
    {
        DataLoadSocialInfo loadData = new();

        (var errorCode, loadData.MailList) = await _mailService.GetMailList(uid);
        if (errorCode != ErrorCode.None)
        {
            return (errorCode, null);
        }

        (errorCode, loadData.FriendList) = await _friendService.GetFriendList(uid);
        if (errorCode != ErrorCode.None)
        {
            return (errorCode, null);
        }

        return (ErrorCode.None, loadData);
    }
}
