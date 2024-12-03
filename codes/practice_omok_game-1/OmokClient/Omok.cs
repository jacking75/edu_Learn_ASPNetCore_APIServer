using System.Text;

public enum OmokStone
{
    None,
    Black,
    White
}

//1.오목 OmokGameData 초기값 설정
// OmokGameData는 바이너리 배열 형태로 현재 게임 정보를 Redis에 저장하기 위한 것
// 매칭 성사 시에 MakeRawData로 데이터를 생성한다
// 데이터 구조 : 오목판 정보 + 흑돌 유저이름 + 백돌 유저이름 + 현재 턴(어떤 돌의 차례인지 1인지 2인지) + 턴 시간(최근 돌 두기 요청이 온 시간) + 이긴 사람 정보(없으면 0, 흑돌 이기면 1, 백돌 이기면 2)
// 돌에 대해서 None이면 0, 흑돌이면 1, 백돌이면 2로 고정한다.
// 초기 값 : 오목판은 모두 0, 현재 턴도 처음 생성 시에는 0, 턴 시간 초기값도 현재 데이터 생성한 시간 + 이긴 사람 정보 0


// 2. 게임 시작 StartGame() 함수
// StartGame() 함수가 호출된다면 
// -> 초기값에서 현재 턴을 1(흑돌)로 바꾸고 턴시간을 해당 함수 호출 시점으로 바꾼다.


// 3. 돌두기 SetStone() 함수
// SetStone() 즉 돌두기 함수가 호출된다면
// -> 초기값에서 오목판 정보를 입력받은 x, y값을 가지고 바꾸기. 현재턴을 다음 돌로 바꾸고 (isBlack이 True면 흑돌이라는 뜻이기 때문에 이제 백돌 차례라서 2로 바꾸기, False라면 1로 바꾸기), 턴시간도 현재 함수 호출 시간으로 바꾼다.

public class OmokGameData
{
    public const int BoardSize = 15;
    public const int BoardSizeSquare = BoardSize * BoardSize;

    const byte BlackStone = 1;
    const byte WhiteStone = 2;

    // 오목판 정보 BoardSize * BoardSize
    // 블랙 플레이어의 이름: 1(이름 바이트 수) + N(앞에서 구한 길이)
    // 화이트 플레이어의 이름: 1(이름 바이트 수) + N(앞에서 구한 길이)
    byte[] _rawData;

    string _blackPlayer;
    string _whitePlayer;


    OmokStone _turnPlayerStone; // 턴 받은 플레이어
    UInt64 _turnTimeMilli; // 턴 받은 시간 유닉스 시간(초)
    OmokStone _winner;


    public byte[] GetRawData()
    {
        return _rawData;
    }

