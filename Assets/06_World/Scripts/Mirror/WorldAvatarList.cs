/// <summary>
/// 현재 Server 에 접속한 사용자 정보를 출력하기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 11. 22 
/// @version        : 0.2
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성, mirror-test 에서 작업한 항목 migration.
///     v0.2 (2023. 11. 22) : WorldController 으로 이동.
/// </summary>

using System.Collections.Generic;
using Mirror;

namespace Joycollab.v2
{
    public class WorldAvatarList : NetworkBehaviour
    {
        internal static readonly HashSet<WorldAvatarInfo> avatarInfos = new HashSet<WorldAvatarInfo>();
        
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        public static void ResetStatics() 
        {
            avatarInfos.Clear();
        }
    }
}