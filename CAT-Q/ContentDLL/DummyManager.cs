using ContentDLL.Scenario;


namespace ContentDLL;

public class DummyManager
{
    List<DummyObject> _dummyList = new();
    Dictionary<string, DummyObject> _dummyDic = new();

    IdWorker _idWorker = new IdWorker(1, 1);

    public GameScenarioConfig ScenarioConfig { get; private set; } = null;
        

    public int Init(GameScenarioConfig config)
    {
        ScenarioConfig = config;

        for (int i = 0; i < ScenarioConfig.DummyCount; ++i)
        {
            DummyObject dummy = new DummyObject();
            dummy.Init(i, ScenarioConfig, _idWorker);

            _dummyList.Add(dummy);
            _dummyDic.Add(dummy.UserID, dummy);
        }

        return ScenarioConfig.DummyCount;
    }
   

    public int TotalDummyCount()
    {
        return _dummyList.Count;
    }

    public DummyObject GetDummyByIndex(int index)
    {
        try
        {
            return _dummyList[index];
        }
        catch { return null; }
    }

    public DummyObject GetDummyByID(string ID)
    {
        try
        {
            return _dummyDic[ID];
        }
        catch { return null; }
    }
           
            
  
}
