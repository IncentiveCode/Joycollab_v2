/// <summary>
/// [mobile]
/// TOP Navigation Bar
/// @author         : HJ Lee
/// @last update    : 2023. 03. 31
/// @version        : 0.5
/// @update
///     v0.1 (2022. 04. 27) : 최초 생성
///     v0.2 (2023. 03. 16) : 기존 코드 수정. UI 최적화 진행. (UniTask 적용)
///     v0.3 (2023. 03. 22) : FixedView 실험, UI 최적화 (TMP 제거)
///     v0.4 (2023. 03. 23) : Repository observer 실험.
///     v0.5 (2023. 03. 31) : R 에 추가한 alarm count 정보 업데이트 로직 추가.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class TopM : FixedView, iRepositoryObserver
    {
        private const string TAG = "TopM";

        [Header("profile")]
        [SerializeField] private Button _btnMyPage;
        [SerializeField] private Text _txtName;
        [SerializeField] private Text _txtDesc;
        [SerializeField] private RawImage _imgProfile;
        [SerializeField] private Vector2 _v2ProfileSize;
        [SerializeField] private Texture2D _texDefault;

        [Header("alarm")]
        [SerializeField] private Button _btnAlarm;
        [SerializeField] private Image _imgAlarmOn;
        [SerializeField] private Text _txtAlarmCount;

        [Header("channel")]
        [SerializeField] private Button _btnChannel;

        // local variables
        private RectTransform rectProfile;
        private Texture2D texture;


    #region Unity functions

        private void Awake()
        {
            Init();
            base.Reset();

            R.singleton.RegisterObserver(this, eMemberKey);
            R.singleton.RegisterObserver(this, eAlarmKey);
        }

        private void OnDestroy()
        {
            if (_imgProfile.texture != null && string.IsNullOrEmpty(_imgProfile.texture.name))
            {
                Destroy(_imgProfile.texture);
            }

            if (texture != null) Destroy(texture);

            if (R.singleton != null)
            {
                R.singleton.UnregisterObserver(this, eMemberKey);
                R.singleton.UnregisterObserver(this, eAlarmKey);
            }
        }

    #endregion  // Unity functions


    #region FixedView functions

        protected override void Init()
        {
            base.Init();
            viewID = ID.MobileScene_NaviTop;

            // set button listener
            _btnMyPage.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_MyPage));
            _btnAlarm.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_Alarm));
            _btnChannel.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_Channel));

            // set local variables
            rectProfile = _imgProfile.GetComponent<RectTransform>();

            // set event variables
            eMemberKey = eStorageKey.UserInfo;
            mySpaceName = myName = myPhoto = myGrade = string.Empty;

            eAlarmKey = eStorageKey.Alarm;
            myAlarmCount = -1;
        }

    #endregion  // FixedView functions


    #region Event Listener

        private eStorageKey eMemberKey;
        private string mySpaceName;
        private string myName;
        private string myPhoto;
        private string myGrade;

        private eStorageKey eAlarmKey;
        private int myAlarmCount;

        public void UpdateInfo(eStorageKey key)
        {
            if (key == eMemberKey)
            {
                if (!mySpaceName.Equals(R.singleton.mySpaceName) || !myGrade.Equals(R.singleton.myGrade))
                {
                    mySpaceName = R.singleton.mySpaceName;
                    myGrade = R.singleton.myGrade;
                    _txtDesc.text = $"{mySpaceName} | {myGrade}";
                }

                if (!myName.Equals(R.singleton.myName))
                {
                    myName = R.singleton.myName;
                    _txtName.text = myName;
                }

                if (!myPhoto.Equals(R.singleton.myPhoto))
                {
                    myPhoto = R.singleton.myPhoto;

                    string url = $"{URL.SERVER_PATH}{myPhoto}";
                    GetProfileImage(url).Forget();
                }
            }
            else if (key == eAlarmKey)
            {
                if (myAlarmCount != R.singleton.AlarmCount)
                {
                    myAlarmCount = R.singleton.AlarmCount;
                    _txtAlarmCount.text = myAlarmCount >= 99 ? "100+" : $"{myAlarmCount}";
                    _imgAlarmOn.gameObject.SetActive(myAlarmCount != 0);
                }
            }
        }

    #endregion  // Event Listener


    #region other function

        public void ShowNavigation(bool on)
        {
            if (on && visibleState != eVisibleState.Appeared)
            {
                R.singleton.RequestInfo(this, eMemberKey);
                R.singleton.RequestInfo(this, eAlarmKey);
            }

            canvasGroup.alpha = on ? 1 : 0;
            canvasGroup.interactable = on ? true : false;
            canvasGroup.blocksRaycasts = on ? true : false;
            // canvas.enabled = on ? true : false;
            visibleState = on ? eVisibleState.Appeared : eVisibleState.Disappeared;
        }

        private async UniTaskVoid GetProfileImage(string url)
        {
            Texture2D res = await NetworkTask.GetTextureAsync(url);
            if (res == null)
            {
                texture = null;
                _imgProfile.texture = _texDefault;
                return;
            }

            res.hideFlags = HideFlags.HideAndDontSave;
            res.filterMode = FilterMode.Point;
            res.Apply();

            if (_imgProfile.texture != null && string.IsNullOrEmpty(_imgProfile.texture.name))
            {
                Destroy(texture);
                Destroy(_imgProfile.texture);
            }

            // texture = Util.CopyTexture(res);
            // _imgProfile.texture = texture;
            _imgProfile.texture = res;
            Util.ResizeRawImage(rectProfile, _imgProfile, _v2ProfileSize.x, _v2ProfileSize.y);
        }

    #endregion  // other function
    }
}