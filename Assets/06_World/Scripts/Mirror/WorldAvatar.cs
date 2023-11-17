/// <summary>
/// World 에서 사용할 Avatar 움직임 제어 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 11. 07 
/// @version        : 0.3
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성, mirror-test 에서 작업한 항목 migration.
///     v0.2 (2023. 10. 25) : UI 변경 test
///     v0.3 (2023. 11. 07) : 상태 관련 값, Hook function 추가.
/// </summary>

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using Mirror;
using TMPro;

namespace Joycollab.v2
{
    public class WorldAvatar : NetworkBehaviour, iRepositoryObserver
    {
        private const string TAG = "WorldAvatar";

        [Header("Basic info")]
        [SerializeField] private bool isMovable; 
        [SerializeField] private bool isFly;
        [SerializeField, SyncVar] private float speed = 5f;

        [Header("Diagnostics")]
        private float horizontal;
        private float vertical;
        private Vector3 v3Dir = Vector3.zero;
        private Vector3 v3Distance = Vector3.zero;
        private Vector3 v3Target = Vector3.zero;

        [Header("UI")]
        [SerializeField] private Image _imgNameArea;
        private RectTransform rectNameArea;
        [SerializeField] private TMP_Text _txtName;
        [SerializeField] private LocalizeStringEvent _txtState;
        [SerializeField] private RawImage _imgProfile;
        [SerializeField] private Image _imgState;
        private ImageLoader loader;

        [Header("chat")]
        [SerializeField] private Transform _transformBubble;

        [Header("Avatar info")]
        internal static WorldAvatarInfo localPlayerInfo;
        public WorldAvatarInfo AvatarInfo => avatarInfo;

        private WorldAvatarInfo avatarInfo;
        [SyncVar] public int avatarSeq;
        [SyncVar(hook = nameof(SetAvatarName_Hook))] public string avatarName; 
        [SyncVar(hook = nameof(SetAvatarPhoto_Hook))] public string avatarPhoto;
        [SyncVar(hook = nameof(SetAvatarMemberType_Hook))] public string avatarMemberType;
        [SyncVar(hook = nameof(SetAvatarState_Hook))] public string avatarState;
        [SyncVar(hook = nameof(SetAvatarChat_Hook))] public string avatarChat;

        // main camera
        private Camera cam;


    #region Unity functions

        private void Awake() 
        {
            isMovable = false;
            isFly = false;

            if (_imgNameArea != null)
            {
                rectNameArea = _imgNameArea.GetComponent<RectTransform>();
            }

            if (_imgProfile != null) 
            {
                loader = _imgProfile.GetComponent<ImageLoader>();
            }

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
            cam.GetComponent<SquareCamera>().UpdateCameraInfo(transform, 5f);

            isMovable = true;
        }

        private void Update() 
        {
            if (! isOwned) return;
        }

