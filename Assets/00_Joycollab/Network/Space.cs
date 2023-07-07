/// <summary>
/// Network 통신 - Space 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 07
/// @version        : 0.4
/// @update
///     v0.1 (2023. 02. 23) : Joycollab 에서 사용하던 클래스 정리 및 통합 (진행 중)
///     v0.2 (2023. 06. 14) : InfoSpace -> SpaceInfo, InfoSpaceMng -> SpaceMng 로 이름 변경.
///     v0.3 (2023. 06. 28) : ResSpaceList, ResSpaceInfo, TopSpace 클래스 추가.
///     v0.4 (2023. 07. 07) : SpaceSeq 클래스 추가.
/// </summary>

using System;
using System.Collections.Generic;

namespace Joycollab.v2
{
    [Serializable]
    public class SpaceInfo 
    {
        public string useYn;
        public string nm;
        public int seq = -100;
        public SpaceMng spaceMng;
        public string setSpaceYn;
    }

    [Serializable]
    public class SpaceMng 
    {
        public string useYn;
        public int seq;
        public int num;
    }

    [Serializable] 
    public class SpaceSeq 
    {
        public string useYn;
        public int seq;
    }

    [Serializable]
    public class CreatorSpaceInfo 
    {
        public string useYn;
        public int seq;
        public string nm;
        public string topSpace;
    }

    [Serializable]
    public class ResSpaceList
    { 
        public List<ResSpaceInfo> list;
    }

    [Serializable]
    public class ResSpaceInfo
    {
        public string useYn;
        public int seq;
        public string nm;
        public List<Seq> members;
        public SpaceMng spaceMng;
        public int arow;
        public int acol;
        public TopSpace topSpace;
        public string setSpaceYn;
    }

    [Serializable]
    public class TopSpace 
    {
        public string useYn;
        public int seq;
        public string nm;
        public List<Seq> members;
        public SpaceMng spaceMng;
        public int arow;
        public int acol;
        public string setSpaceYn;
    }
}