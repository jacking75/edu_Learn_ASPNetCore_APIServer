using System;
using System.ComponentModel.DataAnnotations;

namespace APIServer.ModelDB;


public class Account
{
    public Int64 account_id { get; set; }

    public Int64 user_id { get; set; }

    public string email { get; set; } = string.Empty;

    public string password { get; set; } = string.Empty;

    public string salt_value { get; set; } = string.Empty;

    public string join_date { get; set; } = string.Empty;
}


// Redis 관련
public class CertifiedUser
{
    public Int64 AccountId { get; set; }

    public Int64 UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string AuthToken { get; set; } = string.Empty;

    public Int32 AppVersion { get; set; }

    public Int32 MasterDataVersion { get; set; }

    [Required]
    [Range(1, 100, ErrorMessage = "Channel Number Not Valid")]
    public Int32 ChannelNumber { get; set; }

    public bool Validate(string email, string authToken, Int32 appVersion, Int32 masterDataVersion)
    {
        if (Email != email || AuthToken != authToken || AppVersion != appVersion || MasterDataVersion != masterDataVersion)
        {
            return false;
        }

        return true;
    }
}

