public enum OmokIndex : int
{
	BlackPlayerUid = OmokGame.BoardByteSize,              
	WhitePlayerUid = 233,                        
	TurnCount = 241,                             
	GameStartTime = 242,                         
	LastTurnChangeTime = 250,                    
	GameResult = 258,						    
	TotalByteSize = 259
}

public static class OmokGame
{
	private const int maxTurn = 255;
	private const int maxTurnTime = 30;

	public const int BoardSize = 15;
	public const int BoardByteSize = BoardSize * BoardSize;
	public const int OmokRewardCode = 1;

	public static byte[] MakeOmokGame()
	{
		byte[] gameData = new byte[(int)OmokIndex.TotalByteSize];
		for (int i = 0; i < BoardByteSize; i++)
		{
			gameData[i] = (byte)OmokStone.None;
		}

		Buffer.BlockCopy(BitConverter.GetBytes(0L), 0, gameData, (int)OmokIndex.BlackPlayerUid, sizeof(long)); // Black Player UID
		Buffer.BlockCopy(BitConverter.GetBytes(0L), 0, gameData, (int)OmokIndex.WhitePlayerUid, sizeof(long)); // White Player UID

		gameData[(int)OmokIndex.TurnCount] = 0;

		byte[] zeroTimeBytes = BitConverter.GetBytes(0L);
		Buffer.BlockCopy(zeroTimeBytes, 0, gameData, (int)OmokIndex.GameStartTime, zeroTimeBytes.Length); // Game Start Time
		Buffer.BlockCopy(zeroTimeBytes, 0, gameData, (int)OmokIndex.LastTurnChangeTime, zeroTimeBytes.Length); // Last Turn Change Time

		gameData[(int)OmokIndex.GameResult] = (byte)OmokResultCode.None;

		return gameData;
	}

    public static byte[] GetBoard(byte[] gameData)
    {
        byte[] boardData = new byte[BoardByteSize];
        Buffer.BlockCopy(gameData, 0, boardData, 0, BoardByteSize); // Copy only the board portion
        return boardData;
    }

	public static OmokStone GetPlayerStone(byte[] gameData, Int64 uid)
	{
		Int64 blackPlayerUid = GetBlackPlayerUid(gameData);
		Int64 whitePlayerUid = GetWhitePlayerUid(gameData);

		if (uid == blackPlayerUid)
		{
			return OmokStone.Black;
		}
		else if (uid == whitePlayerUid)
		{
			return OmokStone.White;
		}

		return OmokStone.None;
	}

    public static Int64 GetBlackPlayerUid(byte[] gameData)
	{
		return BitConverter.ToInt64(gameData, (int)OmokIndex.BlackPlayerUid);
	}

	public static Int64 GetWhitePlayerUid(byte[] gameData)
	{
		return BitConverter.ToInt64(gameData, (int)OmokIndex.WhitePlayerUid);
	}
    public static int GetTurnCount(byte[] gameData)
    {
        return gameData[(int)OmokIndex.TurnCount];
    }

    public static Int64 GetGameStartTime(byte[] gameData)
    {
        return BitConverter.ToInt64(gameData, (int)OmokIndex.GameStartTime);
    }

    public static Int64 GetLastTurnChangeTime(byte[] gameData)
    {
        return BitConverter.ToInt64(gameData, (int)OmokIndex.LastTurnChangeTime);
    }

    public static int GetPlayerIndex(OmokStone stone)
	{
		return stone == OmokStone.Black ? (int)OmokIndex.BlackPlayerUid : (int)OmokIndex.WhitePlayerUid;
	}

	public static int GetOpponentIndex(OmokStone stone)
	{
		return stone == OmokStone.Black ? (int)OmokIndex.WhitePlayerUid : (int)OmokIndex.BlackPlayerUid;
	}

	public static Int64 GetOpponentUid(byte[] gameData, Int64 uid)
	{
		Int64 blackPlayerUid = GetBlackPlayerUid(gameData);
		Int64 whitePlayerUid = GetWhitePlayerUid(gameData);

		if (uid == blackPlayerUid)
		{
			return whitePlayerUid;
		}
		else if (uid == whitePlayerUid)
		{
			return blackPlayerUid;
		}

		return 0;
	}

	public static OmokResultCode GetGameResultCode(byte[] gameData)
	{
		return (OmokResultCode)gameData[(int)OmokIndex.GameResult];
	}

	public static bool IsGameStarted(byte[] gameData)
	{
		return BitConverter.ToInt64(gameData, (int)OmokIndex.GameStartTime) != 0;
	}

