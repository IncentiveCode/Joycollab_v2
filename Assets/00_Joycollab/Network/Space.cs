/// <summary>
/// Network 통신 - Space 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 02. 23
/// @version        : 1.0
/// @update
///     [2023. 02. 23] v1.0 - Joycollab 에서 사용하던 클래스 정리 및 통합 (진행 중)
/// </summary>

using System;

namespace Joycollab.v2
{
    [Serializable]
    public class InfoSpace 
    {
        public string useYn;
        public string nm;
        public int seq = -100;
        public InfoSpaceMng spaceMng;
        public string setSpaceYn;
    }

    [Serializable]
    public class InfoSpaceMng 
    {
        public string useYn;
        public int seq;
        public int num;
    }
}