public enum ErrorCode : int
{
    None = 0,

    AuthTokenFailNoBody = 6, 

    Create_Account_Fail_Duplicate = 11,
    Create_Account_Fail_Exception = 12,
    
    Login_Fail_NotUser = 16,
    Login_Fail_PW = 17,
}
