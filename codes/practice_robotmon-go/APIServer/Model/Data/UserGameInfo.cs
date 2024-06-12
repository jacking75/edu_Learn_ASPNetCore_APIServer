namespace ApiServer.Model.Data
{
    public class UserGameInfo
    {
        public UserGameInfo(Int32 userLevel, Int32 userExp, Int32 starPoint, Int32 upgradeCandy)
        {
            UserExp = userExp; 
            UserLevel = userLevel; 
            StarPoint = starPoint; 
            UpgradeCandy = upgradeCandy;
        }

        public Int32 UserLevel { get; set; }
        public Int32 UserExp { get; set; }
        public Int32 StarPoint { get; set; } // 별의 모래
        public Int32 UpgradeCandy { get; set; }
    }
}
