namespace APIServer.MasterData;

public class Manager : IManager
{
    private readonly Artifacts _artifact = new();
    private readonly ArrayExample _tests = new();
    private readonly CharacterAbilityByLevel _characterAbilityByLevel = new();

    public void Load(string filePath)
    {
        _tests.Load(filePath);
        _artifact.Load(filePath);
        _characterAbilityByLevel.Load(filePath);
    }
}