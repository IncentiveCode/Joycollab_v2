/// <summary>
/// World 에서 사용할 네트워크 메시지 모음.
/// @author         : HJ Lee
/// @last update    : 2023. 11. 22 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 11. 22) : Mirror.Examples.MultipleMatch 참고해서 최초 생성
/// </summary>

using System;
using Mirror;

namespace Joycollab.v2
{
    /// <summary>
    /// 월드에서 사용할 서버 명령어 
    /// </summary>
    public enum ServerOperation : byte
    {
        None,
        CreateRoom,
        JoinRoom,
        LeaveRoom,
        RemoveFromRoom,
        DeleteRoom,
    }

    /// <summary>
    /// 월드에서 사용할 클라이언트 명령어
    /// </summary>
    public enum ClientOperation : byte
    {
        None,
        RoomList,
        RoomCreated,
        RoomJoined,
        RoomDeparted,
        UpdateRoom,
        UserList,
    }

    /// <summary>
    /// 서버로 전달할 메시지
    /// </summary>
    public struct ServerMessage : NetworkMessage
    {
        public ServerOperation serverOperation;
        public Guid roomId;
    }

    /// <summary>
    /// 클라이언트로 전달할 메시지 
    /// </summary>
    public struct ClientMessage : NetworkMessage
    {
        public ClientOperation clientOperation;
        public Guid roomId;
        public ClasInfo[] roomInfos;
        public WorldAvatarInfo[] avatarInfos;
    }
}