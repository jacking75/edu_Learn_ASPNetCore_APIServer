using System;
using System.Data;
using System.Threading.Tasks;
using ApiServer.Model;
using ApiServer.Model.Data;
using ApiServer.Options;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using ServerCommon;
using ZLogger;

namespace ApiServer.Services
{
    public class GameDb : IGameDb
    {
        private readonly IOptions<DbConfig> _dbConfig;
        private IDbConnection _dbConn;
        private IDbTransaction _dBTransaction;
        private readonly ILogger<GameDb> _logger;
        private bool _isDisposed = false;
        private readonly IDataStorage _dataStorage;

        public GameDb(ILogger<GameDb> logger, IOptions<DbConfig> dbConfig, IDataStorage dataStorage)
        {
            _dbConfig = dbConfig;
            _logger = logger;
            _dataStorage = dataStorage;
            Open();
        }

        protected virtual void Dispose(bool _disposing)
        {
            if (!_isDisposed)
            {
                if (_disposing)
                {
                    Close();
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public void Open()
        {
            _dbConn = new MySqlConnection(_dbConfig.Value.GameConnStr);

            _dbConn.Open();
        }
        
        public void Close()
        {
            _dbConn.Close();
        }
        
        // 트랜잭션 시작.
        public void StartTransaction()
        {
            if (_dbConn == null)
            {
                throw new Exception("DB is not opened");
            }
            if (_dBTransaction != null)
            {
                throw new Exception("DB transaction is not finished");
            }

            _dBTransaction = _dbConn.BeginTransaction(IsolationLevel.RepeatableRead);

            if (_dBTransaction == null)
            {
                throw new Exception("DB transaction error");
            }
        }

        // 트랜잭션 롤백.
        public void Rollback()
        {
            if (_dbConn == null)
            {
                throw new Exception("DB is not opened");
            }
            if (_dBTransaction == null)
            {
                throw new Exception("DB transaction is not started");
            }
            
            _dBTransaction.Rollback();
            _dBTransaction = null;
        }

        // 트랜잭션 커밋.
        public void Commit()
        {
            if (_dbConn == null)
            {
                throw new Exception("DB is not opened");
            }
            if (_dBTransaction == null)
            {
                throw new Exception("DB transaction is not started");
            }

            _dBTransaction.Commit();
            _dBTransaction = null;
        }
        
        // 게임 정보 가져오기
        public async Task<Tuple<ErrorCode, UserGameInfo>> GetUserGameInfoAsync(string id)
        {
            try
            {
                var selectQuery = $"select * from gamedata where ID = @userId";
                var gameData = await _dbConn.QuerySingleOrDefaultAsync<TableUserGameInfo>(selectQuery, new
                {
                    userId = id
                });
                
                if(gameData is null)
                {
                    return new Tuple<ErrorCode, UserGameInfo>(ErrorCode.GetUserGameInfoFailNoData , null);
                }

                return new Tuple<ErrorCode, UserGameInfo>(ErrorCode.None, new UserGameInfo(gameData.UserLevel, gameData.UserExp, gameData.StarPoint, gameData.UpgradeCandy));
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(GetUserGameInfoAsync)} Exception : {e}");
                return new Tuple<ErrorCode, UserGameInfo>(ErrorCode.GetUserGameInfoFailException, null);
            }
        }
        
        // 게임 정보 설정하기
        public async Task<Tuple<ErrorCode, Int64>> InitUserGameInfoAsync(string id, UserGameInfo gameInfo)
        {
            try
            {
                // StartTransaction과 Commit을 넣는 이유
                // 멀티스레드 환경에서 insert를 한뒤 select last_insert_id를 진행할때, 다른 스레드에서 insert를 진행한다면 엉뚱한 인덱스를 가져올 수 있습니다. 
                StartTransaction();
                var insertQuery = "insert gamedata(ID, StarPoint, UserLevel, UserExp, UpgradeCandy) " +
                                  $"Values(@userId, {gameInfo.StarPoint}, {gameInfo.UserLevel}, {gameInfo.UserExp}, {gameInfo.UpgradeCandy}); SELECT LAST_INSERT_ID();";
                var lastInsertId = await _dbConn.QueryFirstAsync<Int32>(insertQuery, new
                {
                    userId = id
                });
                Commit();
                
                return new Tuple<ErrorCode, Int64>(ErrorCode.None, lastInsertId);
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(InitUserGameInfoAsync)} Exception : {e}");
                return new Tuple<ErrorCode, Int64>(ErrorCode.UserGameInfoFailInitException, 0);
            }
        }

        public async Task<ErrorCode> RollbackInitUserGameInfoAsync(Int64 gamedataId)
        {
            try
            {
                var deleteQuery = $"delete from gamedata where UID = {gamedataId}";
                var count = await _dbConn.ExecuteAsync(deleteQuery);
                
                if (count == 0)
                {
                    _logger.ZLogError($"{nameof(RollbackInitUserGameInfoAsync)} Error : {ErrorCode.RollbackInitUserGameIfnoFailDeleteQuery}");
                    return ErrorCode.RollbackInitUserGameIfnoFailDeleteQuery;
                }
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(RollbackInitUserGameInfoAsync)} Exception : {e}");
                return ErrorCode.RollbackInitUserGameInfoFailException;
            }
        }

