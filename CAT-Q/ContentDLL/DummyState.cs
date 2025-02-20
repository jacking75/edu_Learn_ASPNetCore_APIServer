namespace ContentDLL;


//Change TODO: 게임에 맞는 적절한 상태를 정의한다.


public class DummyState
{
    DummyStateType _state = DummyStateType.None;


    public void Clear()
    {
        _state = DummyStateType.None;
    }

    public void SetState(DummyStateType state)
    {
        _state = state;
    }

    public DummyStateType Cur()
    {
        return _state;
    }

    public bool IsLogin()
    {
        return _state == DummyStateType.Login;
    }
}

public enum DummyStateType
{
    None = 0,
    Connected = 1,
    Login = 2,
    Room = 3,
    Game = 4,

}