/// <summary>
/// World 에서 사용할 Avatar 움직임 제어 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 03. 07 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성, mirror-test 에서 작업한 항목 migration.
/// </summary>

using UnityEngine;
using Mirror;
using TMPro;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class WorldAvatar : NetworkBehaviour
    {
        private const string TAG = "WorldAvatar";

        [Header("Basic info")]
        [SerializeField] private Sprite _spriteDefault;
        [SerializeField] private bool isMovable; 
        [SerializeField, SyncVar] private float speed = 5f;

        [Header("Diagnostics")]
        private float horizontal;
        private float vertical;
        private Vector3 v3Dir = Vector3.zero;

        [Header("UI")]
        [SerializeField] private TMP_Text _txtName;
        [SerializeField] private SpriteRenderer _rendererProfile;
        [SerializeField] private Transform _transformBubble;

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

        [SyncVar(hook = nameof(SetAvatarChat_Hook))] 
        public string avatarChat;

        // main camera
        [SerializeField, Tooltip("test")]
        private Camera cam;


    #region Unity functions

        private void Awake() 
        {
            isMovable = false;
        }

        private void Start() 
        {
            if (! isOwned) return;

            cam = Camera.main;
            cam.transform.position = transform.position;
            cam.GetComponent<SquareCamera>().UpdateCameraInfo(transform, 4f);

            isMovable = true;
        }

        private void FixedUpdate() 
        {
            Move();
        }

        private void OnDestroy() 
        {
            if (! isOwned) return;

            if (_rendererProfile.sprite.texture != null && string.IsNullOrEmpty(_rendererProfile.sprite.texture.name))
                Destroy(_rendererProfile.sprite.texture);
        }

    #endregion  // Unity functions


    #region Public functions

        public void UpdateAvatarInfo(WorldAvatarInfo info) 
        {
            avatarSeq = info.seq;
            avatarName = info.nickNm;
            avatarPhoto = info.photo;
            avatarMemberType = info.memberType;
        }

        public void UpdateAvatarChat(string chat) 
        {
            if (! string.IsNullOrEmpty(chat))
                avatarChat = chat;
        } 

    #endregion


    #region Private functions

        private void Move() 
        {
            if (! isOwned || ! isMovable) return;
            if (WorldChatView.Instance.OnChat) return;

            // TODO. animation 을 사용하는 경우에 적용 예정.
            // bool isMove = false;

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
            GetAvatarPhoto(url).Forget();

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
                rw = 13f / (float) res.width;
                rh = 13f / (float) res.height * ratio;
            }
            else 
            {
                ratio = (float) res.width / (float) res.height;
                rw = 13f / (float) res.width * ratio;
                rh = 13f / (float) res.height;
            }

            _rendererProfile.transform.localScale = new Vector3(rw, rh, 1f);
            _rendererProfile.sprite = s;
        }

        [Command(requiresAuthority = false)]
        public void CmdSetAvatarPhoto(string photo) 
        {
            string url = $"{URL.SERVER_PATH}{photo}";
            GetAvatarPhoto(url).Forget();
        }

        public void SetAvatarChat_Hook(string _, string chat) 
        {
            WorldChatBubble.Create(_transformBubble, Vector3.zero, chat);
        }

    #endregion  // AVatar info
    }
}