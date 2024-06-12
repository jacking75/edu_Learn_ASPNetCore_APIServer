using APIServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIServer.Repository.Interfaces;

public interface IMasterDb
{
    public VersionDAO _version { get; }
    public List<AttendanceRewardData> _attendanceRewardList { get; }
    public List<CharacterData> _characterList { get; }
    public List<SkinData> _skinList { get; }
    public List<CostumeData> _costumeList { get; }
    public List<CostumeSetData> _costumeSetList { get; }
    public List<FoodData> _foodList { get; }
    public List<SkillData> _skillList { get; }
    public List<GachaRewardData> _gachaRewardList { get; }

    public List<ItemLevelData> _itemLevelList { get; set; }
    public Task<bool> Load();
}
