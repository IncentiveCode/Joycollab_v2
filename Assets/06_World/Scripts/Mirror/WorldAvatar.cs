/// <summary>
/// World 에서 사용할 Avatar 움직임 제어 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 10. 25 
/// @version        : 0.2
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성, mirror-test 에서 작업한 항목 migration.
///     v0.2 (2023. 10. 25) : UI 변경 test
/// </summary>

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;
using TMPro;
using Cysharp.Threading.Tasks;

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
        [SerializeField] private TMP_Text _txtName;
        [SerializeField] private SpriteRenderer _rendererProfile;
        [SerializeField] private Transform _transformBubble;

        [Header("test")]
        [SerializeField] private RawImage _imgProfile;
        [SerializeField] private Image _imgState;
        private ImageLoader loader;

        [Header("Avatar info")]
        internal static WorldAvatarInfo localPlayerInfo;
        [SyncVar]
        public int avatarSeq;
        [SyncVar(hook = nameof(SetAvatarName_Hook))] 
        public string avatarName; 
        [SyncVar(hook = nameof(SetAvatarPhoto_Hook))]
        public string avatarPhoto;
        [SyncVar]
        public string avatarMemberType;
        [SyncVar]
        public string avatarState;

        [SyncVar(hook = nameof(SetAvatarChat_Hook))] 
        public string avatarChat;

        // main camera
        private Camera cam;


    #region Unity functions

        private void Awake() 
        {
            isMovable = false;
            isFly = false;

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
            cam.GetComponent<SquareCamera>().UpdateCameraInfo(transform, 4f);

            isMovable = true;
        }

        private void Update() 
        {
            if (! isOwned) return;

            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                v3Target = cam.ScreenToWorldPoint(Input.mousePosition);
                Debug.Log($"{TAG} | click position : {v3Target}");
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

            /**
            if (_rendererProfile.sprite.texture != null && string.IsNullOrEmpty(_rendererProfile.sprite.texture.name))
                Destroy(_rendererProfile.sprite.texture);
             */
        }

    #endregion  // Unity functions


    #region Public functions

        public void UpdateAvatarInfo(WorldAvatarInfo info) 
        {
            avatarSeq = info.seq;
            avatarName = info.nickNm;
            avatarPhoto = info.photo;
            avatarMemberType = info.memberType;
            avatarState = info.stateId;

            _imgState.color = C.ONLINE;
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
                    Debug.Log($"{TAG} | 마우스 클릭 지점 근처까지 이동 완료.");
                    isFly = false;
                }

                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");
                if (horizontal != 0f || vertical != 0f)
                {
                    Debug.Log($"{TAG} | 마우스 클릭 지점까지 이동하다가 방향키 입력 들어와서 멈춤.");
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
            // Debug.Log($"[player] SetAvatarPhoto_Hook() call. newName : {newPhoto}");

            // avatar 에 사진 반영.
            string url = $"{URL.SERVER_PATH}{newPhoto}"; 
            // GetAvatarPhoto(url).Forget();
            loader.LoadImage(url).Forget();

            // server 쪽에 사진 반영.
            CmdSetAvatarPhoto(newPhoto);
        }

        private async UniTaskVoid GetAvatarPhoto(string url) 
        {
            Texture2D res = await NetworkTask.GetTextureAsync(url);
            if (res == null) 
            {
                Debug.LogError("AvatarMover | 이미지 로딩 실패.");
                return;
            }

            res.hideFlags = HideFlags.HideAndDontSave;
            res.filterMode = FilterMode.Point;
            res.Apply(); 

            Sprite s = Sprite.Create(res, new Rect(0, 0, res.width, res.height), new Vector2(0.5f, 0.5f), 16f, 0, SpriteMeshType.FullRect);
            if (_rendererProfile.sprite.texture != null && string.IsNullOrEmpty(_rendererProfile.sprite.texture.name))
                Destroy(_rendererProfile.sprite.texture);

            // change texture scale
            float ratio, rw, rh;
            if (res.width >= res.height)
            {
                ratio = (float) res.height / (float) res.width;
                rw = 14f / (float) res.width;
                rh = 14f / (float) res.height * ratio;
            }
            else 
            {
                ratio = (float) res.width / (float) res.height;
                rw = 14f / (float) res.width * ratio;
                rh = 14f / (float) res.height;
            }

            _rendererProfile.transform.localScale = new Vector3(rw, rh, 1f);
            _rendererProfile.sprite = s;
        }

        [Command(requiresAuthority = false)]
        public void CmdSetAvatarPhoto(string photo) 
        {
            string url = $"{URL.SERVER_PATH}{photo}";
            // GetAvatarPhoto(url).Forget();
            loader.LoadImage(url).Forget();
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
    }
}