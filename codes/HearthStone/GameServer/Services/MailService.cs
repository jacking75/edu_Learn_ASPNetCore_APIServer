using GameServer.Models.DTO;
using GameServer.Services.Interface;
using GameServer.Repository.Interface;
using ZLogger;
using Humanizer;
using GameServer.Repository.Interface;
using GameServer.Models;
namespace GameServer.Services;

public class MailService : IMailService
{
    readonly ILogger<MailService> _logger;
    readonly IGameDb _gameDb;

    public MailService(ILogger<MailService> logger, IGameDb gameDb)
    {
        _logger = logger;
        _gameDb = gameDb;
    }

    public async Task<(ErrorCode, List<MailInfo>)> GetMailInfoList(Int64 accountUid)
    {
        try
        {
            return (ErrorCode.None, await _gameDb.GetMailList(accountUid));
        }
        catch (Exception e)
        {
            _logger.ZLogError($"[ReadMail] ErrorCode: {ErrorCode.MailInfoFailException}, uid : {accountUid}");
            return (ErrorCode.MailInfoFailException, null);
        }
    }
    public async Task<ErrorCode> ReadMail(Int64 accountUid, Int64 mailId)
    {
        try
        {
            if(await _gameDb.ReceiveMail(accountUid, mailId) < 1)
            {
                _logger.ZLogError($"[ReadMail] ErrorCode: {ErrorCode.MailInfoFailException}, uid : {accountUid}");
                return ErrorCode.MailInfoFailException;
            }

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError($"[ReadMail] ErrorCode: {ErrorCode.MailInfoFailException}, uid : {accountUid}");
            return ErrorCode.MailInfoFailException;
        }   
    }
    public async Task<ErrorCode> DeleteMail(Int64 accountUid, Int64 mailId) 
    {
        try
        {
            if (await _gameDb.DeleteMail(accountUid, mailId) < 1)
            {
                _logger.ZLogError($"[DeleteMail] ErrorCode: {ErrorCode.MailInfoFailException}, uid : {accountUid}");
                return ErrorCode.MailInfoFailException;
            }
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError($"[DeleteMail] ErrorCode: {ErrorCode.MailInfoFailException}, uid : {accountUid}");
            return ErrorCode.MailInfoFailException;
        }
    }
}
