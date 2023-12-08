/// <summary>
/// World 에서 사용할 player 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 12. 08 
/// @version        : 0.2
/// @update
///     v0.1 (2023. 12. 06) : 최초 생성, Lobby_Room 에서 참고해서 작성.
///     v0.2 (2023. 12. 08) : mouse wheel event 추가.
/// </summary>

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Mirror;
using TMPro;

namespace Joycollab.v2
{
    public class WorldPlayer : NetworkBehaviour, iRepositoryObserver
    {
        private const string TAG = "WorldPlayer";
        public static WorldPlayer localPlayer;

        [Header("Basic info")]
        private bool isMovable; 
        private bool isFly;
        [SerializeField, SyncVar] private float speed = 5f;

        [Header("avatar info")]
        internal static WorldAvatarInfo localPlayerInfo;
        private WorldAvatarInfo avatarInfo;
        [SyncVar] public int avatarSeq;
        [SyncVar(hook = nameof(SetAvatarName_Hook))] public string avatarName; 
        [SyncVar(hook = nameof(SetAvatarPhoto_Hook))] public string avatarPhoto;
        [SyncVar(hook = nameof(SetAvatarMemberType_Hook))] public string avatarMemberType;
        [SyncVar(hook = nameof(SetAvatarState_Hook))] public string avatarState;
        [SyncVar(hook = nameof(SetAvatarChat_Hook))] public string avatarChat;
        [SyncVar] public string playerName;

        [Header("room info")]
        [SyncVar] public string roomID;
        [SyncVar] public Room currentRoom;
        [SerializeField] private GameObject goRoomPlayer;

        [Header("diagnostics")]
        private float horizontal;
        private float vertical;
        private Vector3 v3Dir = Vector3.zero;
        private Vector3 v3Distance = Vector3.zero;
        private Vector3 v3Target = Vector3.zero;

        [Header("UI")]
        [SerializeField] private Image _imgNameArea;
        private RectTransform rectNameArea;
        private ContentSizeFitter fitterNameArea;
        [SerializeField] private TMP_Text _txtName;
        [SerializeField] private LocalizeStringEvent _txtState;
        [SerializeField] private RawImage _imgProfile;
        [SerializeField] private Image _imgState;
        private ImageLoader loader;

        [Header("chat")]
        [SerializeField] private Transform _transformBubble;

        private Camera cam; 
        private SquareCamera squareCamera;
        private NetworkMatch networkMatch;
        private System.Guid netIdGuid;


    #region Unity functions

        private void Awake() 
        {
            networkMatch = GetComponent<NetworkMatch>();

            // set local variables
            isMovable = isFly = false;
            roomID = string.Empty;

            // get components
            if (_imgNameArea != null) 
            {
                rectNameArea = _imgNameArea.GetComponent<RectTransform>();
                fitterNameArea = _imgNameArea.GetComponent<ContentSizeFitter>();
            }

            if (_imgProfile != null) 
            {
                loader = _imgProfile.GetComponent<ImageLoader>();
            }

            // set observer
            if (R.singleton != null) 
            {
                R.singleton.RegisterObserver(this, eStorageKey.MemberInfo);
            }
        }

        private void Start() 
        {
            if (! isOwned) return;

            cam = Camera.main;
            cam.transform.position = transform.position;

            if (cam.TryGetComponent<SquareCamera>(out squareCamera))
            {
                squareCamera.UpdateCameraInfo(transform, 5f);
            }

            isMovable = true;
        }

        private void FixedUpdate() 
        {
            if (! isOwned) return;

            if (Input.GetMouseButtonUp(0)) 
            {
                if (EventSystem.current.IsPointerOverGameObject()) 
                {
                    // Debug.Log($"{TAG} | UI click. no move");
                    return;
                }

                v3Target = cam.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(v3Target, Vector2.zero, 1, LayerMask.GetMask("ClickableObject"));
                if (hit.collider != null) 
                {
                    // Debug.Log($"{TAG} | clickable object click. no move");
                    return;
                }
                else 
                {
                    // Debug.Log($"{TAG} | click position : {v3Target}");
                    v3Target.z = 0f;
                    Fly(v3Target);
                }
            }
            else if (Input.mouseScrollDelta.y != 0f) 
            {
                squareCamera.HandleWheelEvent(Input.mouseScrollDelta.y);
            }
            else 
            {
                Move();
            }
        }

        private void OnDestroy() 
        {
            if (! isOwned) return;

            if (R.singleton != null) 
            {
                R.singleton.UnregisterObserver(this, eStorageKey.MemberInfo);
            }
        }

    #endregion  // Unity functions


    #region override functions

