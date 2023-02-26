/// <summary>
/// Network 통신 - 설정, 공통 코드 등의 정보 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 02. 23
/// @version        : 1.0
/// @update
///     [2023. 02. 23] v1.0 - Joycollab 에서 사용하던 클래스 정리 및 통합 (진행 중)
/// </summary>

using System;

namespace PitchSolution
{
    [Serializable]
    public class TpsList
    {
        public TpsInfo[] list;
    }

    [Serializable]
    public class TpsInfo
    {
        public string useYn;
        public int cd;
        public string id;
        public string nm;
        public int ord;
        public string refVal;
    }


    [Serializable]
    public class LangSettingInfo
    {
        public string useYn;
        public string id;
        public string nm;
        public int ord;
    }    

    [Serializable]
    public class SettingInfo
    {
        public string useYn;
        public int seq;
        public alarmOptItemInfo[] alarmOptItems;
        public alarmOptItemInfo[] alarmOptSounds;
        public bool bottomPopupNotice;
    }

    [Serializable]
    public class alarmOptItemInfo
    {
        public string useYn;
        public string seq;
        public alarmOptItemTpInfo tp;
        public bool alarm;
    }

    [Serializable]
    public class alarmOptItemTpInfo
    {
        public string useYn;
        public int cd;
        public string id;
        public int ord;
        public string refVal;
    }
}