/// <summary>
/// [mobile]
/// TOP Navigation Bar
/// @author         : HJ Lee
/// @last update    : 2023. 03. 22
/// @version        : 0.1
/// @update
///     v0.1 (2022. 04. 27) : 최초 생성
///     v0.2 (2023. 03. 16) : 기존 코드 수정. UI 최적화 진행. (UniTask 적용)
///     v0.3 (2023. 03. 22) : FixedView 실험, UI 최적화 (TMP 제거)
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class NaviTopM : FixedView
    {
        private const string TAG = "NaviTopM";

        [Header("profile")] 
        [SerializeField] private Button _btnMyPage;
        [SerializeField] private Text _txtName;
        [SerializeField] private Text _txtDesc;
        [SerializeField] private RawImage _imgProfile;
        [SerializeField] private Vector2 _v2ProfileSize;
        [SerializeField] private Texture2D _texDefault;
        private RectTransform rectProfile;

        [Header("alarm")] 
        [SerializeField] private Button _btnAlarm;
        [SerializeField] private Image _imgAlarmOn;
        [SerializeField] private Text _txtAlarmCount;

        [Header("channel")]
        [SerializeField] private Button _btnChannel;


    #region Unity functions
        private void Awake() 
        {
            Init();
            Reset();
        }

        private void OnDestroy() 
        {
            if (_imgProfile.texture != null && string.IsNullOrEmpty(_imgProfile.texture.name)) 
                Destroy(_imgProfile.texture);

            // TODO. Mobile Observer 구독 끊기

        }
    #endregion  // Unity functions


    #region FixedView functions
        protected override void Init() 
        {
            base.Init();

            // set button listener
            _btnMyPage.onClick.AddListener(() => Debug.Log("준비 중."));
            _btnAlarm.onClick.AddListener(() => Debug.Log("준비 중"));
            _btnChannel.onClick.AddListener(() => Debug.Log("준비 중"));

            // set local variables
            rectProfile = _imgProfile.GetComponent<RectTransform>();
        }
    #endregion  // FixedView functions


    #region Event Listener 

    #endregion  // Event Listener


    #region other function
        public void ShowNavigation(bool on) 
        {
            canvasGroup.alpha = on ? 1 : 0;
            canvasGroup.interactable = on ? true : false;
            canvas.enabled = on ? true : false;
        }

        private async UniTaskVoid GetProfileImage(string url) 
        {
            Texture2D res = await NetworkTask.GetTextureAsync(url);
            if (res == null) 
            {
                Debug.LogError($"{TAG} | 이미지 로딩 실패.");
                _imgProfile.texture = _texDefault;
                return;
            }

            res.hideFlags = HideFlags.HideAndDontSave;
            res.filterMode = FilterMode.Point;
            res.Apply(); 

            if (_imgProfile.texture != null && string.IsNullOrEmpty(_imgProfile.texture.name)) 
            {
                Destroy(_imgProfile.texture);
            }

            _imgProfile.texture = res;
            Util.ResizeRawImage(rectProfile, _imgProfile, _v2ProfileSize.x, _v2ProfileSize.y);
        }
    #endregion  // other function
    }
}