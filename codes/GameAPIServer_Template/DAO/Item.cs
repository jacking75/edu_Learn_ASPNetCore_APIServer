using System;

namespace GameAPIServer.DAO
{
    public class GdbUserCharInfo
    {
        public int uid { get; set; }
        public int char_key { get; set; }
        public int char_level { get; set; }
        public int char_cnt { get; set; }
        public int skin_key { get; set; }
        public string costume_json { get; set; }
    }

    public class GdbUserCostumeInfo
    {
        public int uid { get; set; }
        public int costume_key { get; set; }
        public int costume_level { get; set; }
        public int costume_cnt { get; set; }
        public DateTime create_dt { get; set; }
    }

    public class GdbUserCharRandomSkillInfo
    {
        public int uid { get; set; }
        public int char_key { get; set; }
        public int index_num { get; set; }
        public int skill_key { get; set; }
        public DateTime create_dt { get; set; }
    }

    public class GdbUserSkinInfo
    {
        public int uid { get; set; }
        public int skin_key { get; set; }
        public DateTime create_dt { get; set; }
    }

    public class GdbUserFoodInfo
    {
        public int uid { get; set; }
        public int food_key { get; set; }
        public int food_qty { get; set; }
        public int food_level { get; set; }
        public int food_gear_qty { get; set; }
        public DateTime create_dt { get; set; }
    }

    public class CharCostumeInfo
    {
        public int Head { get; set; }
        public int Face { get; set; }
        public int Hand { get; set; }
    }

}
