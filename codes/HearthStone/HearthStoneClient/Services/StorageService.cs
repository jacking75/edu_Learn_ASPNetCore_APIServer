using HearthStoneWeb.Models.Game;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
namespace HearthStoneClient.Services;

public class SessionInfo
{
    public string accountUid { get; set; } = "";
    public string Token { get; set; } = "";

    public bool IsEmpty() { return (string.IsNullOrEmpty(accountUid) || string.IsNullOrEmpty(Token)); }
    public Int64 GetAccountUid() { return int.Parse(accountUid); }
}
public class UserInfo
{
    public string Email = "";
    public string NickName = "";
}

public class CashStorage
{
    public UserInfo? userInfo { get; set; }
    public SessionInfo? sessionInfo { get; set; }
    // Dictionary로 변경: key = asset_name, value = AssetInfo 객체
    public Dictionary<string, AssetInfo>? currencyDictionary { get; set; }
    // Dictionary로 변경: key = item_id, value = ItemInfo 객체
    public Dictionary<int, ItemInfo>? itemDictionary { get; set; }
    public List<Deck>? deckList { get; set; }
    public List<MailInfo>? mailList { get; set; }
    public List<AttendanceInfo>? attendanaceList { get; set; }
    public HSGameInfo? GameInfo { get; set; }
}

