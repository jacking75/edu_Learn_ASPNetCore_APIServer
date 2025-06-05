using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Models;

public class GdbUserInfo
{
    public long account_uid { get; set; }
    public int main_deck_id { get; set; }
    public DateTime last_login_dt { get; set; } // 변경: create_dt -> last_login_dt
}

public class GdbDeckInfo
{
    public long account_uid { get; set; } // 추가: PK의 일부
    public int deck_id { get; set; } // get만 있었는데 set도 추가
    public string deck_list { get; set; } // get만 있었는데 set도 추가
    public DateTime create_dt { get; set; } // get만 있었는데 set도 추가
}

public class DeckInfo
{
    public int item_id { get; set; }
}

public class Deck
{
    public Deck() 
    {
        deck_list = new List<DeckInfo>();
    }

    public Deck(int deckId)
    {
        deck_id = deckId;
        deck_list = new List<DeckInfo>();
    }

    public Deck(int deckId, string deckString)
    {
        deck_id = deckId;
        deck_list = new List<DeckInfo>();

        if (!string.IsNullOrEmpty(deckString))
        {
            string[] deckStringList = deckString.Split(',');
            for (int i = 0; i < deckStringList.Length; i++)
            {
                DeckInfo deckInfo = new DeckInfo();
                deckInfo.item_id = int.Parse(deckStringList[i]);
                deck_list.Add(deckInfo);
            }
        }
    }
    public string GetDeckList()
    {
        if (deck_list == null || deck_list.Count == 0)
            return string.Empty;

        return string.Join(",", deck_list.Select(deck => deck.item_id));
    }

    // 카드 추가 메서드
    public bool AddCard(int itemId)
    {
        // 덱 크기 제한(예: 최대 10장)
        if (deck_list.Count >= 10)
            return false;

        // 이미 존재하는 카드인지 확인(옵션)
        if (deck_list.Any(d => d.item_id == itemId))
            return false;

        var deckInfo = new DeckInfo { item_id = itemId };
        deck_list.Add(deckInfo);
        return true;
    }

    // 카드 제거 메서드
    public bool RemoveCard(int itemId)
    {
        int index = deck_list.FindIndex(d => d.item_id == itemId);
        if (index != -1)
        {
            deck_list.RemoveAt(index);
            return true;
        }
        return false;
    }
    public int deck_id { get; set; }
    public List<DeckInfo> deck_list { get; set; }
}

public class GdbUserAssetInfo
{
    public long account_uid { get; set; } // 추가: PK의 일부
    public string asset_name { get; set; }
    public long asset_amount { get; set; }
}

public class AssetInfo : GdbUserAssetInfo
{
}

public class GdbAttendanceInfo
{
    public long account_uid { get; set; } // 추가: PK의 일부
    public int event_id { get; set; } // 일치
    public int attendance_no { get; set; } // 변경: attendance_count -> attendance_no
    public DateTime attendance_dt { get; set; } // 추가: DB 컬럼 있음
}

public class AttendanceInfo : GdbAttendanceInfo
{
}

public class GdbItemInfo
{
    public long account_uid { get; set; } // 추가: PK의 일부
    public int item_id { get; set; } // 변경: string -> int
    public int item_cnt { get; set; } // 변경: count -> item_cnt
}

public class ItemInfo : GdbItemInfo
{
}

public class GdbMailInfo
{
    public long account_uid { get; set; } // 추가: PK의 일부
    public long mail_id { get; set; } // 변경: string -> long (BIGINT)
    public int status { get; set; }
    public int reward_key { get; set; }
    public string mail_desc { get; set; }
    public DateTime received_dt { get; set; } // 변경: string -> DateTime
    public DateTime expire_dt { get; set; } // 변경: string -> DateTime
}

public class MailInfo : GdbMailInfo
{
}