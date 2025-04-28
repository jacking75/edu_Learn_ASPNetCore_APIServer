namespace TestApiServer.ServerCore;

public enum ErrorCode
{
	None = 0,

	// Login  100 ~ 199
	LoginFail = 100,
	LoginInvalidUsername = 101,
	LoginInvalidPassword = 102,
	LoginInvalidToken = 103,
	LoginInvalidRequest = 104,
	LoginException = 105,

}