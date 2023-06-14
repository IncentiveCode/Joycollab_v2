/// <summary>
/// Network 통신 - Space 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 06. 14
/// @version        : 0.2
/// @update
///     v0.1 (2023. 02. 23) : Joycollab 에서 사용하던 클래스 정리 및 통합 (진행 중)
///     v0.2 (2023. 06. 14) : InfoSpace -> SpaceInfo, InfoSpaceMng -> SpaceMng 로 이름 변경.
/// </summary>

using System;

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
    public class CreatorSpaceInfo 
    {
        public string useYn;
        public int seq;
        public string nm;
        public string topSpace;
    }
}