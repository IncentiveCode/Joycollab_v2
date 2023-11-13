/// <summary>
/// [mobile]
/// OKR 상세 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 07. 26
/// @version        : 0.2
/// @update
///     v0.1 (2023. 07. 05) : 최초 생성.
///     v0.2 (2023. 07. 26) : Show() 매개변수 변경  [string 형 opt -> int 형 seq], period 에 시간값도 출력.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class OkrDetailM : FixedView
    {
        private const string TAG = "OkrDetailM";
        
        [Header("module")]
        [SerializeField] private OkrModule _module;

        [Header("button")]
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnEdit;
        [SerializeField] private Button _btnDelete;

        [Header("sprite")]
        [SerializeField] private Image _imgIcon;
        [SerializeField] private Sprite _sprObjective;
        [SerializeField] private Sprite _sprKeyResult;

        [Header("text")]
        [SerializeField] private TMP_Text _txtTitle;
        [SerializeField] private TMP_Text _txtCreator;
        [SerializeField] private TMP_Text _txtPeriod;
        [SerializeField] private TMP_Text _txtShareOpt;
        [SerializeField] private TMP_Text _txtDetail;

        // local variables
        private OkrData data;
        private int seq;
        private bool isKeyResult;
        private bool sub;


    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();

            // add event handling
            MobileEvents.singleton.OnBackButtonProcess += BackButtonProcess;
        }

        private void OnDestroy() 
        {
            if (MobileEvents.singleton != null) 
            {
                MobileEvents.singleton.OnBackButtonProcess -= BackButtonProcess;
            }
        }

    #endregion  // Unity functions


    #region FixedView functions

        protected override void Init() 
        {
            base.Init();
            viewID = ID.MobileScene_OkrDetail;


            // set 'button' listener
            _btnBack.onClick.AddListener(() => BackProcess());
            _btnEdit.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_CreateOkr, seq));
            _btnDelete.onClick.AddListener(() => {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string message = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "삭제 확인", currentLocale);

                PopupBuilder.singleton.OpenConfirm(message, () => DeleteInfo().Forget());
            });


            // init local variables
            data = null;
            seq = 0;
            isKeyResult = sub = false;
        }

        public override async UniTaskVoid Show(int seq) 
        {
            base.Show().Forget();

            data = Tmp.singleton.GetOkrInfo(seq);
            if (data == null) data = Tmp.singleton.GetSearchOkr(seq);
            if (data == null) 
            {
                PopupBuilder.singleton.OpenAlert("오류가 발생했습니다.", () => BackProcess());
            }

            await Refresh();
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(false);

            // sub task check
            sub = (! this.data.isShare && this.data.isKeyResult);

            // display info 
            this.seq = data.seq;
            this.isKeyResult = data.isKeyResult;

            // icon 처리
            _imgIcon.sprite = isKeyResult ? _sprKeyResult : _sprObjective;

            // text 처리
            _txtTitle.text = sub ? data.subInfo.title : data.info.title;
            _txtCreator.text = sub ? data.subInfo.createMember.nickNm : data.info.createMember.nickNm;
            _txtPeriod.text = sub ? string.Format("{0} - {1}", data.subInfo.sd, data.subInfo.ed) :
                                    string.Format("{0} - {1}", data.info.sd, data.info.ed);
            _txtDetail.text = sub ? data.subInfo.content : data.info.content;

            // 공유 옵션
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            switch (data.shareType - 1) 
            {
                case S.SHARE_DEPARTMENT :
                    bool isTop = string.IsNullOrEmpty(data.info.topSpace.nm);
                    _txtShareOpt.text = isTop ? data.info.topSpace.nm : data.info.space.nm;
                    break;

                case S.SHARE_COMPANY :
                    _txtShareOpt.text = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "전사", currentLocale);
                    break;

                case S.SHARE_NONE :
                default :
                    _txtShareOpt.text = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "없음", currentLocale);
                    break;
            }

            // 내 정보인 경우에만 버튼 출력
            bool isMyInfo = R.singleton.memberSeq == (sub ? data.subInfo.createMember.seq : data.info.createMember.seq);
            _btnEdit.gameObject.SetActive(isMyInfo);
            _btnDelete.gameObject.SetActive(isMyInfo);

            await UniTask.Yield();
            return 0;
        }

        private async UniTaskVoid DeleteInfo()
        {
            PsResponse<string> res = await _module.DeleteOkr(this.seq);
            if (string.IsNullOrEmpty(res.message)) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string message = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "삭제 안내", currentLocale);
                PopupBuilder.singleton.OpenAlert(message, () => {
                    ViewManager.singleton.Pop(true);
                });
            }
            else 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
            }
        }

        private void BackButtonProcess(string name="") 
        {
            if (! name.Equals(gameObject.name)) return; 
            if (visibleState != eVisibleState.Appeared) return;

            if (PopupBuilder.singleton.GetPopupCount() > 0)
            {
                PopupBuilder.singleton.RequestClear();
            }
            else 
            {
                BackProcess();
            }
        }

        private void BackProcess() 
        {
            ViewManager.singleton.Pop();
        }

    #endregion  // event handling
    }
}