    public byte[] MakeRawData(string blackPlayer, string whitePlayer)  // rawDataSize를 따로 입력받는 것이 아니라 이름 길이에 따라 동적으로 변경하도록 수정했습니다.
    {
        // 플레이어 이름의 길이를 동적으로 계산
        var blackPlayerBytes = Encoding.UTF8.GetBytes(blackPlayer);
        var whitePlayerBytes = Encoding.UTF8.GetBytes(whitePlayer);

        // 데이터 크기 계산
        int rawDataSize = BoardSizeSquare + // 오목판 정보
                          1 + blackPlayerBytes.Length + // 흑돌 플레이어 이름 (길이 1 바이트 + 실제 이름 데이터)
                          1 + whitePlayerBytes.Length + // 백돌 플레이어 이름 (길이 1 바이트 + 실제 이름 데이터)
                          1 + // 현재 턴 정보
                          8 + // 턴 시작 시각 (돌 둔 시간)
                          1;  // 이긴 사람 정보

        var rawData = new byte[rawDataSize];
        var index = 0;

        // 1. 오목판 정보 초기화 (모두 0)
        for (int i = 0; i < BoardSizeSquare; i++)
        {
            rawData[index++] = (byte)OmokStone.None;
        }

        // 2. 흑돌 플레이어 이름 저장
        rawData[index++] = (byte)blackPlayerBytes.Length;
        Array.Copy(blackPlayerBytes, 0, rawData, index, blackPlayerBytes.Length);
        index += blackPlayerBytes.Length;

        // 3. 백돌 플레이어 이름 저장
        rawData[index++] = (byte)whitePlayerBytes.Length;
        Array.Copy(whitePlayerBytes, 0, rawData, index, whitePlayerBytes.Length);
        index += whitePlayerBytes.Length;

        // 4. 현재 턴 정보 저장 (초기값 0)
        rawData[index++] = (byte)OmokStone.None;

        // 5. 턴 시간 저장 (초기값 현재 시간)
        var turnTime = BitConverter.GetBytes((UInt64)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        Array.Copy(turnTime, 0, rawData, index, turnTime.Length);
        index += turnTime.Length;

        // 6. 이긴 사람 정보 저장 (초기값 0)
        rawData[index++] = (byte)OmokStone.None;


        // TODO StartGame 로직 분리 및 구현 추가하기
        rawData = StartGame(rawData); // 임시 StartGame 처리까지 여기서 진행

        return rawData;
    }

    public OmokStone GetStoneAt(int x, int y) // 좌표의 돌 색
    {
        int index = y * BoardSize + x;
        return (OmokStone)_rawData[index];
    }

    // TODO 현재 턴인 PlayerId 가져오는 함수 추가하기 
    // : GetCurrentTurn() 활용해서 만들기
    // GetCurrentTurnPlayerId() 


    public OmokStone GetCurrentTurn() // 현재 턴 정보 반환
    {
        int index = BoardSizeSquare + 1 + GetBlackPlayerName().Length + 1 + GetWhitePlayerName().Length;
        return (OmokStone)_rawData[index];
    }

    public string GetBlackPlayerName() // 흑돌 플레이어 이름
    {
        int index = BoardSizeSquare;
        int length = _rawData[index];
        index += 1;
        return Encoding.UTF8.GetString(_rawData, index, length);
    }

    public string GetWhitePlayerName() // 백돌 플레이어 이름
    {
        int index = BoardSizeSquare;
        int blackPlayerNameLength = _rawData[index];
        index += 1 + blackPlayerNameLength;
        int whitePlayerNameLength = _rawData[index];
        index += 1;
        return Encoding.UTF8.GetString(_rawData, index, whitePlayerNameLength);
    }

    public string GetCurrentTurnPlayerName()
    {
        return GetCurrentTurn() == OmokStone.Black ? GetBlackPlayerName() : GetWhitePlayerName();
    }

    public UInt64 GetTurnTime() // 현재 턴 시작 시각 반환
    {
        int index = BoardSizeSquare + 1 + GetBlackPlayerName().Length + 1 + GetWhitePlayerName().Length + 1;
        return BitConverter.ToUInt64(_rawData, index);
    }

    public OmokStone GetWinnerStone() // 이긴 사람 정보 반환
    {
        int index = BoardSizeSquare + 1 + GetBlackPlayerName().Length + 1 + GetWhitePlayerName().Length + 1 + 8;
        return (OmokStone)_rawData[index];
    }
    public string GetWinnerPlayerId()
    {
        var winner = GetWinnerStone();
        if (winner == OmokStone.None)
            return null;
        return winner == OmokStone.Black ? GetBlackPlayerName() : GetWhitePlayerName();
    }


    public void Decoding(byte[] rawData)
    {
        _rawData = rawData;

        DecodingUserName();
        DecodingTurnAndTime();
        DecodingWinner();
    }

    public byte[] StartGame(byte[] rawData)
    {
        Decoding(rawData);
        _turnPlayerStone = OmokStone.Black;
        _turnTimeMilli = (UInt64)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        int turnIndex = BoardSizeSquare + 1 + _blackPlayer.Length + 1 + _whitePlayer.Length;
        rawData[turnIndex] = (byte)OmokStone.Black;

        var turnTimeBytes = BitConverter.GetBytes(_turnTimeMilli);
        Array.Copy(turnTimeBytes, 0, rawData, turnIndex + 1, turnTimeBytes.Length);

        return rawData;
    }

    public byte[] SetStone(byte[] rawData, string playerId, int x, int y) // TODO 가독성/코드 유지보수를 위해 isBlack을 받는 게 아니라. PlayerId 받기
    {
        Decoding(rawData);

        // 현재 턴인 플레이어 이름 확인
        string currentTurnPlayerName = GetCurrentTurnPlayerName();
        if (currentTurnPlayerName != playerId)
        {
            throw new InvalidOperationException("Not the player's turn.");
        }

        // 돌이 이미 놓여진 위치인지 확인
        int index = y * BoardSize + x;
        if (_rawData[index] != (byte)OmokStone.None)
        {
            throw new InvalidOperationException("The position is already occupied.");
        }

        // 돌 두기
        bool isBlack = playerId == GetBlackPlayerName();
        rawData[index] = (byte)(isBlack ? OmokStone.Black : OmokStone.White);

        // 턴 변경
        _turnPlayerStone = isBlack ? OmokStone.White : OmokStone.Black;
        int turnIndex = BoardSizeSquare + 1 + _blackPlayer.Length + 1 + _whitePlayer.Length;
        rawData[turnIndex] = (byte)_turnPlayerStone;

        // 턴 둔 시간 변경
        _turnTimeMilli = (UInt64)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var turnTimeBytes = BitConverter.GetBytes(_turnTimeMilli);
        Array.Copy(turnTimeBytes, 0, rawData, turnIndex + 1, turnTimeBytes.Length);

        // 오목 승리 조건 체크하는 함수
        OmokCheck();

        return rawData;
    }

    public void OmokCheck() // 결과 체크
    {
        for (int y = 0; y < BoardSize; y++)
        {
            for (int x = 0; x < BoardSize; x++)
            {
                var stone = GetStoneAt(x, y);
                if (stone == OmokStone.None)
                    continue;

                if (CheckDirection(x, y, 1, 0, stone) || // 가로 방향 체크
                    CheckDirection(x, y, 0, 1, stone) || // 세로 방향 체크
                    CheckDirection(x, y, 1, 1, stone) || // 대각선 방향 체크 (↘)
                    CheckDirection(x, y, 1, -1, stone))  // 대각선 방향 체크 (↗)
                {
                    _winner = stone;
                    int winnerIndex = BoardSizeSquare + 1 + _blackPlayer.Length + 1 + _whitePlayer.Length + 1 + 8;
                    _rawData[winnerIndex] = (byte)stone;
                    return;
                }
            }
        }
    }

    private bool CheckDirection(int startX, int startY, int dx, int dy, OmokStone stone)
    {
        int count = 1;
        for (int step = 1; step < 5; step++)
        {
            int x = startX + step * dx;
            int y = startY + step * dy;

            if (x < 0 || x >= BoardSize || y < 0 || y >= BoardSize)
                break;

            if (GetStoneAt(x, y) == stone)
            {
                count++;
            }
            else
            {
                break;
            }
        }

        return count >= 5;
    }

    public byte[] ChangeTurn(byte[] rawData, string playerId)
    {
        Decoding(rawData);

        // 현재 턴인 플레이어 이름 확인
        string currentTurnPlayerName = GetCurrentTurnPlayerName();
        if (currentTurnPlayerName != playerId)
        {
            throw new InvalidOperationException("Not the player's turn.");
        }

        bool isBlack = playerId == GetBlackPlayerName();

        // 턴 변경
        _turnPlayerStone = isBlack ? OmokStone.White : OmokStone.Black;
        int turnIndex = BoardSizeSquare + 1 + _blackPlayer.Length + 1 + _whitePlayer.Length;
        rawData[turnIndex] = (byte)_turnPlayerStone;

        // 턴 둔 시간 변경
        _turnTimeMilli = (UInt64)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var turnTimeBytes = BitConverter.GetBytes(_turnTimeMilli);
        Array.Copy(turnTimeBytes, 0, rawData, turnIndex + 1, turnTimeBytes.Length);

        return rawData;
    }


    void DecodingUserName()
    {
        var index = BoardSizeSquare;

        int blackPlayerNameLength = _rawData[index];
        index += 1;
        _blackPlayer = Encoding.UTF8.GetString(_rawData, index, blackPlayerNameLength);

        index += blackPlayerNameLength;
        int whitePlayerNameLength = _rawData[index];
        index += 1;
        _whitePlayer = Encoding.UTF8.GetString(_rawData, index, whitePlayerNameLength);
    }

    void DecodingTurnAndTime()
    {
        int turnIndex = BoardSizeSquare + 1 + _blackPlayer.Length + 1 + _whitePlayer.Length;
        _turnPlayerStone = (OmokStone)_rawData[turnIndex];

        var turnTimeBytes = new byte[8];
        Array.Copy(_rawData, turnIndex + 1, turnTimeBytes, 0, 8);
        _turnTimeMilli = BitConverter.ToUInt64(turnTimeBytes, 0);
    }

    void DecodingWinner()
    {
        int winnerIndex = BoardSizeSquare + 1 + _blackPlayer.Length + 1 + _whitePlayer.Length + 1 + 8;
        _winner = (OmokStone)_rawData[winnerIndex];
    }
}