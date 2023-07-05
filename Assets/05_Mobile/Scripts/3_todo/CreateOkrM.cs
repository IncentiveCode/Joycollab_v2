/// <summary>
/// [mobile]
/// OKR 생성 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 07. 05
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 05) : 최초 생성
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using Gpm.Ui;
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

        [Header("button")]
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnSave;
        [SerializeField] private Button _btnStartDate;
        [SerializeField] private Button _btnDueDate;

        [Header("text")]
        [SerializeField] private TMP_Text _txtStartDate;
        [SerializeField] private TMP_Text _txtDueDate;

        // local variables
        private int seq;
        private OkrData data;
        private bool isKeyResult;
        private bool sub;

        // temp
        private TMP_Text txtTarget;


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
                    txtTarget.text = result;
                    if (txtTarget == _txtStartDate)
                    {
                        System.DateTime date = AndroidDateCallback.SelectedDate.AddMonths(3).AddDays(-1);
                        _txtDueDate.text = date.ToString("yyyy-MM-dd");
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
            _inputTitle.onSubmit.AddListener((value) => Debug.Log($"{TAG} | search, {value}"));
            SetInputFieldListener(_inputDetail);


            // set dropdown listener
            _dropdownCategory.onValueChanged.AddListener((value) => {
                Debug.Log($"{TAG} | category changed. selected value : {value}"); 

                // TODO. objective list 가지고 오기
                _dropdownObjective.gameObject.SetActive(value == 1);
            });
            _dropdownShare.onValueChanged.AddListener((value) => {
                Debug.Log($"{TAG} | share option changed. selected value : {value}"); 
            });
            _dropdownObjective.onValueChanged.AddListener((value) => {
                Debug.Log($"{TAG} | objective option changed. selected value : {value}"); 
            });


            // set button listener
            _btnBack.onClick.AddListener(() => BackProcess());
            _btnSave.onClick.AddListener(() => SaveOkr().Forget());
            _btnStartDate.onClick.AddListener(() => {
                txtTarget = _txtStartDate;
                AndroidLib.singleton.ShowDatepicker(viewID);
            });
            _btnDueDate.onClick.AddListener(() => {
                txtTarget = _txtDueDate;
                AndroidLib.singleton.ShowDatepicker(viewID);
            });


            // init local variables
            data = null;
            seq = 0;
            isKeyResult = sub = false;
        }

        /// <summary>
        /// 새 할 일 등록.
        /// </summary>
        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            data = null;
            await Refresh();
            base.Appearing();
        }

        /// <summary>
        /// 기존 할 일 수정.
        /// </summary>
        public async override UniTaskVoid Show(string opt) 
        {
            base.Show().Forget();

            int temp = -1;
            int.TryParse(opt, out temp);
            if (temp == -1)
            {
                // TODO. 예외처리
            }

            seq = temp;
            data = R.singleton.GetOkrInfo(seq);
            if (data == null)
            {
                // TODO. 예외처리
            }

            await Refresh();
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region to-do handling

        private async UniTaskVoid SaveOkr() 
        {
            if (_dropdownShare.value == 0) 
            {
                // TODO. 공유 옵션 설정 안내
                return;
            }

            ReqOkrInfo req = new ReqOkrInfo();
            req.content = _inputDetail.text;
            req.createMember = new Seq() { seq = R.singleton.memberSeq };
            req.ed = _txtDueDate.text;
            req.modifyMember = new Seq() { seq = R.singleton.memberSeq };
            req.sd = _txtStartDate.text;
            req.title = _inputTitle.text;

            PsResponse<string> res;
            string body = JsonUtility.ToJson(req);
            int objectiveSeq = 0;
            if (this.seq == -1) 
            {
                if (_dropdownCategory.value == 0)
                    res = await _module.SaveObjective(body, _dropdownShare.value);
                else  
                    res = await _module.SaveKeyResult(body, objectiveSeq);
            }
            else 
            {
                res = await _module.UpdateOkr(this.seq, JsonUtility.ToJson(req));
            }

            if (string.IsNullOrEmpty(res.message)) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string message = (this.seq == -1) ? 
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "등록 완료", currentLocale) :
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "수정 완료", currentLocale);

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

                        R.singleton.AddOkrInfo(this.seq, data);

                        ViewManager.singleton.Pop(this.seq.ToString());
                    }
                });
            }
            else 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
            }
        }

    #endregion to-do handling


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(false);

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
                _dropdownCategory.value = _dropdownShare.value = _dropdownObjective.value = 0;
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
                _dropdownShare.value = shareOpt;
                Debug.Log($"{TAG} | share option : {_dropdownShare.value}");
                Debug.Log($"{TAG} | data value : {shareOpt}");

                // TODO. objective list 가져오기

            }

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
                    if (data == null)   ViewManager.singleton.Pop();
                    else                ViewManager.singleton.Pop(this.seq.ToString());
                });
            }
            else 
            {
                if (data == null)   ViewManager.singleton.Pop();
                else                ViewManager.singleton.Pop(this.seq.ToString());
            }
        }

    #endregion  // event handling
    }
}