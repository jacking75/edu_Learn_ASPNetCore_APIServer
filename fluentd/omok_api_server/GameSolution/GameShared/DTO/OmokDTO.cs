namespace GameShared.DTO;

public class OmokEnterRequest;

public class OmokEnterResponse : ErrorCodeDTO;

public class OmokPeekRequest
{
	public int TurnCount { get; set; }
}

public class OmokPeekResponse : ErrorCodeDTO
{
	public int TurnCount { get; set; }
	public byte[]? OmokData { get; set; }

}

public class OmokPutRequest
{
	public int PosX { get; set; }

	public int PosY { get; set; }
}

public class OmokPutResponse : ErrorCodeDTO;
