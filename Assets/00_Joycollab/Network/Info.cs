/// <summary>
/// Network 통신 - 설정, 공통 코드 등의 정보 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 09. 22
/// @version        : 0.4
/// @update
///     v0.1 (2023. 02. 23) : Joycollab 에서 사용하던 클래스 정리 및 통합 (진행 중)
///     v0.2 (2023. 06. 14) : seq 하나만 담고 있는 Seq 클래스 추가
///     v0.3 (2023. 06. 15) : array 를 list 로 변경
///     v0.4 (2023. 09. 22) : cd 하나만 담고 있는 Cd 클래스 추가
/// </summary>

using System;
using System.Collections.Generic;

namespace Joycollab.v2
{
    [Serializable]
    public class TpsList
    {
        public List<TpsInfo> list;

        public TpsList() 
        {
            list = new List<TpsInfo>();
            list.Clear();
        }
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
        public List<alarmOptItemInfo> alarmOptItems;
        public List<alarmOptItemInfo> alarmOptSounds;
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

    [Serializable]
    public class Seq 
    {
        public int seq;

        public Seq(int seq) 
        {
            this.seq = seq;
        }
    }

    [Serializable] 
    public class Cd 
    {
        public int cd;
    }
}