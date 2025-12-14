using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameAPIServer.Repository.Interfaces;
using GameAPIServer.Models;
using SqlKata.Execution;

namespace GameAPIServer.Repository;

public partial class GameDb : IGameDb
{
    public async Task<IEnumerable<GdbMailboxInfo>> GetMailList(Int64 uid)
    {
        return await _queryFactory.Query("mailbox").Where("uid", uid)
                                                .Where("expire_dt", ">", DateTime.Now)
                                                .OrderBy("mail_seq")
                                                .GetAsync<GdbMailboxInfo>();
    }

    public async Task<GdbMailboxInfo> GetMailInfo(int mailSeq)
    {
        return await _queryFactory.Query("mailbox").Where("mail_seq", mailSeq)
                                                .FirstOrDefaultAsync<GdbMailboxInfo>();
    }

    public async Task<IEnumerable<GdbMailboxRewardInfo>> GetMailRewardList(int mailSeq)
    {
        return await _queryFactory.Query("mailbox_reward").Where("mail_seq", mailSeq)
                                                .GetAsync<GdbMailboxRewardInfo>();
    }

    public async Task<int> UpdateReceiveMail(int mailSeq)
    {
        return await _queryFactory.Query("mailbox").Where("mail_seq", mailSeq)
                                                .UpdateAsync(new { receive_dt = DateTime.Now, receive_yn = true });
    }

    public async Task<int> DeleteMail(int mailSeq)
    {
        return await _queryFactory.Query("mailbox").Where("mail_seq", mailSeq)
                                                .DeleteAsync();
    }

    public async Task<int> DeleteMailReward(int mailSeq)
    {
        return await _queryFactory.Query("mailbox_reward").Where("mail_seq", mailSeq)
                                                        .DeleteAsync();
    }
}