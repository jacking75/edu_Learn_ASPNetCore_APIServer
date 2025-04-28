using GameClient.Models;
using GameShared.DTO;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace GameClient.Services;

public class MailService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public List<MailInfo> MailList { get; private set; } = new();
    public int UnreadMailCount { get; private set; }

    public MailService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ErrorCode> GetMailsAsync()
    {
        try
        {
            MailList.Clear();

			var gameClient = _httpClientFactory.CreateClient("Game");
            var response = await gameClient.PostAsync("/mail/list", null);

            if (!response.IsSuccessStatusCode)
            {
                return ErrorCode.MailListFailBadRequest;
            }

            var result = await response.Content.ReadFromJsonAsync<MailListResponse>();

            if (result == null || result.MailList == null)
            {
                return ErrorCode.MailListFailInvalidResponse;
            }

            if (ErrorCode.None != result.Result)
            {
                return result.Result;
            }

			MailList.AddRange(result.MailList);
			UnreadMailCount = MailList.Count((e) => e.StatusCode == MailStatusCode.Unread);

            return ErrorCode.None;
        }
        catch (Exception e)
        {
			Console.WriteLine(e.Message);
			return ErrorCode.MailListFailException;
        }
    }

    public async Task<ErrorCode> ReadMailAsync(Int64 mailUid)
    {
        try
        {
            var gameClient = _httpClientFactory.CreateClient("Game");
            var response = await gameClient.PostAsJsonAsync("/mail/read", new MailReadRequest
            {
                MailUid = mailUid
            });

            if (!response.IsSuccessStatusCode)
            {
                return ErrorCode.MailReadBadRequest;
            }

            var result = await response.Content.ReadFromJsonAsync<MailReadResponse>();

            if (result == null || ErrorCode.None != result.Result)
            {
				Console.WriteLine(result?.Result);
				return ErrorCode.MailReadFail;
			}

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return ErrorCode.MailReadException;
        }
    }

    public async Task<ErrorCode> ReceiveMailAsync(Int64 mailUid)
    {
        try
        {
            var gameClient = _httpClientFactory.CreateClient("Game");
            var response = await gameClient.PostAsJsonAsync("/mail/receive", new MailReceiveRequest
            {
                MailUid = mailUid
            });

            if (!response.IsSuccessStatusCode)
            {
                return ErrorCode.MailReceiveBadRequest;
            }

            var result = await response.Content.ReadFromJsonAsync<MailReceiveResponse>();

            if (result == null || ErrorCode.None != result.Result)
            {
				Console.WriteLine(result?.Result);
				return ErrorCode.MailReceiveFailRequest;
            }

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return ErrorCode.MailReceiveException;
        }
    }

    public async Task<ErrorCode> SendMailAsync(Int64 uid, string title, string content)
    {
        try
        {
            var gameClient = _httpClientFactory.CreateClient("Game");
            var response = await gameClient.PostAsJsonAsync("/mail/send", new MailSendRequest
            {
                ReceiverUid = uid,
                Title = title,
                Content = content
            });

            if (!response.IsSuccessStatusCode)
            {
                return ErrorCode.MailSendBadRequest;
            }

            var result = await response.Content.ReadFromJsonAsync<MailSendResponse>();
            
            if (result == null || ErrorCode.None != result.Result)
            {
                return ErrorCode.MailSendFailRequest;
            }

            return ErrorCode.None;

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return ErrorCode.MailSendException;
        }
    }

    public async Task<ErrorCode> DeleteMailAsync(Int64 mailUid)
    {
        try
        {
            var gameClient = _httpClientFactory.CreateClient("Game");
            var response = await gameClient.PostAsJsonAsync("/mail/delete", new MailDeleteRequest
            {
                MailUid = mailUid
            });

            if (!response.IsSuccessStatusCode)
            {
                return ErrorCode.MailDeleteBadRequest;
            }

            var result = await response.Content.ReadFromJsonAsync<MailDeleteResponse>();

            if (result == null || ErrorCode.None != result.Result)
            {
                return ErrorCode.MailDeleteFailRequest;
            }

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return ErrorCode.MailDeleteException;
        }
    }
}