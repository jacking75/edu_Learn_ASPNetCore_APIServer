# MySQL 라이브러리에서 지원하는 트랜잭션 기능 사용하기
코드에서 트랜잭션 기능을 사용할 때는 아래 코드처럼 사용하면 된다   
```
public async Task<ErrorCode> AttendanceCheck(long playerUid)
{
    var lastAttendanceDate = await _gameDb.GetRecentAttendanceDate(playerUid);

    if (lastAttendanceDate.HasValue && lastAttendanceDate.Value.Date == DateTime.Today)
    {
        return ErrorCode.AttendanceCheckFailAlreadyChecked;
    }

    // 트랜잭션 시작
    var result = await _gameDb.ExecuteTransaction(async transaction =>
    {
        return await UpdateAttendanceInfoAndGiveReward(playerUid, transaction);
    });

    if (!result)
    {
        return ErrorCode.AttendanceCheckFailException;
    }

    return ErrorCode.None;
}

private async Task<bool> UpdateAttendanceInfoAndGiveReward(long playerUid, MySqlTransaction transaction)
{
    var updateResult = await _gameDb.UpdateAttendanceInfo(playerUid, transaction);
    if (!updateResult)
    {
        return false;
    }

    var attendanceCount = await _gameDb.GetTodayAttendanceCount(playerUid, transaction);
    if (attendanceCount == -1)
    {
        return false;
    }

    var rewardResult = await _gameDb.AddAttendanceRewardToMailbox(playerUid, attendanceCount, transaction);
    if (!rewardResult)
    {
        return false;
    }

    return true;
}



classs GameDb
{
	MySqlConnection _connection;
	readonly QueryFactory _queryFactory;


	public async Task<bool> ExecuteTransaction(Func<MySqlTransaction, Task<bool>> operation)
	{
		using var transaction = await _connection.BeginTransactionAsync();
		try
		{
			var result = await operation(transaction);
			if (result)
			{
				await transaction.CommitAsync();
				return true;
			}
			else
			{
				await transaction.RollbackAsync();
				return false;
			}
		}
		catch (Exception ex)
		{
			await transaction.RollbackAsync();
			_logger.LogError(ex, "Transaction failed");
			return false;
		}
	}

	public async Task<bool> UpdateAttendanceInfo(long playerUid, MySqlTransaction transaction)
	{
		var updateCountResult = await _queryFactory.Query("attendance")
		.Where("player_uid", playerUid)
		.IncrementAsync("attendance_cnt", 1, transaction);

		var updateDateResult = await _queryFactory.Query("attendance")
			.Where("player_uid", playerUid)
			.UpdateAsync(new
			{
				recent_attendance_dt = DateTime.Now
			}, transaction);

		return updateCountResult > 0 && updateDateResult > 0;
	}
}
```

```
class GameDb
{
	MySqlConnection _connection;

	public async Task<PlayerInfo> CreatePlayerInfoDataAndStartItems(string playerId)
	{
		using var transaction = await _connection.BeginTransactionAsync();
		try
		{
			var newPlayerInfo = new PlayerInfo
			{
				PlayerId = playerId,
				NickName = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 27),
				Exp = 0,
				Level = 1,
				Win = 0,
				Lose = 0,
				Draw = 0
			};

			var insertId = await _queryFactory.Query("player_info").InsertGetIdAsync<int>(new
			{
				player_id = newPlayerInfo.PlayerId,
				nickname = newPlayerInfo.NickName,
				exp = newPlayerInfo.Exp,
				level = newPlayerInfo.Level,
				win = newPlayerInfo.Win,
				lose = newPlayerInfo.Lose,
				draw = newPlayerInfo.Draw
			}, transaction);

			newPlayerInfo.PlayerUid = insertId;

			var addItemsResult = await AddFirstItemsForPlayer(newPlayerInfo.PlayerUid, transaction);
			if (addItemsResult != ErrorCode.None)
			{
				await transaction.RollbackAsync();
				return null;
			}

			var attendanceResult = await CreatePlayerAttendanceInfo(newPlayerInfo.PlayerUid, transaction);
			if (!attendanceResult)
			{
				await transaction.RollbackAsync();
				return null;
			}

			await transaction.CommitAsync();
			return newPlayerInfo;
		}
		catch (Exception ex)
		{
			await transaction.RollbackAsync();
			_logger.LogError(ex, "Error creating player info for playerId: {PlayerId}", playerId);
			return null;
		}
	}
}
```   
  
  
# 수동으로 트랜잭션 기능 구현하기
DB에서 제공하는 트랜잭션 기능을 사용하지 않고, 직접 코드로 롤백 기능을 구현한다   
```
public async Task<ErrorCode> Attend(long userId, AttendanceModel attendData, bool usingPass)
{
    var rollbackQueries = new List<SqlKata.Query>();

    // 누적 출석일 수 계산
    var newAttendCount = CalcAttendanceCount(attendData);

    // 최종 출석일 갱신
    var attendResult = await UpdateAttendanceData(userId, newAttendCount, attendData, rollbackQueries);
    if (!Successed(attendResult))
    {
        _logger.ZLogDebugWithPayload(EventIdGenerator.Create(attendResult),
                                     new { UserId = userId, NewAttendCount = newAttendCount }, "Failed");

        return ErrorCode.AttendanceService_UpdateAttendanceData;
    }

    // 보상 아이템 우편으로 지급
    var rewardResult = await SendRewardMail(userId, newAttendCount, usingPass, rollbackQueries);
    if (!Successed(rewardResult))
    {
		// 실패가 발생하면 롤백한다
        await Rollback(rewardResult, rollbackQueries);

        _logger.ZLogDebugWithPayload(EventIdGenerator.Create(rewardResult),
                                     new { UserID = userId, NewAttendCount = newAttendCount }, "Failed");

        return ErrorCode.AttendanceService_SendAttendanceReward;
    }

    return ErrorCode.None;
}

async Task<ErrorCode> UpdateAttendanceData(Int64 userId, Int32 newAttendCount, AttendanceModel attendData, List<SqlKata.Query> queries)
{
    try
    {
        // 누적 출석일수와 최종 출석일 갱신
        var affectedRow = await _gameDb.UpdateAttendanceData(userId, newAttendCount);
        if (!ValidateAffectedRow(affectedRow, 1))
        {
            return ErrorCode.AttendanceService_UpdateAttendanceData_AffectedRowOutOfRange;
        }

        // 성공 시 롤백 쿼리 추가
        var query = _gameDb.GetQuery("user_attendance").Where("UserId", userId)
                           .AsUpdate(new { LastAttendance = attendData.LastAttendance,
                                           AttendanceCount = attendData.AttendanceCount });
        queries.Add(query);

        return ErrorCode.None;
    }
    catch (Exception ex)
    {
        var errorCode = ErrorCode.AttendanceService_UpdateAttendanceData_Fail;

        _logger.ZLogErrorWithPayload(EventIdGenerator.Create(errorCode), ex,
                                     new { UserId = userId,
                                           NewAttendanceCount = newAttendCount },
                                     "Failed");

        return errorCode;
    }
}
```