        private void FixedUpdate() 
        {
            if (! isOwned) return;

            if (Input.GetMouseButtonUp(0)) 
            {
                v3Target = cam.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(v3Target, Vector2.zero, 1, LayerMask.GetMask("ClickableObject"));
                if (hit.collider != null) 
                {
                    // Debug.Log($"{TAG} | clickable object click. no move");
                    return;
                }
                else if (EventSystem.current.IsPointerOverGameObject()) 
                {
                    // Debug.Log($"{TAG} | UI click. no move");
                    return;
                }
                else 
                {
                    // Debug.Log($"{TAG} | click position : {v3Target}");
                    v3Target.z = 0f;
                    Fly(v3Target);
                }
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


    #region Public functions

        public void UpdateAvatarInfo(WorldAvatarInfo info) 
        {
            avatarInfo = info;

            avatarSeq = info.seq;
            avatarName = info.nickNm;
            avatarPhoto = info.photo;
            avatarMemberType = info.memberType;
            avatarState = info.stateId;
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

    #endregion


    #region Private functions

        private void Fly(Vector3 position) 
        {
            if (! isOwned) return;
            // if (WorldChatView.Instance.OnChat) return;

            isFly = true;
            v3Dir = position;
        }

        private void Move() 
        {
            if (! isOwned || ! isMovable) return;
            // if (WorldChatView.Instance.OnChat) return;

            // TODO. animation 을 사용하는 경우에 적용 예정.
            // bool isMove = false;

            if (isFly) 
            {
                v3Distance = v3Target - transform.position;
                v3Dir = Vector3.ClampMagnitude(v3Distance, 1f);
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

    #endregion


    #region Avatar info

        public void SetAvatarName_Hook(string _, string newName) 
        {
            Debug.Log($"{TAG} | SetAvatarName_Hook()");

            // avatar 에 이름 반영.
            _txtName.text = newName;

            // name length limiter
            Canvas.ForceUpdateCanvases();
            float rectWidth = rectNameArea.rect.width;
            Debug.Log($"{TAG} | SetAvatarName_Hook(), name width : {rectWidth}");
        }
        [Command(requiresAuthority = false)]
        public void CmdSetAvatarName(string name) 
        {
            Debug.Log($"{TAG} | cmdSetAvatarName(), new name : {name}");
            avatarName = name;
        }

        public void SetAvatarPhoto_Hook(string _, string newPhoto) 
        {
            Debug.Log($"{TAG} | SetAvatarPhoto_Hook()");

            // avatar 에 사진 반영.
            string url = $"{URL.SERVER_PATH}{newPhoto}"; 
            loader.LoadProfile(url, avatarSeq).Forget();
        }
        [Command(requiresAuthority = false)]
        public void CmdSetAvatarPhoto(string photo) 
        {
            Debug.Log($"{TAG} | cmdSetAvatarPhoto(), new photo : {photo}");
            avatarPhoto = photo;
        }

        public void SetAvatarMemberType_Hook(string _, string newType) 
        {
            Debug.Log($"{TAG} | SetAvatarMemberType_Hook()");
        }
        [Command(requiresAuthority = false)]
        public void CmdSetAvatarMemberType(string newType) 
        {
            Debug.Log($"{TAG} | cmdSetAvatarMemberType(), new type : {newType}");
            avatarMemberType = newType;
        }

        public void SetAvatarState_Hook(string _, string stateId) 
        {
            Debug.Log($"{TAG} | SetAvatarState_Hook()");
            ChangeState(stateId);
        }
        [Command(requiresAuthority = false)]
        public void CmdSetAvatarState(string stateId) 
        {
            Debug.Log($"{TAG} | cmdSetAvatarState(), state : {stateId}");
        }

        public void SetAvatarChat_Hook(string _, string chat) 
        {
            WorldChatBubble.Create(_transformBubble, Vector3.zero, chat);
        }

        private void OnTriggerEnter2D(Collider2D other) 
        {
            if (isFly && other.tag.Equals("Room pointer"))
            {
                Debug.Log($"{TAG} | room pointer 도달. 일단 멈춤.");
                isFly = false;
            } 
        }

    #endregion  // AVatar info


    #region Event handling

        public void UpdateInfo(eStorageKey key) 
        {
            if (! isOwned) return;

            if (key == eStorageKey.MemberInfo) 
            {
                // information check
                if (! avatarPhoto.Equals(R.singleton.myPhoto))
                {
                    Debug.Log($"{TAG} | FixedUpdate(), photo : {avatarPhoto}, photo in R : {R.singleton.myPhoto}");
                    CmdSetAvatarPhoto(R.singleton.myPhoto);
                }
                if (! avatarName.Equals(R.singleton.myName))
                {
                    Debug.Log($"{TAG} | FixedUpdate(), name : {avatarName}, name in R : {R.singleton.myName}");
                    CmdSetAvatarName(R.singleton.myName);
                }   
                if (! avatarMemberType.Equals(R.singleton.myMemberType))
                {
                    Debug.Log($"{TAG} | FixedUpdate(), name : {avatarMemberType}, member type in R : {R.singleton.myMemberType}");
                    CmdSetAvatarMemberType(R.singleton.myMemberType);
                }
                if (! avatarState.Equals(R.singleton.myStateId))
                {
                    Debug.Log($"{TAG} | FixedUpdate(), state id : {avatarState}, state id in R : {R.singleton.myStateId}");
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

    #endregion  // Event handling
    }
}