        public override void OnStartServer() 
        {
            netIdGuid = netId.ToString().ToGuid();
            networkMatch.matchId = netIdGuid;

            playerName = (string) connectionToClient.authenticationData;
        }

        public override void OnStartClient() 
        {
            if (isLocalPlayer) 
            {
                localPlayer = this;
                UpdateAvatarInfo(localPlayerInfo);
            }
            else 
            {
                Debug.Log($"{TAG} | spawning other player... {localPlayerInfo.seq}, {localPlayerInfo.nickNm}");
                goRoomPlayer = LobbyTest.singleton.SpawnPlayerPrefab(this);
            }
        }

        public override void OnStopClient()
        {
            Debug.Log($"{TAG} | client stopped.");
            ClientDisconnect();
        }

        public override void OnStopServer() 
        {
            Debug.Log($"{TAG} | client stopped on server.");
            ServerDisconnect();
        }

    #endregion  // override functions


    #region update avatar 

        public void UpdateAvatarInfo(WorldAvatarInfo info) 
        {
            Debug.Log($"{TAG} | UpdateAvatarInfo() call.");
            /**
            avatarInfo = info;

            avatarSeq = info.seq;
            avatarName = info.nickNm;
            avatarPhoto = info.photo;
            avatarMemberType = info.memberType;
            avatarState = info.stateId;
             */

            CmdUpdateAvatarInfo(info);
        }

        public void UpdateAvatarChat(string chat) 
        {
            if (! string.IsNullOrEmpty(chat))
                avatarChat = chat;
        } 

        public void UpdateAVatarPosition(Vector3 position) 
        {
            if (position != Vector3.zero)
            {
                isFly = true;
                v3Target = position;
            }
        }

    #endregion  // update avatar 


    #region Moving functions

        private void Fly(Vector3 position) 
        {
            if (! isOwned) return;

            isFly = true;
            v3Dir = position;
        }

        private void Move() 
        {
            if (! isOwned || ! isMovable) return;

            if (isFly) 
            {
                v3Distance = v3Target - transform.position;
                v3Dir = Vector3.ClampMagnitude(v3Distance, 1f);
                v3Dir.z = 0f;

                transform.position += v3Dir * speed * Time.deltaTime;
                if (Mathf.Abs(v3Dir.x) <= 0.1f && Mathf.Abs(v3Dir.y) <= 0.1f) 
                {
                    // Debug.Log($"{TAG} | 마우스 클릭 지점 근처까지 이동 완료.");
                    isFly = false;
                }

                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");
                if (horizontal != 0f || vertical != 0f)
                {
                    // Debug.Log($"{TAG} | 마우스 클릭 지점까지 이동하다가 방향키 입력 들어와서 멈춤.");
                    isFly = false;

                    v3Dir = Vector3.ClampMagnitude(new Vector3(horizontal, vertical, 0f), 1f);
                    transform.position += v3Dir * speed * Time.deltaTime;
                }
            }
            else 
            {
                var current = EventSystem.current.currentSelectedGameObject;
                if (current != null) 
                {
                    // Debug.Log($"{TAG} | Move(), current selected object name : {current.name}");
                    return;
                }

                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");
                v3Dir = Vector3.ClampMagnitude(new Vector3(horizontal, vertical, 0f), 1f);
                v3Dir.z = 0f;

                /**
                // TODO. animation 을 사용하는 경우에 적용 예정.
                if (v3Dir.x < 0f) 
                    transform.localScale = new Vector3(-0.5f, 0.5f, 1f);
                else if (v3Dir.x > 0f) 
                    transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                */

                transform.position += v3Dir * speed * Time.deltaTime;

                // TODO. animation 을 사용하는 경우에 적용 예정.
                // isMove = v3Dir.magnitude != 0f;
            }
        }

    #endregion  // Moving functions


    #region hook functions

        public void SetAvatarName_Hook(string _, string newName) 
        {
            Debug.Log($"{TAG} | SetAvatarName_Hook()");

            // avatar 에 이름 반영.
            _txtName.text = newName;
            fitterNameArea.enabled = true;

            // name length limiter
            Canvas.ForceUpdateCanvases();
            float rectWidth = rectNameArea.rect.width;
            float rectHeight = rectNameArea.rect.height;
            if (rectWidth >= 150f) 
            {
                fitterNameArea.enabled = false;
                rectNameArea.sizeDelta = new Vector2(150f, rectHeight);
            }
            Debug.Log($"{TAG} | SetAvatarName_Hook(), name : {newName}, name area width : {rectWidth}");
        }

