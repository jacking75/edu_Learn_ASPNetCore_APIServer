using MySqlConnector;
using SqlKata.Execution;
using Microsoft.AspNetCore.Mvc; 

[ApiController]
[Route("[controller]")]
public class CreateAccount : Controller
{
    [HttpPost]
    public async Task<PkCreateAccountResponse> Post(PkCreateAccountRequest request)
    {
        var response = new PkCreateAccountResponse {Result = ErrorCode.None};
        
        var saltValue = Security.SaltString();
        var hashingPassword = Security.MakeHashingPassWord(saltValue, request.Password);
        
        using (var db = await DBManager.GetGameDBQuery())
        {
            try
            {
                var count = await db.Query("account").InsertAsync(new
                {
                    Email = request.Email,
                    SaltValue = saltValue,
                    HashedPassword = hashingPassword
                });

                if (count != 1)
                {
                    response.Result = ErrorCode.Create_Account_Fail_Duplicate;
                }

                Console.WriteLine($"[Request CreateAccount] Email:{request.Email}, saltValue:{saltValue}, hashingPassword:{hashingPassword}");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                response.Result = ErrorCode.Create_Account_Fail_Exception;
                return response;
            }
            finally
            {
                db.Dispose();
            }
        }

        return response;
    }

    
}


public class PkCreateAccountRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class PkCreateAccountResponse
{
    public ErrorCode Result { get; set; }
}
