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
    public class WorldAvatar : NetworkBehaviour
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

        // sprite render 를 사용하는 경우
        // [SerializeField] private SpriteRenderer _rendererProfile;

        [Header("Avatar info")]
        internal static WorldAvatarInfo localPlayerInfo;
        public WorldAvatarInfo AvatarInfo => avatarInfo;

        private WorldAvatarInfo avatarInfo;
        [SyncVar] public int avatarSeq;
        [SyncVar(hook = nameof(SetAvatarName_Hook))] public string avatarName; 
        [SyncVar(hook = nameof(SetAvatarPhoto_Hook))] public string avatarPhoto;
        [SyncVar] public string avatarMemberType;
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

            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                v3Target = cam.ScreenToWorldPoint(Input.mousePosition);
                // Debug.Log($"{TAG} | click position : {v3Target}");
                v3Target.z = 0f;
                Fly(v3Target);
            }
        }

        private void FixedUpdate() 
        {
            Move();
        }

        private void OnDestroy() 
        {
            if (! isOwned) return;
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
            if (WorldChatView.Instance.OnChat) return;

            isFly = true;
            v3Dir = position;
        }

        private void Move() 
        {
            if (! isOwned || ! isMovable) return;
            if (WorldChatView.Instance.OnChat) return;

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
            // avatar 에 이름 반영.
            _txtName.text = newName;

            // name length limiter
            Canvas.ForceUpdateCanvases();
            float rectWidth = rectNameArea.rect.width;

            // server 쪽에 이름 반영.
            CmdSetAvatarName(newName);
        }

        [Command(requiresAuthority = false)]
        public void CmdSetAvatarName(string name) 
        {
            _txtName.text = name;
        }

        public void SetAvatarPhoto_Hook(string _, string newPhoto) 
        {
            // avatar 에 사진 반영.
            string url = $"{URL.SERVER_PATH}{newPhoto}"; 
            // GetAvatarPhoto(url).Forget();
            // loader.LoadImage(url).Forget();
            loader.LoadProfile(url, avatarSeq).Forget();

            // server 쪽에 사진 반영.
            CmdSetAvatarPhoto(newPhoto);
        }
        [Command(requiresAuthority = false)]
        public void CmdSetAvatarPhoto(string photo) 
        {
            string url = $"{URL.SERVER_PATH}{photo}";
            loader.LoadImage(url).Forget();
        }

        public void SetAvatarState_Hook(string _, string id) 
        {
            ChangeState(id);

            // server 쪽에 상태 반영.
            CmdSetAvatarState(id);
        }
        [Command(requiresAuthority = false)]
        public void CmdSetAvatarState(string id) 
        {
            ChangeState(id);
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


    #region Avatar State

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

    #endregion  // Avatar State
    }
}