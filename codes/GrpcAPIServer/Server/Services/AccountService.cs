using Grpc.Core;
using Account;


namespace GrpcAPIServer.Services;

public class AccounterService : Accounter.AccounterBase
{
    private readonly ILogger _logger;

    public AccounterService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AccounterService>();
    }

    public override Task<CreateAccountResponse> Create(CreateAccountRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Sending CreateAccountResponse");
        return Task.FromResult(new CreateAccountResponse { Result = 1 });
    }
}

