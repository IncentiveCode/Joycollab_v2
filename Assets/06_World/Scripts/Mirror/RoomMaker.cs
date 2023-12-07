/// <summary>
/// World 에서 사용할 RoomMaker 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 12. 06 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 12. 06) : 최초 생성, Lobby_Room 에서 참고해서 작성.
/// </summary>

using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Joycollab.v2
{
    [System.Serializable]
    public class Room 
    {
        public string roomID;
        public bool isPublic;
        public bool inRoom;
        public bool isRoomFull;
        public List<WorldPlayer> players = new List<WorldPlayer>();

        public Room(string _roomID, WorldPlayer player, bool _isPublic) 
        {
            roomID = _roomID;
            isPublic = _isPublic;
            inRoom = isRoomFull = false;
            players.Add(player);
        }

        public Room() { }
    }


    public class RoomMaker : NetworkBehaviour
    {
        private const string TAG = "RoomMaker";
        public static RoomMaker singleton;

        public readonly SyncList<Room> rooms = new SyncList<Room>(); 
        public readonly SyncList<String> roomIDs = new SyncList<String>();
        [SerializeField] int maxRoomPlayers = 100;

        private void Start() 
        {
            singleton = this;
        }

        // public bool CreateRoom(string _roomID, GameObject player, bool isPublic, out int playerIndex) 
        public bool CreateRoom(string _roomID, WorldPlayer player, bool isPublic) 
        {
            if (! roomIDs.Contains(_roomID))
            {
                roomIDs.Add(_roomID);
                Room room = new Room(_roomID, player, isPublic);
                rooms.Add(room);

                Debug.Log($"{TAG} | room generated");
                player.currentRoom = room;
                return true;
            }
            else 
            {
                Debug.Log($"{TAG} | room ID already exists");
                return false;
            }
        }

        // public bool JoinRoom(string _roomID, GameObject player, out int playerIndex) 
        public bool JoinRoom(string _roomID, WorldPlayer player) 
        {
            if (roomIDs.Contains(_roomID))
            {
                for (int i = 0; i < rooms.Count; i++) 
                {
                    if (rooms[i].roomID == _roomID) 
                    {
                        if (!rooms[i].inRoom && !rooms[i].isRoomFull)
                        {
                            rooms[i].players.Add(player);
                            player.currentRoom = rooms[i];

                            rooms[i].players[0].PlayerCountUpdated(rooms[i].players.Count);
                            if (rooms[i].players.Count == maxRoomPlayers)
                                rooms[i].isRoomFull = true;

                            break;
                        }
                        else 
                        {
                            return false;
                        }
                    }
                }

                Debug.Log($"{TAG} | room joined");
                return true;
            }
            else 
            {
                Debug.Log($"{TAG} | room ID does not exists");
                return false;
            }
        }

        public void PlayerDisconnected(WorldPlayer player, string _roomID) 
        {
            for (int i = 0; i < rooms.Count; i++) 
            {
                if (rooms[i].roomID.Equals(_roomID)) 
                {
                    int playerIndex = rooms[i].players.IndexOf(player);
                    rooms[i].players.RemoveAt(playerIndex);
                    Debug.Log($"{TAG} | Player disconnected from match {_roomID} | {rooms[i].players.Count} players remaining");

                    if (rooms[i].players.Count == 0) 
                    {
                        Debug.Log($"{TAG} | No more players in match. terminating {_roomID}, (테스트 이후에는 방 없애지 않고 그대로 둠...)"); 
                        rooms.RemoveAt(i);
                        roomIDs.Remove(_roomID);
                    }
                    else 
                    {
                        rooms[i].players[0].PlayerCountUpdated(rooms[i].players.Count);
                    }
                    break;
                }
            }
        }
    }

    public static class MatchExtension {

        public static System.Guid ToGuid(this string id) 
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.Default.GetBytes(id);
            byte[] hashBytes = provider.ComputeHash(inputBytes);
            return new System.Guid(hashBytes);
        }   
    }
}