/// <summary>
/// [mobile]
/// OKR 생성 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 07. 26
/// @version        : 0.2
/// @update
///     v0.1 (2023. 07. 05) : 최초 생성
///     v0.2 (2023. 07. 26) : 기간 설정 예외처리 추가. 제목 또는 상세 input 이 선택되면 스크롤 위치 변경 기능 추가.
///                           Show(string opt) -> Show(int seq) 로 변경.
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class CreateOkrM : FixedView
    {
        private const string TAG = "CreateOkrM";

        [Header("module")]
        [SerializeField] private OkrModule _module;

        [Header("input field")]
        [SerializeField] private TMP_InputField _inputTitle;
        [SerializeField] private Button _btnClearTitle;
        [SerializeField] private TMP_InputField _inputDetail;

        [Header("dropdown")]
        [SerializeField] private TMP_Dropdown _dropdownCategory;
        [SerializeField] private TMP_Dropdown _dropdownShare;
        [SerializeField] private TMP_Dropdown _dropdownObjective;
        [SerializeField] private TMP_Text _txtSelectedObjective;

        [Header("button")]
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnSave;
        [SerializeField] private Button _btnStartDate;
        [SerializeField] private Button _btnDueDate;

        [Header("text")]
        [SerializeField] private TMP_Text _txtStartDate;
        [SerializeField] private TMP_Text _txtDueDate;

        [Header("others")]
        [SerializeField] private Scrollbar _scrollBar;

        // local variables
        private int seq;
        private OkrData data;
        private bool sub;
        private Locale currentLocale; 

        // temp
        private TMP_Text txtTarget;
        private List<TopOkrInfo> listObjective;


    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();

            // add event handling
            MobileEvents.singleton.OnBackButtonProcess += BackButtonProcess;
        }

        #if UNITY_ANDROID
        private void Update() 
        {
            // date picker 처리
            if (AndroidDateCallback.isDateUpdated && AndroidDateCallback.viewID == this.viewID) 
            {
                string result = AndroidDateCallback.SelectedDate.ToString("yyyy-MM-dd");
                if (txtTarget != null)
                {
                    System.DateTime start, end;

                    // 시작일 
                    if (txtTarget == _txtStartDate) 
                    {
                        // 시작일 정리
                        start = System.Convert.ToDateTime(result);
                        System.TimeSpan diff = start - System.DateTime.Now;
                        if (diff.TotalDays < 0) 
                        {
                            string message = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "날짜 선택.OKR 시작일은 오늘보다 빠를 수 없음", currentLocale);
                            PopupBuilder.singleton.OpenAlert(message);

                            txtTarget.text = System.DateTime.Now.ToString("yyyy-MM-dd");    
                        }
                        else 
                        {
                            txtTarget.text = result;
                        }

                        // 마감일 정리
                        end = start.AddMonths(3).AddDays(-1);
                        _txtDueDate.text = end.ToString("yyyy-MM-dd");
                    }
                    // 마감일, 시작보다 빠르게 설정되면 기존 룰대로 3개월을 설정.
                    else if (txtTarget == _txtDueDate) 
                    {
                        start = System.Convert.ToDateTime(_txtStartDate.text);
                        System.TimeSpan diff = AndroidDateCallback.SelectedDate - start;
                        if (diff.TotalDays < 0) 
                        {
                            string message = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "날짜 선택.마감일은 시작일보다 빠를 수 없음", currentLocale);
                            PopupBuilder.singleton.OpenAlert(message);

                            end = start.AddMonths(3).AddDays(-1);
                            _txtDueDate.text = end.ToString("yyyy-MM-dd");
                        }
                        else 
                        {
                            txtTarget.text = result;
                        }
                    }
                }
                AndroidDateCallback.isDateUpdated = false;
            }
        }
        #endif

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
            viewID = ID.MobileScene_CreateOkr;


            // set input field listener
            SetInputFieldListener(_inputTitle);
            _inputTitle.onValueChanged.AddListener((value) => {
                _btnClearTitle.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _btnClearTitle.onClick.AddListener(() => {
                _inputTitle.text = string.Empty;
            });
            _inputTitle.onSelect.AddListener((value) => _scrollBar.value = 1);

            SetInputFieldListener(_inputDetail);
            _inputDetail.onSelect.AddListener((value) => _scrollBar.value = 0);


            // set dropdown listener
            _dropdownCategory.onValueChanged.AddListener((value) => {
                Debug.Log($"{TAG} | category changed. selected value : {value}"); 

                _dropdownObjective.gameObject.SetActive(value == 1);
                if (value == 1)
                    GetObjectives(_dropdownShare.value + 1).Forget();
            });
            _dropdownShare.onValueChanged.AddListener((value) => {
                if (_dropdownCategory.value == 1)
                {
                    Debug.Log($"{TAG} | share option changed. selected value : {value}"); 
                    GetObjectives(value + 1).Forget();
                }
            });
            _dropdownObjective.onValueChanged.AddListener((value) => {
                Debug.Log($"{TAG} | objective option changed. selected value : {value}"); 

                if (listObjective.Count == 0) return;

                _txtStartDate.text = listObjective[value].sd;
                _txtDueDate.text = listObjective[value].ed;
            });


            // set button listener
            _btnBack.onClick.AddListener(() => BackProcess());
            _btnSave.onClick.AddListener(() => SaveOkr().Forget());
            _btnStartDate.onClick.AddListener(() => {
                txtTarget = _txtStartDate;
                AndroidLib.singleton.ShowDatepicker(viewID, _txtStartDate.text);
            });
            _btnDueDate.onClick.AddListener(() => {
                txtTarget = _txtDueDate;
                AndroidLib.singleton.ShowDatepicker(viewID, _txtDueDate.text);
            });


            // init local variables
            data = null;
            seq = -1;
            sub = false;
            listObjective = new List<TopOkrInfo>();
            listObjective.Clear();
        }

        /// <summary>
        /// 새 OKR 등록.
        /// </summary>
        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            data = null;
            await Refresh();
            base.Appearing();
        }

        /// <summary>
        /// 기존 OKR 수정.
        /// </summary>
        public async override UniTaskVoid Show(int seq) 
        {
            base.Show().Forget();

            this.seq = seq;
            data = Tmp.singleton.GetOkrInfo(seq);
            if (data == null)
            {
                PopupBuilder.singleton.OpenAlert("오류가 발생했습니다.", () => BackProcess());
            }

            await Refresh();
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region okr handling

        private async UniTaskVoid SaveOkr() 
        {
            ReqOkrInfo req = new ReqOkrInfo();
            req.content = _inputDetail.text;
            req.createMember = new Seq() { seq = R.singleton.memberSeq };
            req.ed = _txtDueDate.text;
            req.modifyMember = new Seq() { seq = R.singleton.memberSeq };
            req.sd = _txtStartDate.text;
            req.title = _inputTitle.text;

            PsResponse<string> res;
            string body = JsonUtility.ToJson(req);
            if (this.seq == -1) 
            {
                if (_dropdownCategory.value == 0)
                {
                    res = await _module.SaveObjective(body, _dropdownShare.value + 1);
                }
                else  
                {
                    if (listObjective.Count == 0)
                    {
                        Locale currentLocale = LocalizationSettings.SelectedLocale;
                        string message = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "Objective 선택 안내", currentLocale);
                        PopupBuilder.singleton.OpenAlert(message);
                        return;
                    }

                    int objectiveSeq = listObjective[_dropdownObjective.value].seq;
                    res = await _module.SaveKeyResult(body, objectiveSeq);
                }
            }
            else 
            {
                res = await _module.UpdateOkr(this.seq, JsonUtility.ToJson(req));
            }

            if (string.IsNullOrEmpty(res.message)) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string message = (this.seq == -1) ? 
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "등록 안내", currentLocale) :
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "수정 안내", currentLocale);

                PopupBuilder.singleton.OpenAlert(message, () => {
                    if (this.seq == -1)
                    {
                        ViewManager.singleton.Pop(true);
                    }
                    else
                    {
                        if (sub) 
                        {
                            data.subInfo.title = req.title;
                            data.subInfo.content = req.content;
                            data.subInfo.sd = req.sd;
                            data.subInfo.ed = req.ed;
                        }
                        else 
                        {
                            data.info.title = req.title;
                            data.info.content = req.content;
                            data.info.sd = req.sd;
                            data.info.ed = req.ed;
                        }

                        Tmp.singleton.AddOkrInfo(this.seq, data);
                        Tmp.singleton.AddSearchOkr(this.seq, data);

                        ViewManager.singleton.Pop(this.seq);
                    }
                });
            }
            else 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
            }
        }

        private async UniTaskVoid GetObjectives(int shareOpt) 
        {
            PsResponse<TopOkrList> res = await _module.GetObjectives(shareOpt);
            if (string.IsNullOrEmpty(res.message)) 
            {
                listObjective.Clear();
                _dropdownObjective.options.Clear();
                
                foreach (var info in res.data.list) 
                {
                    listObjective.Add(info);
                    _dropdownObjective.options.Add(new TMP_Dropdown.OptionData() {text = info.title});
                }

                if (data == null) 
                {
                    if (listObjective.Count >= 1)
                    {
                        _txtStartDate.text = listObjective[0].sd;
                        _txtDueDate.text = listObjective[0].ed;
                        _dropdownObjective.value = 0;
                    }

                    _dropdownObjective.RefreshShownValue();
                }
                else 
                {
                    _txtSelectedObjective.text = data.objective;
                }
            }
            else 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
            }
        }

    #endregion okr handling


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(false);
            _scrollBar.value = 1;

            _dropdownCategory.interactable = (data == null);
            _dropdownShare.interactable = (data == null);
            _dropdownObjective.interactable = (data == null);

            // ready for new content
            if (data == null) 
            {
                // set input field value
                _inputTitle.text = _inputDetail.text = string.Empty;

                // calculate hour
                System.DateTime now = System.DateTime.Now;
                int tempMinute;
                tempMinute = (now.Minute <= 30) ? (30 - now.Minute) : (60 - now.Minute);
                now = now.AddMinutes(tempMinute);

                // set text value
                _txtStartDate.text = now.ToString("yyyy-MM-dd");
                now = now.AddMonths(3).AddDays(-1);
                _txtDueDate.text = now.ToString("yyyy-MM-dd");

                // set dropdown value
                _dropdownCategory.value = _dropdownShare.value = 0;
                _dropdownObjective.gameObject.SetActive(false);
            }
            // refresh for content detail
            else 
            {
                sub = (! this.data.isShare && this.data.isKeyResult);

                // set input field value
                _inputTitle.text = sub ? data.subInfo.title : data.info.title;
                _inputDetail.text = sub ? data.subInfo.content : data.info.content;

                // set text value
                _txtStartDate.text = sub ? data.subInfo.sd : data.info.sd;
                _txtDueDate.text = sub ? data.subInfo.ed : data.info.ed;

                // set dropdown value;
                _dropdownCategory.value = data.isKeyResult ? 1 : 0;
                int shareOpt = data.shareType;
                _dropdownShare.value = shareOpt - 1;

                // get objective list
                if (this.data.isKeyResult) GetObjectives(shareOpt).Forget();    
                _dropdownObjective.gameObject.SetActive(this.data.isKeyResult);
            }

            // button visible
            _btnClearTitle.gameObject.SetActive(! string.IsNullOrEmpty(_inputTitle.text));

            await UniTask.Yield();
            return 0;
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
            bool isChanged = false;
            if (data == null) 
            {
                isChanged = ! string.IsNullOrEmpty(_inputTitle.text) || ! string.IsNullOrEmpty(_inputDetail.text);
            }
            else 
            {
                if (sub) 
                {
                    isChanged = 
                        (data.subInfo.title != _inputTitle.text) ||
                        (data.subInfo.sd != _txtStartDate.text) || 
                        (data.subInfo.ed != _txtDueDate.text) ||
                        (data.subInfo.content != _inputDetail.text);
                }
                else 
                {
                    isChanged = 
                        (data.info.title != _inputTitle.text) ||
                        (data.info.sd != _txtStartDate.text) || 
                        (data.info.ed != _txtDueDate.text) ||
                        (data.info.content != _inputDetail.text);
                }
            }

            if (isChanged) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string message = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "작성 취소 확인", currentLocale);
                PopupBuilder.singleton.OpenConfirm(message, () => {
                    if (data == null)   ViewManager.singleton.Pop(false);
                    else                ViewManager.singleton.Pop(this.seq);
                });
            }
            else 
            {
                if (data == null)   ViewManager.singleton.Pop(false);
                else                ViewManager.singleton.Pop(this.seq);
            }
        }

    #endregion  // event handling
    }
}