public class StorageService
{
    private readonly IJSRuntime _jsRuntime;  // IJSRuntime 추가
    CashStorage _cashStorage;
    public StorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _cashStorage = new CashStorage();
    }

    public void SetSessionInfo(Int64 accountUid, string token)
    {
        if (accountUid <= 0 || string.IsNullOrEmpty(token))
            return;

        _cashStorage.sessionInfo = new SessionInfo { accountUid = accountUid.ToString(), Token = token };
    }

    public SessionInfo? GetSessionInfo()
    {
        try
        {
            if (_cashStorage.sessionInfo != null)
                return _cashStorage.sessionInfo;
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetSessionInfo 오류: {e.Message}");
        }
        return null;
    }

    public void ClearSessionInfo()
    {
    }

    public void SetCreateAccountInfo(string email, string nickname)
    {
        _cashStorage.userInfo = new UserInfo { Email = email, NickName = nickname };
    }

    public (string, string) GetCreateAccountInfo()
    {
        try
        {
            if (_cashStorage.userInfo != null)
                return (_cashStorage.userInfo.Email, _cashStorage.userInfo.NickName);
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetCreateAccountInfo 오류: {e.Message}");
        }
        return ("", "");
    }

    public string GetNickName()
    {
        try
        {
            if (_cashStorage.userInfo != null)
                return _cashStorage.userInfo.NickName;
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetNickName 오류: {e.Message}");
        }

        return "";
    }

    public void ClearCreateAccountInfo()
    {
    }

    public void SetDataLoad(DataLoadResponse dataLoadResponse)
    {
        SetAssetInfo(dataLoadResponse.CurrencyList);
        SetItemInfo(dataLoadResponse.ItemInfoList);
        SetDeckInfo(dataLoadResponse.DeckList);
        SetMailInfo(dataLoadResponse.MailList);
        SetAttendanceInfo(dataLoadResponse.AttendanaceList);
    }

    public void ClearDataLoad()
    {
        ClearAssetInfo();
        ClearItemInfo();
        ClearDeckInfo();
        ClearMailInfo();
        ClearAttendanceInfo();
    }

    // List를 Dictionary로 변경
    public void SetAssetInfo(List<AssetInfo> currencyList)
    {
        // 딕셔너리로 변환
        var currencyDict = currencyList.ToDictionary(c => c.asset_name, c => c);

        _cashStorage.currencyDictionary = currencyDict;
    }

    // 딕셔너리에서 다시 리스트로 변환해 반환하는 호환성 메서드
    public List<AssetInfo>? GetAssetInfo()
    {
        try
        {
            if (_cashStorage.currencyDictionary != null)
                return _cashStorage.currencyDictionary.Values.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetAssetInfo 오류: {e.Message}");
        }

        return null;
    }

    // 딕셔너리에 새 항목 추가 또는 업데이트
    public void AddAssetInfo(List<AssetInfo> currencyList)
    {
        if (_cashStorage.currencyDictionary == null)
            _cashStorage.currencyDictionary = new Dictionary<string, AssetInfo>();

        foreach (var currency in currencyList)
        {
            // 이미 존재하는 경우 값 업데이트
            if (_cashStorage.currencyDictionary.ContainsKey(currency.asset_name))
            {
                _cashStorage.currencyDictionary[currency.asset_name].asset_amount += currency.asset_amount;
            }
            else
            {
                // 새로운 항목 추가
                _cashStorage.currencyDictionary[currency.asset_name] = currency;
            }
        }
    }

    // 딕셔너리에서 직접 조회
    public AssetInfo? GetAssetInfo(string assetName)
    {
        try
        {
            if (_cashStorage.currencyDictionary != null &&
                _cashStorage.currencyDictionary.TryGetValue(assetName, out AssetInfo AssetInfo))
            {
                return AssetInfo;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetAssetInfo(string assetName) 오류: {e.Message}");
        }
        return null;
    }

    public void ClearAssetInfo()
    {
        _cashStorage.currencyDictionary = null;
    }

    // 아이템 정보도 딕셔너리로 변경
    public void SetItemInfo(List<ItemInfo> itemInfoList)
    {
        // 아이템 ID가 중복될 경우 count를 합산하도록 처리
        var itemDict = new Dictionary<int, ItemInfo>();
        foreach (var item in itemInfoList)
        {
            if (itemDict.TryGetValue(item.item_id, out ItemInfo existingItem))
            {
                existingItem.item_cnt += item.item_cnt;
            }
            else
            {
                itemDict[item.item_id] = item;
            }
        }

        _cashStorage.itemDictionary = itemDict;
    }

    // 아이템 추가 또는 업데이트
    public void AddItemInfo(List<ItemInfo> itemList)
    {
        if (_cashStorage.itemDictionary == null)
            _cashStorage.itemDictionary = new Dictionary<int, ItemInfo>();

        foreach (var item in itemList)
        {
            // 이미 존재하는 경우 count 업데이트
            if (_cashStorage.itemDictionary.ContainsKey(item.item_id))
            {
                _cashStorage.itemDictionary[item.item_id].item_cnt += item.item_cnt;
            }
            else
            {
                // 새로운 아이템 추가
                _cashStorage.itemDictionary[item.item_id] = item;
            }
        }
    }

    // 호환성 유지를 위해 딕셔너리에서 리스트로 변환
    public List<ItemInfo> GetItemInfo()
    {
        try
        {
            if (_cashStorage.itemDictionary != null)
                return _cashStorage.itemDictionary.Values.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetItemInfo 오류: {e.Message}");
        }
        return null;
    }

    // 특정 아이템 ID로 조회
    public ItemInfo GetItemInfoById(int itemId)
    {
        try
        {
            if (_cashStorage.itemDictionary != null &&
                _cashStorage.itemDictionary.TryGetValue(itemId, out ItemInfo itemInfo))
            {
                return itemInfo;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetItemInfoById 오류: {e.Message}");
        }
        return null;
    }

    public void ClearItemInfo()
    {
        _cashStorage.itemDictionary = null;
    }

    public void AddDeckInfo(List<Deck> deckList)
    {
        // 기존 덱 정보 가져오기
        var existingDecks = GetDeckInfo() ?? new List<Deck>();

        // 새로운 덱 추가 또는 기존 덱 업데이트
        foreach (var newDeck in deckList)
        {
            // 기존에 같은 ID의 덱이 있는지 확인
            var existingDeck = existingDecks.FirstOrDefault(d => d.deck_id == newDeck.deck_id);

            if (existingDeck != null)
            {
                // 기존 덱 업데이트 (덱 ID는 같고 내용 교체)
                int index = existingDecks.IndexOf(existingDeck);
                existingDecks[index] = newDeck;
            }
            else
            {
                // 새로운 덱 추가
                existingDecks.Add(newDeck);
            }
        }

        _cashStorage.deckList = existingDecks;
    }

    public void SetDeckInfo(List<Deck> deckList)
    {
        _cashStorage.deckList = deckList;
    }

    public List<Deck> GetDeckInfo()
    {
        try
        {
            if (_cashStorage.deckList != null)
                return _cashStorage.deckList;
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetDeckInfo 오류: {e.Message}");
        }
        return null;
    }

    public void ClearDeckInfo()
    {
    }

    public void SetMailInfo(List<MailInfo> mailList)
    {
        _cashStorage.mailList = mailList;
    }

    public List<MailInfo> GetMailInfo()
    {
        try
        {
            if (_cashStorage.mailList != null)
                return _cashStorage.mailList;
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetMailInfo 오류: {e.Message}");
        }
        return null;
    }

    public void ClearMailInfo()
    {
    }

    public void SetAttendanceInfo(List<AttendanceInfo> attendanceList)
    {
        _cashStorage.attendanaceList = attendanceList;
    }

    public List<AttendanceInfo> GetAttendanceInfo()
    {
        try
        {
            if (_cashStorage.attendanaceList != null)
                return _cashStorage.attendanaceList;
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetAttendanceInfo 오류: {e.Message}");
        }
        return null;
    }

    public void ClearAttendanceInfo()
    {
    }
    public void SetGameInfo(HSGameInfo gameInfo)
    {
        _cashStorage.GameInfo = gameInfo;
    }

    public HSGameInfo? GetGameInfo()
    {
        try
        {
            if (_cashStorage.GameInfo != null)
                return _cashStorage.GameInfo;
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetMatchInfo 오류: {e.Message}");
        }
        return null;
    }

    public void ClearGameInfo()
    {
        _cashStorage.GameInfo = null;
    }
}
