using HiveAPIServer.Model.DAO;
using System;
using System.Threading.Tasks;
using ZLogger;
using HiveAPIServer.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace HiveAPIServer.Services
{
	public class HiveService : IHiveService
	{
		private readonly ILogger<HiveService> _logger;
		private readonly IHiveDb _hiveDb;
		readonly string _saltValue;

		public HiveService(ILogger<HiveService> logger, IHiveDb hiveDb, IConfiguration config)
		{
			_logger = logger;
			_hiveDb = hiveDb;
			_saltValue = config.GetSection("TokenSaltValue").Value;
		}

		public async Task<ErrorCode> VerifyToken(Int64 playerId, string token)
		{
			try
			{
				var tokenInfo = await _hiveDb.SelectAsync<Token, Int64>("token", "player_id", playerId);

				if (null == tokenInfo)
				{
					return ErrorCode.HiveTokenNotFound;
				}

				if (tokenInfo.token != token)
				{
					return ErrorCode.HiveTokenMismatch;
				}

				if (DateTime.TryParse(tokenInfo.expire_dt, out DateTime expireDateTime) &&
					expireDateTime < DateTime.Now)
				{
					return ErrorCode.HiveTokenExpired;
				}

				var hashingToken = Security.MakeHashingToken(_saltValue, playerId);
				if (token != hashingToken)
				{
					return ErrorCode.HiveTokenInvalid;
				}

				return ErrorCode.None;
			}
			catch (Exception e)
			{
				_logger.ZLogError(e,
				$"[HiveService.VerifyToken] PlayerId:{playerId} ErrorCode: {ErrorCode.HiveVerifyTokenFailException}");
				return ErrorCode.HiveVerifyTokenFailException;
			}
		}

		public async Task<(ErrorCode, Token)> LoginHive(string email, string pw)
		{
			var (result, playerId) = await VerifyUser(email, pw);

			if (result != ErrorCode.None)
			{
				_logger.ZLogError($"[LoginHive.VerifyUser] email: {email} ErrorCode: {result}");
				return (result, null);
			}

			(result, var token) = await CreateToken(playerId);
			if (result != ErrorCode.None)
			{
				_logger.ZLogError($"[LoginHive.CreateTokenAsync] email: {email} ErrorCode: {result}");
				return (result, null);
			}

			return (result, new Token { 
			player_id = playerId,
			token = token
			});
		}

		public async Task<ErrorCode> CreateAccount(string email, string pw)
		{
			var saltValue = Security.SaltString();
			var hashingPassword = Security.MakeHashingPassWord(saltValue, pw);

			try
			{
				var result = await _hiveDb.CreateAsync("account", new 
				{
					email = email,
					salt = saltValue,
					pw = hashingPassword,
					create_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
				});

				if (false == result)
				{
					_logger.ZLogError($"[CreateAccount] email: {email}, salt : {saltValue}, hashed_pw:{hashingPassword}");
					return ErrorCode.HiveCreateAccountFail;
				}

				_logger.ZLogDebug( $"[CreateAccount] email: {email}, salt : {saltValue}, hashed_pw:{hashingPassword}");
				return ErrorCode.None;
			}
			catch (Exception e)
			{
				_logger.ZLogError(e,
				$"[AccoutDb.CreateAccount] ErrorCode: {ErrorCode.HiveCreateAccountException}");
				return ErrorCode.HiveCreateAccountException;
			}
		}

		private async Task<(ErrorCode, string)> CreateToken(Int64 playerId)
		{
			try
			{
				var token = Security.MakeHashingToken(_saltValue, playerId);
				var tokenInfo = new Token
				{
					player_id = playerId,
					token = token,
					expire_dt = DateTime.Now.AddHours(1).ToString("yyyy/MM/dd HH:mm:ss"),
				};

				if (false == await _hiveDb.UpsertToken(tokenInfo))
				{
					return (ErrorCode.HiveSaveTokenFail, null);
				}
		
				return (ErrorCode.None, token);
			}
			catch (Exception e)
			{
				_logger.ZLogError(e, $"[HiveService.SaveTokenException] PlayerId: {playerId}");
				return (ErrorCode.HiveSaveTokenException, null);
			}
		}

		private async Task<(ErrorCode, Int64)> VerifyUser(string email, string pw)
		{
			try
			{
				var userInfo = await _hiveDb.SelectAsync<Account, string>("account", "email", email);
				if (null == userInfo)
				{
					return (ErrorCode.HiveLoginFailUserNotFound, 0);
				}

				var hashingPassword = Security.MakeHashingPassWord(userInfo.salt, pw);
				if (userInfo.pw != hashingPassword)
				{
					return (ErrorCode.HiveLoginFailPasswordInvalid, 0);
				}

				return (ErrorCode.None, userInfo.player_id);
			}
			catch (Exception e)
			{
				_logger.ZLogError(e,
				$"[HiveService.VerifyUser] Email:{email} ErrorCode: {ErrorCode.HiveLoginFailException}");
				return (ErrorCode.HiveLoginFailException, 0);
			}
		}
	}
}
