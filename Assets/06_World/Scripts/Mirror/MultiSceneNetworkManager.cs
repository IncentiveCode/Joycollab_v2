/// <summary>
/// [world - Square]
/// Network manager 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 12. 14 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 12. 14) : Mirror - MultiSceneNetManager 참고해서 새로 제작
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
            base.OnServerAddPlayer(conn);
            StartCoroutine(OnServerAddPlayerDelayed(conn));
        }

        private IEnumerator OnServerAddPlayerDelayed(NetworkConnectionToClient conn) 
        {
            while (!subscenesLoaded)
                yield return null;

            // Wait for end of frame before adding the player to ensure Scene Message goes first
            // yield return new WaitForEndOfFrame();
            // base.OnServerAddPlayer(conn);
            yield return new WaitForSeconds(1);

            // 사용자가 가려고 하는 곳 확인
            if (conn.identity.TryGetComponent<WorldPlayer>(out var player)) 
            {
                Debug.Log($"{TAG} | OnServerAddPlayerDelayed(), seq : {player.avatarSeq}, workspace : {player.workspaceSeq}, room type : {player.roomTypeId}");

                // Send Scene message to client to additively load the game scene
                switch (player.roomTypeId) 
                {
                    case S.ROOM_TYPE_COZY :
                        conn.Send(new SceneMessage { sceneName = roomCozy, sceneOperation = SceneOperation.LoadAdditive });
                        break;

                    case S.ROOM_TYPE_LIFE :
                        conn.Send(new SceneMessage { sceneName = roomLife, sceneOperation = SceneOperation.LoadAdditive });
                        break;

                    case S.ROOM_TYPE_BnB :
                        conn.Send(new SceneMessage { sceneName = roomBnB, sceneOperation = SceneOperation.LoadAdditive });
                        break;

                    case S.ROOM_TYPE_DEBATE :
                        conn.Send(new SceneMessage { sceneName = roomDebate, sceneOperation = SceneOperation.LoadAdditive });
                        break;

                    case S.ROOM_TYPE_SUPPORT :
                        conn.Send(new SceneMessage { sceneName = roomSupport, sceneOperation = SceneOperation.LoadAdditive });
                        break;

                    default :
                        conn.Send(new SceneMessage { sceneName = communityCenter, sceneOperation = SceneOperation.LoadAdditive });
                        break;
                }
            }
            else 
            {
                conn.Send(new SceneMessage { sceneName = communityCenter, sceneOperation = SceneOperation.LoadAdditive });
            }

            Debug.Log($"{TAG} | dictionary count : {dictRooms.Count}");
            Debug.Log($"{TAG} | is scene exist ? : {dictRooms.ContainsKey(player.workspaceSeq)}");


            foreach (var info in dictRooms)
            {
                Debug.Log($"{TAG} | key : {info.Key} = value : {info.Value}");
            }


            if (dictRooms.Count > 0 && dictRooms.ContainsKey(player.workspaceSeq))
            {
                SceneManager.MoveGameObjectToScene(conn.identity.gameObject, dictRooms[player.workspaceSeq]);
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
            await SceneManager.LoadSceneAsync(communityCenter, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
            dictRooms.Add(178, SceneManager.GetSceneAt(index));

            index = 2;
            await SceneManager.LoadSceneAsync(roomCozy, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
            dictRooms.Add(1213, SceneManager.GetSceneAt(index));

            /**
            int page = 1;
            int index = 1;

            PsResponse<ClasList> resClas = null;
            do {
                string url = string.Format(URL.CLAS_LIST, page, 20);
                Debug.Log($"{TAG} | ServerLoadSubRooms(), url : {url}");
                resClas = await NetworkTask.RequestAsync<ClasList>(url, eMethodType.GET, string.Empty, R.singleton.token);

                foreach (var info in resClas.data.content) 
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
            }
            while(! resClas.data.last);
             */

            subscenesLoaded = true;
            addSceneLoaded = true;
        }


        public override void OnStopServer()
        {
            NetworkServer.SendToAll(new SceneMessage { sceneOperation = SceneOperation.UnloadAdditive });
            ServerUnloadSubRoomsAsync().Forget();
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

        public void AddSubRoom(int workspaceSeq, string themeId) 
        {
            addSceneLoaded = false;
            AddSubRoomAsync(workspaceSeq, themeId).Forget();
        }

        [ServerCallback]
        private async UniTaskVoid AddSubRoomAsync(int workspaceSeq, string themeId)
        {
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

            addSceneLoaded = true;
        }

    #endregion  // Add sub room
    }
}