	public static bool IsGameEnded(byte[] gameData)
	{
		return GetGameResultCode(gameData) != OmokResultCode.None;
	}

	public static bool IsMyTurn(byte[] gameData, Int64 uid)
	{
		OmokStone stone = GetPlayerStone(gameData, uid);
		if (stone == OmokStone.None)
		{
			return false;
		}

		return stone == OmokStone.Black
			? gameData[(int)OmokIndex.TurnCount] % 2 == 0
			: gameData[(int)OmokIndex.TurnCount] % 2 == 1;
	}

	public static OmokStone GetStone(byte[] gameData, int posX, int posY)
	{
		int index = GetBoardIndex(posX, posY);
		return (OmokStone)gameData[index];
	}

	#region Game Logic
	public static bool PlaceStone(byte[] gameData, OmokStone stone, int posX, int posY)
	{
		if (IsGameEnded(gameData) == true || IsGameStarted(gameData) == false)
		{
			return false;
		}

		if (false == CheckTurn(gameData, stone))
		{
			return false;
		}

		if (false == CheckPosition(gameData, posX, posY))
		{
			return false;  
		}

		if (GetStone(gameData, posX, posY) != OmokStone.None)
		{
			return false;  
		}

		int index = GetBoardIndex(posX, posY);
		gameData[index] = (byte)stone;

		UpdateTurn(gameData);

		return true;
	}

	public static bool CheckWinAndEndGame(byte[] gameData, OmokStone stone, int posX, int posY)
	{
		bool isWin = CheckDirection(gameData, stone, posX, posY, 1, 0) ||  
			   CheckDirection(gameData, stone, posX, posY, 0, 1) || 
			   CheckDirection(gameData, stone, posX, posY, 1, 1) ||  
			   CheckDirection(gameData, stone, posX, posY, 1, -1);   

		if (false == isWin)
			return false;

		EndGame(gameData, stone);

		return true;
	}

	public static bool CheckMaxTurn(byte[] gameData)
	{
		if (gameData[(int)OmokIndex.TurnCount] >= maxTurn)
		{
			EndGame(gameData, OmokStone.None);
			return true;
		}

		return false;
	}
	#endregion

	#region Private Methods

	private static int GetBoardIndex(int posX, int posY)
	{
		return posY * BoardSize + posX;  // Row-major order
	}

	private static bool CheckDirection(byte[] gameData, OmokStone stone, int posX, int posY, int dx, int dy)
	{
		int consecutive = 0;

		for (int i = -4; i <= 4; i++)
		{
			int newX = posX + i * dx;
			int newY = posY + i * dy;

			if (newX >= 0 && newX < BoardSize && newY >= 0 && newY < BoardSize && GetStone(gameData, newX, newY) == stone)
			{
				consecutive++;
				if (consecutive == 5)
				{
					return true;  
				}
			}
			else
			{
				consecutive = 0;
			}
		}
		return false;
	}

	private static void EndGame(byte[] gameData, OmokStone winner)
	{
		if (winner == OmokStone.None)
		{
			gameData[(int)OmokIndex.GameResult] = (byte)OmokResultCode.Draw;
			return;
		}

		gameData[(int)OmokIndex.GameResult] = (byte)winner;
	}

	private static bool CheckTurn(byte[] gameData, OmokStone stone)
	{
		Int64 lastUpdateTime = BitConverter.ToInt64(gameData, (int)OmokIndex.LastTurnChangeTime);
		Int64 currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

		var updatedTime = currentTime - lastUpdateTime;

		if (updatedTime != 0 && updatedTime > maxTurnTime)
		{
			UpdateTurn(gameData);
		}

		return stone == OmokStone.Black
			? gameData[(int)OmokIndex.TurnCount] % 2 == 0 
			: gameData[(int)OmokIndex.TurnCount] % 2 == 1; 
	}

	private static bool CheckPosition(byte[] gameData, int posX, int posY)
	{
		return posX >= 0 && posX < BoardSize && posY >= 0 && posY < BoardSize;
	}

	private static void UpdateTurn(byte[] gameData)
	{
		if (gameData[(int)OmokIndex.TurnCount] >= maxTurn)
		{
			return;
		}

		gameData[(int)OmokIndex.TurnCount]++;

		byte[] turnTimeBytes = BitConverter.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
		Buffer.BlockCopy(turnTimeBytes, 0, gameData, (int)OmokIndex.LastTurnChangeTime, turnTimeBytes.Length);
	}

	#endregion
}
