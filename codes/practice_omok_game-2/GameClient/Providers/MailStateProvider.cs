using System.Net.Http.Json;
namespace GameClient.Providers;

public class MailStateProvider
{
	private readonly IHttpClientFactory _httpClientFactory;
	public int UnreadMailCount { get; private set; }

	public MailStateProvider(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task<(ErrorCode, List<MailInfo>)> GetMailsAsync()
	{
		try
		{
			List<MailInfo> mails = new List<MailInfo>();

			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsync("/mail/check", null);

			if (!response.IsSuccessStatusCode)
			{
				return (ErrorCode.MailGetBadRequest, mails);
			}

			var result = await response.Content.ReadFromJsonAsync<GetMailResponse>();

			if (result == null || result.MailData == null)
			{
				return (ErrorCode.MailGetNotFound, mails);
			}

			if (ErrorCode.None != result.Result)
			{
				return (result.Result, mails);
			}

			mails.AddRange(result.MailData);

			UnreadMailCount = result.MailData.Count((e) => e.StatusCode == MailStatusCode.Unread);

			return (ErrorCode.None, mails);
		}
		catch (Exception e)
		{
			return (ErrorCode.MailGetException, null);
		}
	}

	public async Task<(ErrorCode, MailDisplayData?)> ReadMailAsync(Int64 mailUid)
	{
		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsJsonAsync("/mail/read", new ReadMailRequest
			{
				MailUid = mailUid
			});

			if (!response.IsSuccessStatusCode)
			{
				return (ErrorCode.MailReadBadRequest, null);
			}

			var result = await response.Content.ReadFromJsonAsync<ReadMailResponse>();

			if (ErrorCode.None != result.Result)
			{
				return (result.Result, null);
			}

			var mailDisplayData = new MailDisplayData
			{
				MailInfo = result.MailInfo,
				Items = result.Items
			};

			return (ErrorCode.None, mailDisplayData);
		}
		catch (Exception e)
		{
			return (ErrorCode.MailReadException, null);
		}
	}

	public async Task<ErrorCode> ReceiveMailAsync(Int64 mailUid)
	{
		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsJsonAsync("/mail/receive", new ReceiveMailRequest
			{
				MailUid = mailUid
			});

			if (!response.IsSuccessStatusCode)
			{
				return ErrorCode.MailReceiveBadRequest;
			}

			var result = await response.Content.ReadFromJsonAsync<ReceiveMailResponse>();

			return result.Result;

		}
		catch (Exception e)
		{
			return ErrorCode.MailReceiveException;
		}
	}

	public async Task<ErrorCode> SendMailAsync(MailInfo mail)
	{
		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsJsonAsync("/mail/send", new SendMailRequest
			{
				ReceiveUid = mail.ReceiveUid,
				Title = mail.Title,
				Content = mail.Content
			});

			if (!response.IsSuccessStatusCode)
			{
				return ErrorCode.MailSendBadRequest;
			}

			var result = await response.Content.ReadFromJsonAsync<SendMailResponse>();

			return result.Result;

		}
		catch (Exception e)
		{
			return ErrorCode.MailSendException;
		}
	}

	public async Task<ErrorCode> DeleteMailAsync(Int64 mailUid)
	{
		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsJsonAsync("/mail/delete", new DeleteMailRequest
			{
				MailUid = mailUid
			});

			if (!response.IsSuccessStatusCode)
			{
				return ErrorCode.MailDeleteBadRequest;
			}

			var result = await response.Content.ReadFromJsonAsync<DeleteMailResponse>();

			return result.Result;
		}
		catch (Exception e)
		{
			return ErrorCode.MailDeleteException;
		}
	}
}
