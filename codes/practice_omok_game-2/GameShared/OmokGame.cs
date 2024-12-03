public enum OmokStone 
{				
	Empty,		
	Black,		
	White		
}

public enum GameResultCode
{
	Draw = 0,
	BlackWin = 1,
	WhiteWin = 2

}

[Flags]
public enum GameFlag : byte
{
	GameStart = 1 << 0,			
	GameEnterBlack = 1 << 1,    
	GameEnterWhite = 1 << 2,    
	GameWinner = 1 << 3,				
	GameEnd = 1 << 4,				
	GameResultSaved = 1 << 5,
	GameRewardSent = 1 << 6,             
	Flag8 = 1 << 7              
}

public enum GameIndex : int
{
	GameFlag = OmokGame.BoardByteSize,          // BoardByteSize + 1
	BlackPlayer = 58,							// GameFlag + 1
	WhitePlayer = 66,							// BlackPlayer + sizeof(Int64) (8 bytes)
	GameStartTime = 74,							// WhitePlayer + sizeof(Int64) (8 bytes)
	LastTurnTime = 82,							// GameStartTime + sizeof(Int64) (8 bytes)
	TurnCount = 90,								// LastTurnTime + sizeof(Int64) (8 bytes)
	TotalByteSize = 91							// TurnCount + 1 (1 byte)
}

public static class OmokGame
{
	public static readonly Int64 TurnExpiry = 1000 * 10 * 3; // 30 seconds

	public const int BoardSize = 15;
	private const int BitsPerStone = 2;
	public const int BoardByteSize = 57;
	public const int OmokRewardCode = 1;

	public static byte[]? MakeOmokGame(Int64 blackUid, Int64 whiteUid)
	{
		byte[] gameData = new byte[(int)GameIndex.TotalByteSize];

		Buffer.BlockCopy(BitConverter.GetBytes(blackUid), 0, gameData, 
			(int)GameIndex.BlackPlayer, sizeof(Int64));

		Buffer.BlockCopy(BitConverter.GetBytes(whiteUid), 0, gameData, 
			(int)GameIndex.WhitePlayer, sizeof(Int64));

		return gameData;
	}

	#region Game Logic

	/// <summary>
	///	게임 입장
	/// </summary>
	public static bool TryEnterPlayer(byte[] gameData, Int64 uid)
	{
		if (IsPlayerEntered(gameData, uid))
		{
			return false;
		}

		EnterPlayer(gameData, uid);

		return true;
	}

	/// <summary>
	///	돌 배치 하기 
	/// </summary>
	public static ErrorCode TryPutStone(byte[] gameData, int posX, int posY, OmokStone stone)
	{
		if (true == IsTurnExpired(gameData))
		{
			return ErrorCode.GameSaveStoneFailExpiredTurn;
		}

		var errorCode = CanPutStone(gameData, posX, posY, stone);
		
		if (ErrorCode.None != errorCode)
		{
			return errorCode;
		}

		SetStone(gameData, posX, posY , stone);

		if (CheckWin(gameData, stone))
		{
			EndGame(gameData, stone);
		}
		else
		{
			UpdateTurn(gameData);
		}

		return ErrorCode.None;
	}

