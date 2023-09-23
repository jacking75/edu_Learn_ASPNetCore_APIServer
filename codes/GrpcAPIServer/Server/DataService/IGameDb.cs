public interface IGameDb
{
    public Task<Tuple<ErrorCode, Int64>> InsertCharacter(Int64 accountId, string nickName);

    public Task<ErrorCode> InsertCharacterItem(Int64 characterId);

    public Task<ErrorCode> DeleteCharacter(string nickName);

    public Task<ErrorCode> DeleteCharacterItem(Int64 characterId);

    public Task<Tuple<ErrorCode, IEnumerable<ModelGameDB.CharacterInfo>>> GetCharacterList(Int64 accountId);


}