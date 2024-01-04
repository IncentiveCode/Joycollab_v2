/// <summary>
/// [world - Square]
/// Network manager 클래스
/// @author         : HJ Lee
/// @last update    : 2024. 01. 04 
/// @version        : 0.2
/// @update
///     v0.1 (2023. 12. 14) : Mirror - MultiSceneNetManager 참고해서 새로 제작
///     v0.2 (2024. 01. 04) : client 시작시 chat window 생성 (테스트)
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class MultiSceneNetworkManager : NetworkManager
    {
        private const string TAG = "MultiSceneNetworkManager";
        public static new MultiSceneNetworkManager singleton { get; private set; }

        [Header("room info")]
        public readonly Dictionary<int, Scene> dictRooms = new Dictionary<int, Scene>();
        public int roomCount;

        [Header("Square, room type")]
        [Scene] public string communityCenter;
        [Scene] public string roomCozy;
        [Scene] public string roomLife;
        [Scene] public string roomBnB;
        [Scene] public string roomDebate;
        [Scene] public string roomSupport;

        bool subscenesLoaded;
        bool addSceneLoaded;


    #region override functions

        public override void Awake() 
        {
            base.Awake();
            singleton = this;
        }

    #endregion  // override functions


    #region Server system callback

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            StartCoroutine(OnServerAddPlayerDelayed(conn));
        }

        private IEnumerator OnServerAddPlayerDelayed(NetworkConnectionToClient conn) 
        {
            while (!subscenesLoaded)
                yield return null;

            // Wait for end of frame before adding the player to ensure Scene Message goes first
            yield return new WaitForEndOfFrame();

            base.OnServerAddPlayer(conn);

            // 사용자 위치 지정. 기존 start position 은 사용 안할 예정.
            Vector3 position = Vector3.zero;

            // 사용자가 가려고 하는 곳 확인
            if (conn.identity.TryGetComponent<WorldPlayer>(out var player)) 
            {
                float limit = 10f;
                while (player.workspaceSeq == -1)
                {
                    limit -= Time.deltaTime;
                    if (limit < 0) 
                    {
                        Debug.Log($"{TAG} | break !"); 
                        break;
                    }
                    yield return null; 
                }
                Debug.Log($"{TAG} | OnServerAddPlayerDelayed(), seq : {player.avatarSeq}, workspace : {player.workspaceSeq}, room type : {player.roomTypeId}");

                // Send Scene message to client to additively load the game scene
                switch (player.roomTypeId) 
                {
                    case S.ROOM_TYPE_COZY :
                        position = new Vector3(-9.837f, 0f, 0f);
                        conn.Send(new SceneMessage { sceneName = roomCozy, sceneOperation = SceneOperation.LoadAdditive });
                        break;

                    case S.ROOM_TYPE_LIFE :
                        position = new Vector3(-9.66f, -1.28f, 0f);
                        conn.Send(new SceneMessage { sceneName = roomLife, sceneOperation = SceneOperation.LoadAdditive });
                        break;

                    case S.ROOM_TYPE_BnB :
                        position = new Vector3(-9.44f, -0.27f, 0f);
                        conn.Send(new SceneMessage { sceneName = roomBnB, sceneOperation = SceneOperation.LoadAdditive });
                        break;

                    case S.ROOM_TYPE_DEBATE :
                        position = new Vector3(-10.36f, -0.79f, 0f);
                        conn.Send(new SceneMessage { sceneName = roomDebate, sceneOperation = SceneOperation.LoadAdditive });
                        break;

                    case S.ROOM_TYPE_SUPPORT :
                        position = new Vector3(-10.49f, -1.21f, 0f);
                        conn.Send(new SceneMessage { sceneName = roomSupport, sceneOperation = SceneOperation.LoadAdditive });
                        break;

                    default :
                        Debug.Log($"{TAG} | OnServerAddPlayerDelayed(), 알 수 없는 공간명 : {player.roomTypeId}");
                        position = new Vector3(-24f, -16f, 0f);
                        conn.Send(new SceneMessage { sceneName = communityCenter, sceneOperation = SceneOperation.LoadAdditive });
                        break;
                }
            }
            else 
            {
                position = new Vector3(-24f, -16f, 0f);
                conn.Send(new SceneMessage { sceneName = communityCenter, sceneOperation = SceneOperation.LoadAdditive });
            }

            Debug.Log($"{TAG} | client request workspace seq : {player.workspaceSeq}");
            Debug.Log($"{TAG} | is scene exist ? : {dictRooms.ContainsKey(player.workspaceSeq)}");
            Debug.Log($"{TAG} | player position : {position}");
            if (dictRooms.ContainsKey(player.workspaceSeq))
            {
                SceneManager.MoveGameObjectToScene(conn.identity.gameObject, dictRooms[player.workspaceSeq]);
                conn.identity.gameObject.transform.position = position;
            }
            else 
            {
                // TODO.
                Debug.Log($"{TAG} | error handling 추가 예정");
            }
        }

    #endregion  // Server system callback


    #region Start & Stop callback

        public override void OnStartServer() 
        {
            ServerLoadSubRoomsAsync().Forget();
        }

        private async UniTaskVoid ServerLoadSubRoomsAsync() 
        {
            int index = 1;

            string url = URL.SIMPLE_CLAS_LIST;
            PsResponse<SimpleClasList> resClas = await NetworkTask.RequestAsync<SimpleClasList>(url, eMethodType.GET);
            Debug.Log($"{TAG} | simple clas list, count : {resClas.data.list.Count}");
            foreach (var info in resClas.data.list) 
            {
                switch (info.clas.themes.id) 
                {
                    case S.ROOM_TYPE_COZY :
                        await SceneManager.LoadSceneAsync(roomCozy, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
                        break;

                    case S.ROOM_TYPE_LIFE :
                        await SceneManager.LoadSceneAsync(roomLife, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
                        break;

                    case S.ROOM_TYPE_BnB :
                        await SceneManager.LoadSceneAsync(roomBnB, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
                        break;

                    case S.ROOM_TYPE_DEBATE :
                        await SceneManager.LoadSceneAsync(roomDebate, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
                        break;

                    case S.ROOM_TYPE_SUPPORT :
                        await SceneManager.LoadSceneAsync(roomSupport, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
                        break;

                    default :
                        await SceneManager.LoadSceneAsync(communityCenter, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
                        break;
                }

                Scene newScene = SceneManager.GetSceneAt(index); 
                dictRooms.Add(info.seq, newScene);
                Debug.Log($"{TAG} | ServerLoadSubScenes(), index : {index}, scene theme : {info.clas.themes.id}");
                index ++;
            }

            subscenesLoaded = true;
            addSceneLoaded = true;
        }

        public override void OnStopServer()
        {
            NetworkServer.SendToAll(new SceneMessage { sceneOperation = SceneOperation.UnloadAdditive });
            ServerUnloadSubRoomsAsync().Forget();
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            WorldPlayer.playerNames.Remove(conn);
            base.OnServerDisconnect(conn);
        }

        private async UniTaskVoid ServerUnloadSubRoomsAsync() 
        {
            foreach (var roomInfo in dictRooms) 
            {
                if (roomInfo.Value.IsValid())
                {
                    await SceneManager.UnloadSceneAsync(roomInfo.Value);
                }
            }

            dictRooms.Clear();
            subscenesLoaded = false;
            addSceneLoaded = false;

            await Resources.UnloadUnusedAssets();
        }

        public override void OnStopClient()
        {
            if (mode == NetworkManagerMode.Offline)
                ClientUnloadSubRoomsAsync().Forget();
        }

        private async UniTaskVoid ClientUnloadSubRoomsAsync() 
        {
            for (int i = 0; i < SceneManager.sceneCount; i++) 
            {
                if (SceneManager.GetSceneAt(i) != SceneManager.GetActiveScene()) 
                {
                    await SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
                }
            } 
        }

    #endregion  // Start & Stop callback


    #region Add sub room

        public async UniTaskVoid AddSubRoom(GameObject player, int workspaceSeq, string themeId) 
        {
            addSceneLoaded = await AddSubRoomAsync(workspaceSeq, themeId);

            if (addSceneLoaded)
                SceneManager.MoveGameObjectToScene(player, dictRooms[workspaceSeq]);
        }

        [ServerCallback]
        private async UniTask<bool> AddSubRoomAsync(int workspaceSeq, string themeId)
        {
            addSceneLoaded = false;

            switch (themeId) 
            {
                case S.ROOM_TYPE_COZY :
                    await SceneManager.LoadSceneAsync(roomCozy, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
                    break;

                case S.ROOM_TYPE_LIFE :
                    await SceneManager.LoadSceneAsync(roomLife, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
                    break;

                case S.ROOM_TYPE_BnB :
                    await SceneManager.LoadSceneAsync(roomBnB, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
                    break;

                case S.ROOM_TYPE_DEBATE :
                    await SceneManager.LoadSceneAsync(roomDebate, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
                    break;

                case S.ROOM_TYPE_SUPPORT :
                    await SceneManager.LoadSceneAsync(roomSupport, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
                    break;

                default :
                    await SceneManager.LoadSceneAsync(communityCenter, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
                    break;
            }

            int index = dictRooms.Count;
            Scene newScene = SceneManager.GetSceneAt(index);
            dictRooms.Add(workspaceSeq, newScene);

            return true;
        }

    #endregion  // Add sub room
    }
}