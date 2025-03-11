using APIServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIServer.Repository.Interfaces;

public interface IMasterDb
{
    public VersionDAO _version { get; }
    public List<AttendanceRewardData> _attendanceRewardList { get; }    
    public List<GachaRewardData> _gachaRewardList { get; }
    public List<ItemLevelData> _itemLevelList { get; set; }

    public Task<bool> Load();
}