        public async Task<ErrorCode> UpdateUserStarCountAsync(string ID, Int32 starCount)
        {
            try
            {
                var updateQuery =
                    $"update gamedata set StarPoint = StarPoint + {starCount} where ID = @userId";
                var updateCount = await _dbConn.ExecuteAsync(updateQuery, new
                {
                    userId = ID
                });

                if (updateCount == 0)
                {
                    _logger.ZLogError($"{nameof(UpdateUserStarCountAsync)} Error : {ErrorCode.UserGameInfoFailStarCountUpdateFail}");
                    return ErrorCode.UserGameInfoFailStarCountUpdateFail;
                }
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(UpdateUserStarCountAsync)} Exception : {e}");
                return ErrorCode.UserGameInfoFailStarCountException;
            }
            return ErrorCode.None;
        }

        public async Task<Tuple<ErrorCode, FieldMonsterResponse>> GetMonsterInfoAsync(Int64 monsterUID)
        {
            try
            {
                var selectQuery = $"select MonsterName, Type, Level, HP, Att, Def, StarCount from monsterinfo where MID = {monsterUID}";
                var monsterInfo = await _dbConn.QuerySingleOrDefaultAsync<TableMonsterInfo>(selectQuery);
                
                if (monsterInfo is null)
                {
                    return new Tuple<ErrorCode, FieldMonsterResponse>(ErrorCode.MonsterInfoFailNoMonster, null);
                }

                return new Tuple<ErrorCode, FieldMonsterResponse>(ErrorCode.None, new FieldMonsterResponse()
                {
                    MonsterID = monsterUID,
                    Name = monsterInfo.MonsterName,
                    Type = monsterInfo.Type,
                    Att = monsterInfo.Att,
                    Def = monsterInfo.Def,
                    HP = monsterInfo.HP,
                    Level = monsterInfo.Level,
                    StarCount = monsterInfo.StarCount,
                });
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(GetMonsterInfoAsync)} Exception : {e}");
                return new Tuple<ErrorCode, FieldMonsterResponse>(ErrorCode.MonsterInfoFailException, null);
            }
        }

        public async Task<Tuple<ErrorCode, Int32>> SetCatchAsync(string id, Int64 monsterID, DateTime catchTime, Int32 combatPoint)
        {
            try
            {
                // StartTransaction과 Commit을 넣는 이유
                // 멀티스레드 환경에서 insert를 한뒤 select last_insert_id를 진행할때, 다른 스레드에서 insert를 진행한다면 엉뚱한 인덱스를 가져올 수 있습니다. 
                StartTransaction();
                var insertQuery = $"insert catch(UserID, MonsterID, CatchTime, CombatPoint) Values(@userId, {monsterID}, @catchTime, {combatPoint}); SELECT LAST_INSERT_ID();";
                var lastInsertId = await _dbConn.QueryFirstAsync<Int32>(insertQuery, new
                {
                    userId = id,
                    catchTime = catchTime.ToString("yyyy-MM-dd")
                });
                Commit();
                
                return new Tuple<ErrorCode, Int32>(ErrorCode.None, lastInsertId);
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(SetCatchAsync)} Exception : {e}");
                return new Tuple<ErrorCode, Int32>(ErrorCode.CatchFailException, 0);
            }
        }

        public async Task<ErrorCode> RollbackSetCatchAsync(Int64 catchID)
        {
            try
            {
                var deleteQuery = $"delete from catch where CatchID = {catchID}";
                var count = await _dbConn.ExecuteAsync(deleteQuery);
                if (count == 0)
                {
                    _logger.ZLogError($"{nameof(RollbackSetCatchAsync)} Error : {ErrorCode.CatchFailDeleteFail}");
                    return ErrorCode.CatchFailDeleteFail;
                }
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(RollbackSetCatchAsync)} Exception : {e}");
                return ErrorCode.CatchFailException;
            }

            return ErrorCode.None;
        }
        public async Task<ErrorCode> InitDailyCheckAsync(string ID)
        {
            try
            {
                var insertQuery = $"INSERT INTO dailycheck(`ID`, `RewardCount`, `RewardDate`) VALUES (@userId, 0, @dateTime)";
                // 초기 값을 생성해준다.
                var count = await _dbConn.ExecuteAsync(insertQuery, new
                {
                    userId = ID,
                    dateTime = "1000-01-01"
                });

                if (count == 0)
                {
                    _logger.ZLogError($"{nameof(InitDailyCheckAsync)} Error : {ErrorCode.DailyCheckFailInsertQuery}");
                    return ErrorCode.DailyCheckFailInsertQuery;
                }
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(InitDailyCheckAsync)} Exception : {e}");
                return ErrorCode.InitDailyCheckFailException;
            }

            return ErrorCode.None;
        }
        
        public async Task<ErrorCode> RollbackInitDailyCheckAsync(string dailyID)
        {
            try
            {
                var deleteQuery = $"delete from dailycheck where ID = @userId";
                var count = await _dbConn.ExecuteAsync(deleteQuery, new
                {
                    userId = dailyID
                });
                
                if (count == 0)
                {
                    _logger.ZLogError($"{nameof(RollbackSendMailAsync)} Error : {ErrorCode.RollbackInitDailyCheckFailDeleteQuery}");
                    return ErrorCode.RollbackInitDailyCheckFailDeleteQuery;
                }
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(RollbackSendMailAsync)} Exception : {e}");
                return ErrorCode.RollbackInitDailyCheckFailException;
            }
        }
        
        public async Task<Tuple<ErrorCode, DateTime>> TryDailyCheckAsync(string ID)
        {
            try
            {
                var selectQuery = $"select RewardCount, RewardDate from dailycheck where ID = @userId";
                // 먼저 select해서 RewardCount와 RewardDate를 가져온다.
                var dailyCheck = await _dbConn.QuerySingleOrDefaultAsync<TableDailyCheck>(selectQuery, new
                {
                    userId = ID
                });

                // 회원 가입할때 생성해줄 것이기 때문에.. 값이 항상 있어야함.
                if (dailyCheck is null)
                {
                    _logger.ZLogError($"{nameof(TryDailyCheckAsync)} Error : {ErrorCode.DailyCheckFailNoData}");
                    return new Tuple<ErrorCode, DateTime>(ErrorCode.DailyCheckFailNoData, new DateTime());
                }

                // 오늘 이미 받았기 때문에 보상을 받을 수 없다.
                if (dailyCheck.RewardDate == DateTime.Today)
                {
                    return new Tuple<ErrorCode, DateTime>(ErrorCode.DailyCheckFailAlreadyChecked, new DateTime());
                }
                
                // data 갱신
                var updateQuery = 
                    $"UPDATE dailycheck Set RewardCount = RewardCount + 1, RewardDate = CURDATE() WHERE ID = @userId";
                var updateCount = await _dbConn.ExecuteAsync(updateQuery, new
                {
                    userId = ID
                });

                if (updateCount == 0)
                {
                    _logger.ZLogError($"{nameof(TryDailyCheckAsync)} Error : {ErrorCode.DailyCheckFailUpdateQuery}");
                    return new Tuple<ErrorCode, DateTime>(ErrorCode.DailyCheckFailUpdateQuery, new DateTime());
                }
                
                return new Tuple<ErrorCode, DateTime>(ErrorCode.None, dailyCheck.RewardDate);
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(TryDailyCheckAsync)} Exception : {e}");
                return new Tuple<ErrorCode, DateTime>(ErrorCode.TryDailyCheckFailException, new DateTime());
            }
        }

        public async Task<ErrorCode> RollbackDailyCheckAsync(string id, DateTime prevDate)
        {
            try
            {
                var updateQuery = $"UPDATE dailycheck Set RewardCount = RewardCount - 1, RewardDate = @date WHERE ID = @userId";
                var updateCount = await _dbConn.ExecuteAsync(updateQuery, new
                {
                    userId = id,
                    date = prevDate.ToString("yy-MM-dd")
                });
                
                if (updateCount == 0)
                {
                    _logger.ZLogError($"{nameof(RollbackDailyCheckAsync)} Error : {ErrorCode.RollbackDailyCheckFailUpdateQuery}");
                    return ErrorCode.RollbackDailyCheckFailUpdateQuery;
                }
                
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(RollbackDailyCheckAsync)} Exception : {e}");
                return ErrorCode.RollbackDailyCheckFailException;
            }
        }
        
        public async Task<Tuple<ErrorCode, Int32, List<Tuple<Int64,Int32>>?>> CheckMailAsync(string ID, Int32 pageIndex, Int32 pageSize)
        {
            try
            {
                var selectQuery = $"select postID, StarCount from Mail where ID = @userId LIMIT {pageIndex * pageSize}, {pageSize}";
                var Mail = await _dbConn.QueryAsync<TableMail>(selectQuery, new
                {
                    userId = ID
                });

                // 우편함에 값이 없는 상태임.
                if (Mail is null)
                {
                    return new Tuple<ErrorCode, Int32, List<Tuple<Int64, Int32>>?>(ErrorCode.CheckMailFailNoMail, 0, null);
                }

                // 전체 개수 찾아오기
                var countQuery = $"select count(*) from Mail where ID = @userId";
                var count = await _dbConn.QueryFirstAsync<Int32>(countQuery, new
                {
                    userId = ID
                });
                
                // 반환할 Tuple
                var ret = new Tuple<ErrorCode, Int32, List<Tuple<Int64, Int32>>?>(ErrorCode.None, count, new List<Tuple<Int64, Int32>>());
                
                // 쿼리를 돌면서 선물함의 ID와 선물 내용을 보내준다.
                foreach (var eachMail in Mail)
                {
                    ret.Item3.Add(new Tuple<Int64, Int32>(eachMail.postID, eachMail.StarCount));
                }

                return ret;
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(CheckMailAsync)} Exception : {e}");
                return new Tuple<ErrorCode, Int32, List<Tuple<Int64, Int32>>?>(ErrorCode.CheckMailFailException, 0, null);
            }
        }

        public async Task<Tuple<ErrorCode, Int64>> SendMailAsync(string ID, Int32 starCount)
        {
            try
            {
                // StartTransaction과 Commit을 넣는 이유
                // 멀티스레드 환경에서 insert를 한뒤 select last_insert_id를 진행할때, 다른 스레드에서 insert를 진행한다면 엉뚱한 인덱스를 가져올 수 있습니다. 
                StartTransaction();
                var insertQuery = $"insert into Mail(ID, StarCount, Date) " +
                    $"values (@userID, {starCount}, CURDATE()); SELECT LAST_INSERT_ID();";
                var lastInsertId = await _dbConn.QueryFirstAsync<Int32>(insertQuery, new
                {
                    userId = ID
                });
                Commit();
                
                return new Tuple<ErrorCode, Int64>(ErrorCode.None, lastInsertId);
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(SendMailAsync)} Exception : {e}");
                return new Tuple<ErrorCode, Int64>(ErrorCode.SendMailFailException, 0);
            }
        }

        public async Task<ErrorCode> RollbackSendMailAsync(Int64 MailID)
        {
            try
            {
                var deleteQuery = $"delete from Mail where postID = {MailID}";
                var count = await _dbConn.ExecuteAsync(deleteQuery);
                
                if (count == 0)
                {
                    _logger.ZLogError($"{nameof(RollbackSendMailAsync)} Error : {ErrorCode.RollbackSendMailFailDeleteQuery}");
                    return ErrorCode.RollbackSendMailFailDeleteQuery;
                }
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(RollbackSendMailAsync)} Exception : {e}");
                return ErrorCode.RollbackSendMailFailException;
            }
        }
        
        public async Task<Tuple<ErrorCode, Int32, DateTime>> RecvMailAsync(string ID, Int64 MailID)
        {
            try
            {
                var selectQuery = $"select ID, StarCount, Date from Mail where postID = {MailID} and ID = @userId";
                var selInfo = await _dbConn.QuerySingleAsync<TableMail>(selectQuery, new
                {
                    userId = ID
                });

                if (selInfo is null)
                {
                    // 선물이 없다면 문제가 있는 상황
                    return new Tuple<ErrorCode, Int32, DateTime>(ErrorCode.RecvMailFailNoMail, 0, new DateTime());
                }
                
                var delQuery = $"delete from Mail where postID = {MailID} and ID = @userId";
                var delCount = await _dbConn.ExecuteAsync(delQuery, new
                {
                    userId = ID
                });

                return new Tuple<ErrorCode, Int32, DateTime>(ErrorCode.None, selInfo.StarCount, selInfo.Date);
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(RecvMailAsync)} Exception : {e}");
                return new Tuple<ErrorCode, Int32, DateTime>(ErrorCode.RecvMailFailException, 0, new DateTime());
            }
        }

        public async Task<ErrorCode> RollbackRecvMailAsync(string id, Int32 startCount, DateTime date)
        {
            try
            {
                var insertQuery = $"insert into Mail(ID, StarCount, Date) values (@userId, {startCount}, @dateStr)";
                var count = await _dbConn.ExecuteAsync(insertQuery, new
                {
                    userId = id,
                    dateTime = date.ToString("yy-MM-dd")
                });
                
                if (count == 0)
                {
                    _logger.ZLogError($"{nameof(RollbackRecvMailAsync)} Error : {ErrorCode.RollbackRecvMailFailInsertQuery}");
                    return ErrorCode.RollbackRecvMailFailInsertQuery;
                }
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(RecvMailAsync)} Exception : {e}");
                return ErrorCode.RollbackRecvMailFailException;
            }
        }

        public async Task<Tuple<ErrorCode, List<Tuple<Int64, Int64, DateTime, Int32>>>> GetCatchListAsync(string id)
        {
            try
            {
                var selectQuery = $"select * from catch where UserID = @userId";
                var multiQuery = await _dbConn.QueryAsync<TableCatch>(selectQuery, new
                {
                    userId = id
                });

                if (multiQuery is null)
                {
                    return new Tuple<ErrorCode, List<Tuple<Int64, Int64, DateTime, Int32>>>(ErrorCode.GetCatchListFailNoCatchInfo, null);
                }
                
                var retList = new List<Tuple<Int64, Int64, DateTime, Int32>>();
                foreach (var tableCatch in multiQuery)
                {
                    retList.Add(new Tuple<Int64, Int64, DateTime, Int32>(tableCatch.CatchID, tableCatch.MonsterID, tableCatch.CatchTime, tableCatch.CombatPoint));
                }

                return new Tuple<ErrorCode, List<Tuple<Int64, Int64, DateTime, Int32>>>(ErrorCode.None, retList);
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(RecvMailAsync)} Exception : {e}");
                return new Tuple<ErrorCode, List<Tuple<Int64, Int64, DateTime, Int32>>>(ErrorCode.GetCatchListFailException, null);
            }
        }
        
        public async Task<Tuple<ErrorCode, Int64, Int64, DateTime, Int32>> DelCatchAsync(Int64 catchID)
        {
            try
            {
                var selectQuery = $"select * from catch where CatchID = {catchID}";
                var selInfo = await _dbConn.QuerySingleAsync<TableCatch>(selectQuery);

                if (selInfo is null)
                {
                    // 잡은 정보가 없는 상황
                    return new Tuple<ErrorCode, Int64, Int64, DateTime, Int32>(ErrorCode.DelCatchFailNoCatch, 0, 0, new DateTime(), 0);
                }
                
                var delQuery = $"delete from catch where CatchID = {catchID}";
                var delCount = await _dbConn.ExecuteAsync(delQuery);

                return new Tuple<ErrorCode, Int64, Int64, DateTime, Int32>(ErrorCode.None, selInfo.CatchID, selInfo.MonsterID, selInfo.CatchTime, selInfo.CombatPoint);
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(DelCatchAsync)} Exception : {e}");
                return new Tuple<ErrorCode, Int64, Int64, DateTime, Int32>(ErrorCode.DelCatchFailException, 0, 0, new DateTime(), 0);
            }
        }

        public async Task<ErrorCode> RollbackDelCatchAsync(string id, Int64 monsterID, DateTime catchDate) 
        {
            try
            {
                var insertQuery = $"insert catch(UserID, MonsterID, CatchTime) Values(@userId, {monsterID}, @catchTime);";
                var count = await _dbConn.QueryAsync(insertQuery, new
                {
                    userId = id,
                    catchTime = catchDate.ToString("yyyy-MM-dd")
                });
                
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(RollbackDelCatchAsync)} Exception : {e}");
                return ErrorCode.RollbackCatchFailException;
            }
        }

        public async Task<ErrorCode> UpdateUserExpAsync(string id, Int32 gainExp)
        {
            try
            {
                var selectQuery = $"select * from gamedata where ID = @userId";
                var selInfo = await _dbConn.QuerySingleAsync<TableUserGameInfo>(selectQuery, new
                {
                    userId = id
                });
                if (selInfo is null)
                {
                    _logger.ZLogError($"{nameof(UpdateUserExpAsync)} Error : {ErrorCode.UpdateUserExpFailNoUserExist}");
                    return ErrorCode.UpdateUserExpFailNoUserExist;
                }

                var nowLevel = selInfo.UserLevel;
                var nowExp = selInfo.UserExp + gainExp;

                while (true)
                {
                    var levelUpInfo = _dataStorage.GetLevelUpMaxExp(nowLevel);
                    if (levelUpInfo is null)
                    {
                        break;
                    }
                    
                    if (levelUpInfo.MaxExpForLevelUp > nowExp)
                    {
                        break;
                    }
                    
                    nowExp = levelUpInfo.MaxExpForLevelUp - nowExp;
                    nowLevel++;
                }

                var updateQuery = $"update gamedata set UserExp = {nowExp}, UserLevel = {nowLevel} WHERE ID = @userId";
                var updateCount = await _dbConn.ExecuteAsync(updateQuery, new
                {
                    userId = id
                });
                
                if (updateCount == 0)
                {
                    _logger.ZLogError($"{nameof(UpdateUserExpAsync)} Error : {ErrorCode.UpdateUserExpFailUpdateFail}");
                    return ErrorCode.UpdateUserExpFailUpdateFail;
                }
                
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(UpdateUserExpAsync)} Exception : {e}");
                return ErrorCode.UpdateUserExpFailException;
            }
        }

        public async Task<ErrorCode> UpdateUpgradeCostAsync(string id, Int32 updateCost)
        {
            try
            {
                var updateQuery = $"update gamedata set UpgradeCandy = UpgradeCandy + {updateCost} where ID = @userId";
                var updateCount = await _dbConn.ExecuteAsync(updateQuery, new
                {
                    userId = id
                });
                
                if (updateCount == 0)
                {
                    _logger.ZLogError($"{nameof(UpdateUserExpAsync)} Error : {ErrorCode.UpdateUpgradeCostExpFailUpdateFail}");
                    return ErrorCode.UpdateUpgradeCostExpFailUpdateFail;
                }
                
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(UpdateUserExpAsync)} Exception : {e}");
                return ErrorCode.UpdateUpgradeCostFailException;
            }
        }

        public async Task<ErrorCode> EvolveCatchMonsterAsync(Int64 catchId, Int64 evolveMonsterId)
        {
            try
            {
                var updateQuery = $"update catch set MonsterID = {evolveMonsterId} where CatchID = {catchId}";
                var updateCount = await _dbConn.ExecuteAsync(updateQuery);
                
                if (updateCount == 0)
                {
                    _logger.ZLogError($"{nameof(EvolveCatchMonsterAsync)} Error : {ErrorCode.EvolveCatchMonsterFailUpdateFail}");
                    return ErrorCode.EvolveCatchMonsterFailUpdateFail;
                }

                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError($"{nameof(EvolveCatchMonsterAsync)} Exception : {e}");
                return ErrorCode.EvolveCatchMonsterFailException;
            }
        }

        public async Task<ErrorCode> UpdateCatchCombatPointAsync(Int64 catchId, Int32 combatPoint)
        {
            try
            {
                var updateQuery = $"update catch set CombatPoint = CombatPoint + {combatPoint} where CatchID = {catchId}";
                var updateCount = await _dbConn.ExecuteAsync(updateQuery);

                if (updateCount == 0)
                {
                    _logger.ZLogError($"{nameof(EvolveCatchMonsterAsync)} Error : {ErrorCode.UpdateCatchCombatPointFailUpdateFail}");
                    return ErrorCode.UpdateCatchCombatPointFailUpdateFail;
                }

                return ErrorCode.None;
            }
            catch(Exception e)
            {
                _logger.ZLogError($"{nameof(UpdateCatchCombatPointAsync)} Exception : {e}");
                return ErrorCode.UpdateCatchCombatPointFailException;
            }
        }
    }
}