        public void SetAvatarPhoto_Hook(string _, string newPhoto) 
        {
            Debug.Log($"{TAG} | SetAvatarPhoto_Hook()");

            // avatar 에 사진 반영.
            string url = $"{URL.SERVER_PATH}{newPhoto}"; 
            loader.LoadProfile(url, avatarSeq).Forget();
        }

        public void SetAvatarMemberType_Hook(string _, string newType) 
        {
            Debug.Log($"{TAG} | SetAvatarMemberType_Hook()");
        }

        public void SetAvatarState_Hook(string _, string stateId) 
        {
            Debug.Log($"{TAG} | SetAvatarState_Hook()");
            ChangeState(stateId);
        }

        public void SetAvatarChat_Hook(string _, string chat) 
        {
            WorldChatBubble.Create(_transformBubble, Vector3.zero, chat);
        }

    #endregion  // hook functions


    #region command functions

        [Command(requiresAuthority = false)]
        public void CmdUpdateAvatarInfo(WorldAvatarInfo info) 
        {
            Debug.Log($"{TAG} | cmdUpdateAvatarInfo(), new name : {name}");
            avatarInfo = info;

            avatarSeq = info.seq;
            avatarName = info.nickNm;
            avatarPhoto = info.photo;
            avatarMemberType = info.memberType;
            avatarState = info.stateId;
        }

        [Command(requiresAuthority = false)]
        public void CmdSetAvatarName(string name) 
        {
            Debug.Log($"{TAG} | cmdSetAvatarName(), new name : {name}");
            avatarName = name;
        }

        [Command(requiresAuthority = false)]
        public void CmdSetAvatarPhoto(string photo) 
        {
            Debug.Log($"{TAG} | cmdSetAvatarPhoto(), new photo : {photo}");
            avatarPhoto = photo;
        }

        [Command(requiresAuthority = false)]
        public void CmdSetAvatarMemberType(string newType) 
        {
            Debug.Log($"{TAG} | cmdSetAvatarMemberType(), new type : {newType}");
            avatarMemberType = newType;
        }

        [Command(requiresAuthority = false)]
        public void CmdSetAvatarState(string stateId) 
        {
            Debug.Log($"{TAG} | cmdSetAvatarState(), state : {stateId}");
        }

    #endregion  // command functions 


    #region create room functions
    
        public void CreateRoom(int seq, bool isPublic) 
        {
            string roomID = $"room_{seq}";
            CmdCreateRoom(roomID, isPublic);
        }

        [Command]
        private void CmdCreateRoom(string _roomID, bool isPublic)
        {
            roomID = _roomID;

            if (RoomMaker.singleton.CreateRoom(_roomID, this, isPublic))
            {
                Debug.Log($"{TAG} | <color=green>Game hosted successfully</color>");
                networkMatch.matchId = _roomID.ToGuid();
                RpcCreateRoom(true, _roomID, avatarSeq);
            }
            else 
            {
                Debug.Log($"{TAG} | <color=red>Game hosted failed</color>");
                RpcCreateRoom(false, _roomID, avatarSeq);
            }
        }

        [TargetRpc]
        private void RpcCreateRoom(bool success, string _roomID, int _playerIndex) 
        {
            roomID = _roomID;
            Debug.Log($"{TAG} | room id : {roomID} == {_roomID}");

            // TODO. 방 생성 완료 이벤트 호출.
        }

    #endregion  // create room functions


    #region join room functions

        public void JoinRoom(string _roomID) 
        {
            CmdJoinRoom(_roomID);
        }

        [Command]
        private void CmdJoinRoom(string _roomID) 
        {
            roomID = _roomID;

            if (RoomMaker.singleton.JoinRoom(_roomID, this)) 
            {
                Debug.Log($"{TAG} | <color=green>room joined successfully</color>");
                networkMatch.matchId = _roomID.ToGuid();
                RpcJoinRoom(true, _roomID, avatarSeq);

                // host
                if (isServer && goRoomPlayer != null)
                {
                    goRoomPlayer.SetActive(true);
                }
            }
            else 
            {
                Debug.Log($"{TAG} | <color=red>Game joined failed</color>");
                RpcJoinRoom(false, _roomID, avatarSeq);
            }    
        }

        [TargetRpc]
        private void RpcJoinRoom(bool success, string _roomID, int seq) 
        {
            roomID = _roomID;
            Debug.Log($"{TAG} | RpcJoinRoom(), match id : {_roomID}");

            LobbyTest.singleton.JoinSuccess(success, _roomID);
        }

    #endregion  // join room functions


    #region disconnect functions 

        public void DisconnectGame() 
        {
            CmdDisconnectGame();
        }

        [Command]
        private void CmdDisconnectGame()
        {
            ServerDisconnect();
        }

