//Change TODO: 테스트할 게임에 맞게 기능을 구현합니다

using ContentDLL.Scenario;


namespace ContentDLL;

public class DummyObject
{
    public Int32 Index { get; protected set; } = -1;  // 에이전트 내에서 유니크한 번호

    public string UserID { get; protected set; } = string.Empty; // 게임에서 ID라는 것을 사용할 때 사용

    public DummyState CurrnetState { get; protected set; } = new();

    public string SendedChatMessage { get; protected set; } = string.Empty;

    IdWorker _idWorker;



    public void Init(int index, GameScenarioConfig config, IdWorker idWorker)
    {
        _idWorker = idWorker;

        Index = index;
        
        UserID = $"test01_{_idWorker.NextId()}@com2us.com";
          
    }

       

    



}


