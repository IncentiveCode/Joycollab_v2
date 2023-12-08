using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
{
    public class LobbyTest : MonoBehaviour
    {
        private const string TAG = "LobbyTest";

        public static LobbyTest singleton { get; private set; }

        [Header("Lobby")]
        [SerializeField] private Canvas _canvasLobby;
        [SerializeField] private Transform _transformPlayer;        
        [SerializeField] private GameObject _goRoomPlayer;
        [SerializeField] private Button _btnExit;

        private GameObject localRoomPlayer;

    
    #region Unity functions

        private void Awake() 
        {
            _btnExit.onClick.AddListener(DisconnectRoom);
        }

        private void Start() 
        {
            singleton = this;
        }

    #endregion  // Unity functions


    #region event test

        public void JoinSuccess(bool success, string roomID) 
        {
            if (success) 
            {
                _canvasLobby.enabled = true;

                if (localRoomPlayer != null) Destroy(localRoomPlayer);
                localRoomPlayer = SpawnPlayerPrefab(WorldPlayer.localPlayer);
            }
            else 
            {
                Debug.Log($"{TAG} | Join failure.");     
            }
        }

        public GameObject SpawnPlayerPrefab(WorldPlayer player) 
        {
            GameObject prefab = Instantiate(_goRoomPlayer, _transformPlayer);
            prefab.GetComponent<RoomPlayer>().SetPlayer(player);
            prefab.transform.SetAsLastSibling();

            return prefab;
        }

        public void DisconnectRoom() 
        {
            if (localRoomPlayer != null) Destroy(localRoomPlayer);
            WorldPlayer.localPlayer.DisconnectGame();

            _canvasLobby.enabled = false;
        }

    #endregion  // event test
    }
}