        private void ServerDisconnect()
        {
            RoomMaker.singleton.PlayerDisconnected(this, roomID);
            RpcDisconnectGame();
            networkMatch.matchId = netIdGuid;
        }

        [TargetRpc]
        private void RpcDisconnectGame() 
        {
            ClientDisconnect();
        }

        private void ClientDisconnect()
        {
            Debug.Log($"{TAG} | ClientDisconnect() call.");
            if (goRoomPlayer != null) 
            {
                if (! isServer)
                    Destroy(goRoomPlayer);
                else
                    goRoomPlayer.SetActive(false);
            }
        }

    #endregion  // disconnect functions 


    #region match player functions

        [Server]
        public void PlayerCountUpdated(int count) 
        {
            RpcPlayerCountUpdated(count);
        }

        [TargetRpc]
        private void RpcPlayerCountUpdated(int count) 
        {
            Debug.Log($"{TAG} | current room {roomID}, user count : {count}");
        }

    #endregion  // match player functions


    #region event handling

        private void OnTriggerEnter2D(Collider2D other) 
        {
            if (other.tag.Equals("Room pointer"))
            {
                // Debug.Log($"{TAG} | room pointer 도달. 일단 멈춤.");
                isFly = false;
            } 
        }

        public void UpdateInfo(eStorageKey key) 
        {
            if (! isOwned) return;

            if (key == eStorageKey.MemberInfo) 
            {
                // information check
                if (! avatarPhoto.Equals(R.singleton.myPhoto))
                {
                    Debug.Log($"{TAG} | UpdateInfo(), photo : {avatarPhoto}, photo in R : {R.singleton.myPhoto}");
                    CmdSetAvatarPhoto(R.singleton.myPhoto);
                }
                if (! avatarName.Equals(R.singleton.myName))
                {
                    Debug.Log($"{TAG} | UpdateInfo(), name : {avatarName}, name in R : {R.singleton.myName}");
                    CmdSetAvatarName(R.singleton.myName);

                    WorldAvatar.localPlayerInfo.nickNm = R.singleton.myName;
                    WorldChatView.localPlayerInfo.nickNm = R.singleton.myName;
                    WorldPlayer.localPlayerInfo.nickNm = R.singleton.myName;
                }   
                if (! avatarMemberType.Equals(R.singleton.myMemberType))
                {
                    Debug.Log($"{TAG} | UpdateInfo(), name : {avatarMemberType}, member type in R : {R.singleton.myMemberType}");
                    CmdSetAvatarMemberType(R.singleton.myMemberType);
                }
                if (! avatarState.Equals(R.singleton.myStateId))
                {
                    Debug.Log($"{TAG} | UpdateInfo(), state id : {avatarState}, state id in R : {R.singleton.myStateId}");
                    CmdSetAvatarState(R.singleton.myStateId); 
                }
            }
        }

        private void ChangeState(string id) 
        {
            switch (id) 
            {
                case S.OFFLINE :
                    _imgNameArea.color = C.OFFLINE;
                    _imgState.color = C.OFFLINE;
                    break;

                case S.MEETING :
                    _imgNameArea.color = C.MEETING;
                    _imgState.color = C.MEETING;
                    break;

                case S.LINE_BUSY :
                    _imgNameArea.color = C.LINE_BUSY;
                    _imgState.color = C.LINE_BUSY;
                    break;

                case S.BUSY :
                    _imgNameArea.color = C.BUSY;
                    _imgState.color = C.BUSY;
                    break;

                case S.OUT_ON_BUSINESS :
                    _imgNameArea.color = C.OUT_ON_BUSINESS;
                    _imgState.color = C.OUT_ON_BUSINESS;
                    break;

                case S.OUTING :
                    _imgNameArea.color = C.OUTING;
                    _imgState.color = C.OUTING;
                    break;

                case S.NOT_HERE :
                    _imgNameArea.color = C.NOT_HERE;
                    _imgState.color = C.NOT_HERE;
                    break;

                case S.DO_NOT_DISTURB :
                    _imgNameArea.color = C.DO_NOT_DISTURB;
                    _imgState.color = C.DO_NOT_DISTURB;
                    break;

                case S.VACATION :
                    _imgNameArea.color = C.VACATION;
                    _imgState.color = C.VACATION;
                    break;

                case S.NOT_AVAILABLE :
                    _imgNameArea.color = C.NOT_AVAILABLE;
                    _imgState.color = C.NOT_AVAILABLE;
                    break;

                case S.ONLINE :
                default :
                    _imgNameArea.color = C.ONLINE;
                    _imgState.color = C.ONLINE;
                    break;
            } 

            _txtState.StringReference.SetReference("Word", $"상태.{id}");
        }

    #endregion  // event handling
    }
}