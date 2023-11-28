/// <summary>
/// World 에서 사용할 네트워크 메시지 모음.
/// @author         : HJ Lee
/// @last update    : 2023. 11. 22 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 11. 22) : Mirror.Examples.MultipleMatch 참고해서 최초 생성
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

/**
    playerMatches 		=> avatarMatches
    openMatches 		=> roomInfos
    matchConnections 	=> connectionsInRoom
    playerInfos 		=> avatarInfos
    waitingConnections 	=> connectionsInCenter
    waitingInfos 		=> avatarsInCenter
 */

namespace Joycollab.v2
{
    public class WorldController : MonoBehaviour
    {
        private const string Tag = "WorldController";
        
        /// <summary>
        /// 이 항목은 확인이 필요함. (for Cross-reference) 
        /// </summary>
        internal static readonly Dictionary<NetworkConnectionToClient, Guid> avatarMatches = new Dictionary<NetworkConnectionToClient, Guid>();

        /// <summary>
        /// 모임방 리스트
        /// </summary>
        internal static readonly Dictionary<Guid, ClasInfo> roomInfos = new Dictionary<Guid, ClasInfo>();

        /// <summary>
        /// 특정 모임방에 들어가 있는 Connection 리스트
        /// </summary>
        internal static readonly Dictionary<Guid, HashSet<NetworkConnectionToClient>> connectionsInRoom = new Dictionary<Guid, HashSet<NetworkConnectionToClient>>();

        /// <summary>
        /// Network Connection 되어 있는 World Avatar Info 
        /// </summary>
        internal static readonly Dictionary<NetworkConnection, WorldAvatarInfo> avatarInfos = new Dictionary<NetworkConnection, WorldAvatarInfo>();

        /// <summary>
        /// Center 에 있는 Network Connection 리스트
        /// </summary>
        internal static readonly List<NetworkConnectionToClient> connectionsInCenter = new List<NetworkConnectionToClient>();

        /// <summary>
        /// 커뮤니티 센터에 있는 사용자 리스트
        /// </summary>
        internal static readonly Dictionary<int, WorldAvatarInfo> avatarsInCenter = new Dictionary<int, WorldAvatarInfo>(); 

        /// <summary>
        /// 특정 모임방에 들어가 있는 사용자 리스트
        /// </summary>
        internal static readonly Dictionary<int, WorldAvatarInfo> avatarsInRoom = new Dictionary<int, WorldAvatarInfo>();

        /// <summary>
        /// GUID of a match the local player has created
        /// </summary>
        internal Guid selectRoomId = Guid.Empty;
        
        // local variables
        internal static WorldAvatarInfo localPlayerInfo;


        [Header("GUI, World, Room")]
        [SerializeField] private GameObject goCenter;
        [SerializeField] private GameObject goRoom;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void ResetStatics()
        {
            avatarMatches.Clear();
            roomInfos.Clear();
            connectionsInRoom.Clear();
            avatarInfos.Clear();
            connectionsInCenter.Clear();
            avatarsInCenter.Clear();
            avatarsInRoom.Clear();
        }
    }
}