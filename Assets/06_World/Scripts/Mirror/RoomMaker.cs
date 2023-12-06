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
        public List<GameObject> players = new List<GameObject>();

        public Room(string roomID, GameObject player) 
        {
            this.roomID = roomID;
            this.players.Add(player);
        }

        public Room() { }
    }

    [System.Serializable]
    public class ListPlayer : List<GameObject> { }

    public class RoomMaker : NetworkBehaviour
    {
        private const string TAG = "MatchMaker";
        public static RoomMaker singleton;

        public readonly SyncList<Room> rooms = new SyncList<Room>(); 
        public readonly SyncList<string> roomIDs = new SyncList<string>();

        [SerializeField] GameObject goTurnManager;

        private void Start() 
        {
            singleton = this;
        }

        public bool HostGame(string _matchID, GameObject player, bool isPublic, out int playerIndex) 
        {
            playerIndex = -1;

            if (! roomIDs.Contains(_matchID))
            {
                roomIDs.Add(_matchID);
                Room room = new Room(_matchID, player);
                room.isPublic = isPublic;
                rooms.Add(room);
                Debug.Log($"{TAG} | Match generated");

                playerIndex = 1;
                return true;
            }
            else 
            {
                Debug.Log($"{TAG} | Match ID already exists");
                return false;
            }
        }

        public bool JoinGame(string roomID, GameObject player, out int playerIndex) 
        {
            playerIndex = -1;

            if (roomIDs.Contains(roomID))
            {
                for (int i = 0; i < rooms.Count; i++) 
                {
                    if (rooms[i].roomID == roomID) 
                    {
                        rooms[i].players.Add(player);
                        playerIndex = rooms[i].players.Count;
                        break;
                    }
                }

                Debug.Log($"{TAG} | Match joined");
                return true;
            }
            else 
            {
                Debug.Log($"{TAG} | Match ID does not exists");
                return false;
            }
        }

        public void BeginGame(string roomID) 
        {
            GameObject newTurnManager = Instantiate(goTurnManager);
            NetworkServer.Spawn(newTurnManager);

            newTurnManager.GetComponent<NetworkMatch>().matchId = roomID.ToGuid();
            TurnManager turnManager = newTurnManager.GetComponent<TurnManager>(); 
            for (int i = 0; i< rooms.Count; i++) 
            {
                if (rooms[i].roomID == roomID) 
                {
                    foreach (var player in rooms[i].players) 
                    {
                        WorldPlayer p = player.GetComponent<WorldPlayer>();
                        turnManager.AddPlayer(p);
                        // p.StartGame();
                    }
                    break;
                }
            }
        }

        public bool SearchGame(GameObject player, out int playerIndex, out string matchID) 
        {
            playerIndex = -1;
            matchID = string.Empty;

            for (int i = 0; i < rooms.Count; i++) 
            {
                if (rooms[i].isPublic && !rooms[i].isRoomFull && !rooms[i].inRoom) 
                {
                    matchID = rooms[i].roomID;
                    if (JoinGame(matchID, player, out playerIndex))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static string GetRandomMatchID() 
        {
            string _id = string.Empty;
            int _no = 0;
            for (int i = 0; i < 5; i++) 
            {
                _no = Random.Range(0, 36); 
                if (_no < 26)
                    _id += (char)(_no + 65);
                else 
                    _id += (_no - 26).ToString();
            }
            Debug.Log($"{TAG} | Random match id : {_id}");
            return _id;
        } 

        public void PlayerDisconnected(WorldPlayer player, string roomID) 
        {
            for (int i = 0; i < rooms.Count; i++) 
            {
                if (rooms[i].roomID.Equals(roomID)) 
                {
                    int playerIndex = rooms[i].players.IndexOf(player.gameObject);
                    rooms[i].players.RemoveAt(playerIndex);
                    Debug.Log($"{TAG} | Player disconnected from match {roomID} | {rooms[i].players.Count} players remaining");

                    if (rooms[i].players.Count == 0) 
                    {
                        Debug.Log($"{TAG} | No more players in match. terminating {roomID}, (방 없애지 않고 그대로 둠...)"); 
                        // rooms.RemoveAt(i);
                        // matchIDs.Remove(_matchID);
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