	/// <summary>
	///	승리 여부 확인
	/// </summary>
	public static bool CheckWin(byte[] gameData, OmokStone stone)
	{
		for (int posX = 0; posX < BoardSize; posX++)
		{
			for (int posY = 0; posY < BoardSize; posY++)
			{
				if (GetStone(gameData, posX, posY) == stone)
				{		
					if (CheckDirection(gameData, stone, posX, posY, 1, 0) ||  
						CheckDirection(gameData, stone, posX, posY, 0, 1) ||  
						CheckDirection(gameData, stone, posX, posY, 1, 1) ||  
						CheckDirection(gameData, stone, posX, posY, 1, -1))   
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	/// <summary>
	///	턴 만료 여부 확인 (1턴 이상 턴이 지났을 경우)
	/// </summary>
	public static bool CheckAndUpdateTurnExpiry(byte[] gameData, Int64 uid)
	{
		if (GetCurrentTurn(gameData) == GetPlayerStone(gameData, uid))
		{
			return false;
		}

		if (false == IsTurnExpired(gameData))
		{
			return false;
		}

		UpdateTurn(gameData);
		return true;
	}

	/// <summary>
	///	게임 만료 여부 확인 (2턴 이상 턴이 지났을 경우)
	/// </summary>
	public static bool CheckAndUpdateGameExpiry(byte[] gameData)
	{
		if (IsGameExpired(gameData))
		{
			EndGame(gameData, OmokStone.Empty);
			return true;
		}

		return false;
	}

	#endregion

	#region GET

	public static bool IsPosValid(int posX, int posY)
	{
		return posX >= 0 && posX < BoardSize && posY >= 0 && posY < BoardSize;
	}

	public static bool IsStoneValid(OmokStone stone)
	{
		return stone == OmokStone.Black || stone == OmokStone.White;
	}

	public static Int64 GetWhitePlayerUid(byte[] gameData)
	{
		return BitConverter.ToInt64(gameData, (int)GameIndex.WhitePlayer);
	}

	public static Int64 GetBlackPlayerUid(byte[] gameData)
	{
		return BitConverter.ToInt64(gameData, (int)GameIndex.BlackPlayer);
	}

	public static Int64 GetGameStartTime(byte[] gameData)
	{
		return BitConverter.ToInt64(gameData, (int)GameIndex.GameStartTime);
	}

	public static Int64 GetOpponentUid(byte[] gameData, Int64 uid)
	{
		if (GetWhitePlayerUid(gameData) == uid)
		{
			return GetBlackPlayerUid(gameData);
		}

		if (GetBlackPlayerUid(gameData) == uid)
		{
			return GetWhitePlayerUid(gameData);
		}

		return 0;
	}

	public static OmokStone GetStone(byte[] gameData, int posX, int posY)
	{
		int bitIndex = GetIndex(posX, posY);
		int byteIndex = bitIndex / 8;
		int bitOffset = bitIndex % 8;

		if (byteIndex >= gameData.Length)
		{
		}

		int occupiedMask = 1 << (byte)bitOffset;
		int colorMask = 1 << (byte)bitOffset + 1;

		if ((gameData[byteIndex] & (byte)occupiedMask ) == 0)
		{
			return OmokStone.Empty;
		}

		return (gameData[byteIndex] & (byte)colorMask) == 0 ? OmokStone.Black : OmokStone.White;
	}

	public static OmokStone GetCurrentTurn(byte[] gameData)
	{
		byte turnCount = gameData[(int)GameIndex.TurnCount];
		return (turnCount & 1) == 0 ? OmokStone.White : OmokStone.Black;
	}

	public static OmokStone GetPlayerStone(byte[] gameData, Int64 uid)
	{
		if (GetWhitePlayerUid(gameData) == uid)
		{
			return OmokStone.White;

		}
		 
		if (GetBlackPlayerUid(gameData) == uid)
		{
			return OmokStone.Black;

		}

		return OmokStone.Empty;
	}

	public static bool GetGameState(byte[] gameData, GameFlag flag)
	{
		return (gameData[(int)GameIndex.GameFlag] & (byte)flag) != 0;
	}

	public static bool IsPlayerEntered(byte[] gameData, Int64 uid)
	{
		if (GetWhitePlayerUid(gameData) == uid)
		{
			return GetGameState(gameData, GameFlag.GameEnterWhite);
		}

		if (GetBlackPlayerUid(gameData) == uid)
		{
			return GetGameState(gameData, GameFlag.GameEnterBlack);
		}

		return false;
	}

	public static bool IsGameReady(byte[] gameData)
	{
		return IsFlagSet(gameData, GameFlag.GameEnterBlack) && IsFlagSet(gameData, GameFlag.GameEnterWhite);
	}

	public static bool IsGameStarted(byte[] gameData)
	{
		return IsFlagSet(gameData, GameFlag.GameStart) && !IsFlagSet(gameData, GameFlag.GameEnd);
	}

	public static bool IsGameEnded(byte[] gameData)
	{
		return IsFlagSet(gameData, GameFlag.GameEnd);
	}

	public static GameResultCode GetGameResultCode(byte[] gameData)
	{
		if (true == IsFlagSet(gameData, GameFlag.GameEnd))
		{
			if (IsFlagSet(gameData, GameFlag.GameWinner))
			{
				return GameResultCode.WhiteWin;
			}
			else
			{
				return GameResultCode.BlackWin;
			}
		}

		return GameResultCode.Draw;
	}

	public static bool IsGameResultSaved(byte[] gameData)
	{
		return IsFlagSet(gameData, GameFlag.GameResultSaved);
	}

	public static bool IsGameRewardSent(byte[] gameData)
	{
		return IsFlagSet(gameData, GameFlag.GameRewardSent);
	}

	public static OmokStone GetGameWinner(byte[] gameData)
	{
		return IsFlagSet(gameData, GameFlag.GameWinner) ? OmokStone.White : OmokStone.Black;
	}

	public static Int64 GetGameWinnerUid(byte[] gameData)
	{
		return GetGameWinner(gameData) == OmokStone.White ? GetWhitePlayerUid(gameData) : GetBlackPlayerUid(gameData);
	}

	public static bool IsGameExpired(byte[] gameData)
	{
		Int64 lastTurnTime = BitConverter.ToInt64(gameData, (int)GameIndex.LastTurnTime);
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastTurnTime > TurnExpiry * 2;

	}

	public static bool CheckExpiry(byte[] gameData, Int64 uid)
	{
		return true == OmokGame.CheckAndUpdateGameExpiry(gameData) || true == OmokGame.CheckAndUpdateTurnExpiry(gameData, uid);
	}

	public static bool IsTurnExpired(byte[] gameData)
	{
		Int64 lastTurnTime = BitConverter.ToInt64(gameData, (int)GameIndex.LastTurnTime);
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastTurnTime > TurnExpiry;
	}

	public static ErrorCode CanPutStone(byte[] gameData, int posX, int posY, OmokStone stone)
	{
		if (true == IsGameEnded(gameData))
		{
			return ErrorCode.GameSaveStoneFailInvalidGameStatus;
		}

		if (false == IsGameStarted(gameData))
		{
			return ErrorCode.GameSaveStoneFailInvalidGameStatus;
		}

		if (GetCurrentTurn(gameData) != stone)
		{
			return ErrorCode.GameSaveStoneFailInvalidTurn;
		}

		if (!IsPosValid(posX, posY) || !IsStoneValid(stone) || !IsCellEmpty(gameData, posX, posY))
		{
			return ErrorCode.GameSaveStoneFailInvalidParameters;
		}

		return ErrorCode.None;
	}

	#endregion

	#region SET

	public static void SetLastTurnChangeTime(byte[] gameData, Int64 turnTimeInMillis)
	{
		byte[] turnTimeBytes = BitConverter.GetBytes(turnTimeInMillis);
		Buffer.BlockCopy(turnTimeBytes, 0, gameData, (int)GameIndex.LastTurnTime, sizeof(Int64)); 
	}

	public static void SetGameStartTime(byte[] gameData, Int64 startTimeInMillis)
	{
		byte[] startTimeBytes = BitConverter.GetBytes(startTimeInMillis);
		Buffer.BlockCopy(startTimeBytes, 0, gameData, (int)GameIndex.GameStartTime, sizeof(Int64)); 
	}

	public static void StartGame(byte[] gameData)
	{
		SetFlag(gameData, GameFlag.GameStart);
		SetGameStartTime(gameData, (Int64)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
		UpdateTurn(gameData);
	}

	public static void EndGame(byte[] gameData, OmokStone winner)
	{
		UnsetFlag(gameData, GameFlag.GameStart);
		SetFlag(gameData, GameFlag.GameEnd);

		if (winner == OmokStone.White)
		{
			SetFlag(gameData, GameFlag.GameWinner);
		}
		else 
		{
			UnsetFlag(gameData, GameFlag.GameWinner);
		}
	}

	public static void SetGameResultSaved(byte[] gameData)
	{
		SetFlag(gameData, GameFlag.GameResultSaved);
	}

	public static void SetGameRewardSent(byte[] gameData)
	{
		SetFlag(gameData, GameFlag.GameRewardSent);
	}

	public static void EnterPlayer(byte[] gameData, Int64 uid)
	{
		if (GetWhitePlayerUid(gameData) == uid)
		{
		
			SetFlag(gameData, GameFlag.GameEnterWhite);
		}
		else if (GetBlackPlayerUid(gameData) == uid)
		{			
			SetFlag(gameData, GameFlag.GameEnterBlack);
		}
	}


	#endregion

	#region GameFlag 관리
	private static void SetFlag(byte[] gameData, GameFlag flag)
	{
		gameData[(int)GameIndex.GameFlag] |= (byte)flag;
	}

	private static void UnsetFlag(byte[] gameData, GameFlag flag)
	{
		gameData[(int)GameIndex.GameFlag] &= (byte)~flag;
	}

	private static bool IsFlagSet(byte[] gameData, GameFlag flag)
	{
		return (gameData[(int)GameIndex.GameFlag] & (byte)flag) != 0;
	}

	#endregion

	#region PRIVATE

	private static int GetIndex(int posX, int posY)
	{
		return (posY * BoardSize + posX) * BitsPerStone;
	}

	private static bool IsCellEmpty(byte[] gameData, int posX, int posY)
	{
		int bitIndex = GetIndex(posX, posY);
		int byteIndex = bitIndex / 8;
		int bitOffset = bitIndex % 8;

		if (byteIndex >= gameData.Length)
		{
			return false;
		}

		int occupiedMask = 1 << (byte)bitOffset;
		return (gameData[byteIndex] & (byte)occupiedMask) == 0;
	}
	
	private static bool CheckDirection(byte[] gameData, OmokStone stone, int posX, int posY, int dx, int dy)
	{
		int consecutive = 0;

		for (int i = 0; i < 5; i++)
		{
			int newX = posX + i * dx;
			int newY = posY + i * dy;

			if (IsPosValid(newX, newY) &&
				GetStone(gameData, newX, newY) == stone)
			{
				consecutive++;
			}
			else
			{
				break;
			}
		}

		return consecutive == 5;
	}

	private static void SetStone(byte[] gameData, int posX, int posY, OmokStone stone)
	{
		int bitIndex = GetIndex(posX, posY);
		int byteIndex = bitIndex / 8;
		int bitOffset = bitIndex % 8;

		int occupiedMask = 1 << (byte)bitOffset;
		int colorMask = 1 << (byte)bitOffset + 1;

		gameData[byteIndex] |= (byte)occupiedMask;

		if (stone == OmokStone.White)
		{
			gameData[byteIndex] |= (byte)colorMask;
		}
		else
		{
			gameData[byteIndex] &= (byte)~colorMask;
		}
	}

	private static void UpdateTurn(byte[] gameData)
	{
		SetLastTurnChangeTime(gameData, (Int64)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
		gameData[(int)GameIndex.TurnCount]++;
	}

	#endregion
}