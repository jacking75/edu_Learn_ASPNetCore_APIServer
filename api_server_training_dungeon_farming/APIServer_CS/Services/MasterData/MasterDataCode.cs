using System;

namespace APIServer.Services.MasterData;

public class MasterDataCode
{
    public static bool IsPossibleOverlap(Int32 itemTypeCode)
    {
        switch (itemTypeCode)
        {
            case (int)ItemTypeCode.돈:
                return true;
        }

        return false;
    }


    public enum ItemCode : Int32
    {
        게임돈 = 1,
        작은칼 = 2,
        도금칼 = 3,
        나무방패 = 4,
        보통모자 = 5,
        포션 = 6
    }


    public enum ItemTypeCode : Int32
    {
        무기 = 1,
        방어구 = 2,
        복장 = 3,
        마법도구 = 4,
        돈 = 5,
    }


    public enum MailCode : Int32
    {
        유료상품 = 1001,
        이벤트상품 = 1002,
        운영툴지급상품 = 1003,
    }


    public enum StageCode : Int32
    {
        스테이지_1 = 1,
        스테이지_2 = 2,
    }


    public enum StageEnemyCode : Int32
    {
        스테이지_1_101 = 101,
        스테이지_1_110 = 110,
        스테이지_2_201 = 201,
        스테이지_2_211 = 211,
        스테이지_2_221 = 221,
    }


    public enum InAppPID : Int32
    {
        초보상품 = 1,
        중수상품 = 2,
        고수상품 = 3,
    }



}
