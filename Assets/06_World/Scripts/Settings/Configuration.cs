/// <summary>
/// [world]
/// 환경설정 _ 환경설정 Script
/// @author         : HJ Lee
/// @last update    : 2023. 09. 15
/// @version        : 0.1
/// @update
///     v0.1 (2023. 09. 15) : 최초 생성
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class Configuration : WindowPage
    {
        private const string TAG = "Configuration";
        private const string GOOGLE_AUTH_CALLBACK = "OnGoogleCallback";
        private const string ZOOM_AUTH_CALLBACK = "OnZoomCallback";

        [Header("module")]
        [SerializeField] private SettingModule _module;

        [Header("language and region")]
        [SerializeField] private Dropdown _dropdownTimezone;
        [SerializeField] private Dropdown _dropdownLanguage;
        [SerializeField] private Dropdown _dropdownFontSize;
        [SerializeField] private Dropdown _dropdownWeekStart;
        [SerializeField] private Dropdown _dropdownTimeFormat;

        [Header("sound")]
        [SerializeField] private Transform _transformSoundContent;

        [Header("notification")]
        [SerializeField] private List<string> _listAlarmKey;

        // - meeting
        [SerializeField] private Toggle _toggleMeetingAll;
        [SerializeField] private Toggle _toggleMeetingCreated;
        [SerializeField] private Toggle _toggleMeetingStarted;
        [SerializeField] private Toggle _toggleMeetingClosed;

        // - voice call
        [SerializeField] private Toggle _toggleVoiceAll;
        [SerializeField] private Toggle _toggleVoiceReceive;
        [SerializeField] private Toggle _toggleVoiceMissed;
        [SerializeField] private Toggle _toggleVoiceRefuse;

        [Header("connection _ google")]
        [SerializeField] private Button _btnGoogleSignIn;
        [SerializeField] private Text _txtGoogleId;
        [SerializeField] private Button _btnGoogleDisconnect;

        [Header("connection _ zoom")]
        [SerializeField] private Button _btnZoomSignIn;
        [SerializeField] private Image _imgZoomSignInDone;
        [SerializeField] private Text _txtZoomId;
        [SerializeField] private Button _btnZoomDisconnect;

        // local variables
        private ReqMemberEnvironmentInfo reqEnvironmentInfo;
        private ReqMemberAlarmInfo reqAlarmInfo;

        private Dictionary<string, alarmOptItemInfo> dictAlarmOptions;
        private Dictionary<string, alarmOptItemInfo> dictAlarmSounds;
        private bool flagParentToggle;
        private bool flagChildToggle;
        

    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();
        }

    #endregion  // Unity functions


    #region WindowPage functions

        protected override void Init() 
        {
            base.Init();


            // set dropdown listener
            // - 없어도 괜찮은 항목들은 주석처리... 추후 필요하면 다시 작업할 것.
            /**
            _dropdownTimezone.onValueChanged.AddListener((index) => {
                Debug.Log($"{TAG} | timezone changed. index : {index}");
            });
             */
            _dropdownLanguage.onValueChanged.AddListener((index) => {
                Debug.Log($"{TAG} | langauge changed. index : {index}");
                R.singleton.ChangeLocale(index);
            });
            _dropdownFontSize.onValueChanged.AddListener((index) => {
                Debug.Log($"{TAG} | font size changed. index : {index}");
                R.singleton.FontSizeOpt = (index + 1);
            });
            /**
            _dropdownWeekStart.onValueChanged.AddListener((index) => {
                Debug.Log($"{TAG} | week start changed. index : {index}");
            });
            _dropdownTimeFormat.onValueChanged.AddListener((index) => {
                Debug.Log($"{TAG} | time format changed. index : {index}");
            });
             */


            // set 'meeting' notification toggle listener
            _toggleMeetingAll.onValueChanged.AddListener(async (isOn) => {
                if (! flagChildToggle) 
                {
                    flagParentToggle = true;
                    await UniTask.Delay(50);

                    _toggleMeetingCreated.isOn = isOn;
                    _toggleMeetingStarted.isOn = isOn;
                    _toggleMeetingClosed.isOn = isOn;

                    await UniTask.Delay(50);
                    flagParentToggle = false;
                }
            });
            _toggleMeetingCreated.onValueChanged.AddListener((isOn) => CheckMeetingEvent().Forget());
            _toggleMeetingStarted.onValueChanged.AddListener((isOn) => CheckMeetingEvent().Forget());
            _toggleMeetingClosed.onValueChanged.AddListener((isOn) => CheckMeetingEvent().Forget());

            // set 'meeting' notification toggle listener
            _toggleVoiceAll.onValueChanged.AddListener(async (isOn) => {
                if (! flagChildToggle) 
                {
                    flagParentToggle = true;
                    await UniTask.Delay(50);

                    _toggleVoiceReceive.isOn = isOn;
                    _toggleVoiceMissed.isOn = isOn;
                    _toggleVoiceRefuse.isOn = isOn;

                    await UniTask.Delay(50);
                    flagParentToggle = false;
                }
            });
            _toggleVoiceReceive.onValueChanged.AddListener((isOn) => CheckVoiceCallEvent().Forget());
            _toggleVoiceMissed.onValueChanged.AddListener((isOn) => CheckVoiceCallEvent().Forget());
            _toggleVoiceRefuse.onValueChanged.AddListener((isOn) => CheckVoiceCallEvent().Forget());


            // set 'google connection'
            _btnGoogleSignIn.onClick.AddListener(() => {
                if (R.singleton.isGoogleConnected) 
                {
                    Debug.Log($"{TAG} | 구글과 연동되어 있는데 connect button 이 출력됨. 확인 요망.");    
                    return;
                }

                Debug.Log($"{TAG} | 구글 연동 시도.");
                string url = string.Format(URL.GOOGLE_AUTH, R.singleton.ID);
                JsLib.OpenAuth(gameObject.name, url, GOOGLE_AUTH_CALLBACK);
            });
            _btnGoogleDisconnect.onClick.AddListener(() => {
                if (! R.singleton.isGoogleConnected) 
                {
                    Debug.Log($"{TAG} | 구글과 연동되어 있지 않은데 disconnect button 이 출력됨. 확인 요망.");    
                    return;
                }

                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "환경설정.구글 연동 해지 확인", R.singleton.CurrentLocale),
                    async () => {
                        string urlDisconnect = string.Format(URL.REVOKE_GOOGLE_CONNECTION, R.singleton.myGoogleId);
                        PsResponse<string> res = await NetworkTask.RequestAsync<string>(urlDisconnect, eMethodType.DELETE, string.Empty, R.singleton.token);
                        if (! string.IsNullOrEmpty(res.message))
                        {
                            PopupBuilder.singleton.OpenAlert(res.message);
                            return;
                        }

                        // TODO. Refresh
                        PopupBuilder.singleton.OpenAlert(
                            LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "환경설정.구글 연동 해지 안내", R.singleton.CurrentLocale),
                            () => { }
                        );
                    }
                );
            });

            // set 'zoom connection'
            _btnZoomSignIn.onClick.AddListener(() => {
                if (R.singleton.isZoomConnected)
                {
                    Debug.Log($"{TAG} | 줌과 연동되어 있는데 connect button 이 출력됨. 확인 요망.");    
                    return;
                }

                Debug.Log($"{TAG} | 줌 연동 시도.");
                string url = string.Format(URL.ZOOM_AUTH, R.singleton.memberSeq);
                JsLib.OpenAuth(gameObject.name, url, ZOOM_AUTH_CALLBACK);
            });
            _btnZoomDisconnect.onClick.AddListener(() => {
                if (! R.singleton.isZoomConnected) 
                {
                    Debug.Log($"{TAG} | 줌과 연동되어 있지 않은데 disconnect button 이 출력됨. 확인 요망.");    
                    return;
                }

                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "환경설정.줌 연동 해지 확인", R.singleton.CurrentLocale),
                    async () => {
                        string urlDisconnect = string.Format(URL.REVOKE_ZOOM_CONNECTION, R.singleton.memberSeq);
                        PsResponse<string> res = await NetworkTask.RequestAsync<string>(urlDisconnect, eMethodType.DELETE, string.Empty, R.singleton.token);
                        if (! string.IsNullOrEmpty(res.message))
                        {
                            PopupBuilder.singleton.OpenAlert(res.message);
                            return;
                        }

                        // TODO. Refresh
                        PopupBuilder.singleton.OpenAlert(
                            LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "환경설정.줌 연동 해지 안내", R.singleton.CurrentLocale),
                            () => { }
                        );
                    }
                );
            });
        }

        public override async UniTaskVoid Show() 
        {
            base.Show().Forget();

            // await Refresh();
            await UniTask.Yield();

            base.Appearing();
        }

    #endregion  // WindowPage functions


    #region event handling

        public async UniTask<int> Refresh() 
        {
            flagParentToggle = flagChildToggle = false;

            var (memberInfoRes, alarmSoundRes) = await UniTask.WhenAll(
                _module.GetMyInfo(),
                _module.GetAlarmContents()
            );

            if (! string.IsNullOrEmpty(memberInfoRes.message))
            {
                PopupBuilder.singleton.OpenAlert(memberInfoRes.message);
                return -2;
            }
            
            if (! string.IsNullOrEmpty(alarmSoundRes.message))
            {
                PopupBuilder.singleton.OpenAlert(alarmSoundRes.message);
                return -2;
            }

            R.singleton.MemberInfo = memberInfoRes.data;
            reqEnvironmentInfo = new ReqMemberEnvironmentInfo(memberInfoRes.data);
            SetEnvironment();

            reqAlarmInfo = new ReqMemberAlarmInfo();
            SetAlarmOption();
            SetAlarmSound(alarmSoundRes.data);

            SetConnections();

            return 0;
        }

        public async UniTask<string> UpdateConfiguration() 
        {
            // 환경 설정 정리
            reqEnvironmentInfo.lanId = R.singleton.Region;
            reqEnvironmentInfo.fontSize = R.singleton.FontSizeOpt;
            reqEnvironmentInfo.weekStart = _dropdownWeekStart.value;
            reqEnvironmentInfo.hourFormatStr = (_dropdownTimeFormat.value == 1) ? "HH" : "hh";

            // 알림 설정 정리
            string alarmBody = OrganizeAlarmOptions();

            var (resEnvironment, resAlarm) = await UniTask.WhenAll(
                _module.UpdateEnvironment(reqEnvironmentInfo),
                _module.UpdateAlarmOptions(alarmBody)
            );

            if (! string.IsNullOrEmpty(resEnvironment))
                return resEnvironment;

            if (! string.IsNullOrEmpty(resAlarm))
                return resAlarm;

            return string.Empty;
        }

    #endregion  // event handling


    #region value check

        private bool IsAllCheckMeetingToggle() 
        {
            return _toggleMeetingCreated.isOn && _toggleMeetingStarted.isOn && _toggleMeetingClosed.isOn;
        }
        private async UniTaskVoid CheckMeetingEvent() 
        {
            if (! flagParentToggle) 
            {
                flagChildToggle = true;
                await UniTask.Delay(50);

                _toggleMeetingAll.isOn = IsAllCheckMeetingToggle();

                await UniTask.Delay(50);
                flagChildToggle = false;
            }
        }

        private bool IsAllCheckVoiceCall()
        {
            return _toggleVoiceReceive.isOn && _toggleVoiceMissed.isOn && _toggleVoiceRefuse.isOn;
        }
        private async UniTaskVoid CheckVoiceCallEvent() 
        {
            if (! flagParentToggle) 
            {
                flagChildToggle = true;
                await UniTask.Delay(50);

                _toggleVoiceAll.isOn = IsAllCheckVoiceCall();

                await UniTask.Delay(50);
                flagChildToggle = false;
            } 
        }

        private void SetEnvironment() 
        {
            _dropdownTimezone.value = 0;
            _dropdownLanguage.value = reqEnvironmentInfo.lanId switch {
                S.REGION_KOREAN => ID.LANGUAGE_KOREAN,
                S.REGION_JAPANESE => ID.LANGUAGE_JAPANESE,
                _ => ID.LANGUAGE_ENGLISH 
            };
            _dropdownFontSize.value = Mathf.Clamp(reqEnvironmentInfo.fontSize - 1, 0, 3);
            _dropdownWeekStart.value = (reqEnvironmentInfo.weekStart == 1) ? 1 : 0;
            _dropdownTimeFormat.value = (reqEnvironmentInfo.hourFormatStr.Equals("HH")) ? 1 : 0;
        }

        private void SetAlarmOption() 
        {
            // current data 정리
            if (dictAlarmOptions == null) dictAlarmOptions = new Dictionary<string, alarmOptItemInfo>();
            dictAlarmOptions.Clear();
            foreach (var item in R.singleton.myAlarmOpt.alarmOptItems) 
            {
                if (_listAlarmKey.Contains(item.tp.id))
                    dictAlarmOptions.Add(item.tp.id, item);
            }

            // alarm option 정리
            foreach (var i in dictAlarmOptions) 
            {
                // Debug.Log($"id : {i.Value.tp.id}, usage : {i.Value.useYn}");
                switch (i.Value.tp.id) 
                {
                    case S.ALARM_ID_RESERVE_MEETING :
                        _toggleMeetingCreated.isOn = i.Value.alarm;
                        break;

                    case S.ALARM_ID_START_MEETING :
                        _toggleMeetingStarted.isOn = i.Value.alarm;
                        break;

                    case S.ALARM_ID_DONE_MEETING :
                        _toggleMeetingClosed.isOn = i.Value.alarm;
                        break;

                    case S.ALARM_ID_REQUEST_VOICE :
                        _toggleVoiceReceive.isOn = i.Value.alarm;
                        break;

                    case S.ALARM_ID_MISSED_VOICE :
                        _toggleVoiceMissed.isOn = i.Value.alarm;
                        break;

                    case S.ALARM_ID_REJECT_VOICE :
                        _toggleVoiceRefuse.isOn = i.Value.alarm;
                        break;
                }
            }
        }

        private void SetAlarmSound(TpsList data) 
        {
            // current data 정리 
            if (dictAlarmSounds == null) dictAlarmSounds = new Dictionary<string, alarmOptItemInfo>();
            dictAlarmSounds.Clear();
            foreach (var i in R.singleton.myAlarmOpt.alarmOptSounds) 
            {
                dictAlarmSounds.Add(i.tp.id, i);
            }

            // alarm list 정리
            var children = _transformSoundContent.GetComponentInChildren<Transform>();
            foreach (Transform child in children) 
            {
                if (child.name.Equals(_transformSoundContent.name)) continue;
                Destroy(child.gameObject);
            }

            foreach (TpsInfo i in data.list) 
            {
                var go = Instantiate(SystemManager.singleton.pfWorldAlarmSoundItem, Vector3.zero, Quaternion.identity);
                if (go.TryGetComponent<AlarmSoundItem>(out AlarmSoundItem item))
                {
                    item.Init(i);
                    item.Usage = dictAlarmSounds.ContainsKey(i.id) ? dictAlarmSounds[i.id].alarm : false;
                    go.transform.SetParent(_transformSoundContent, false);
                }
                else 
                {
                    Destroy(go.gameObject);
                }
            }
        }

        public string OrganizeAlarmOptions() 
        {
            // alarm option 정리
            reqAlarmInfo.alarmOptItems.Clear();
            foreach (var i in dictAlarmOptions) 
            {
                switch (i.Value.tp.id) 
                {
                    case S.ALARM_ID_RESERVE_MEETING :
                        i.Value.alarm = _toggleMeetingCreated.isOn;
                        reqAlarmInfo.alarmOptItems.Add(i.Value);
                        break;

                    case S.ALARM_ID_START_MEETING :
                        i.Value.alarm = _toggleMeetingStarted.isOn;
                        reqAlarmInfo.alarmOptItems.Add(i.Value);
                        break;

                    case S.ALARM_ID_DONE_MEETING :
                        i.Value.alarm = _toggleMeetingClosed.isOn;
                        reqAlarmInfo.alarmOptItems.Add(i.Value);
                        break;

                    case S.ALARM_ID_REQUEST_VOICE :
                        i.Value.alarm = _toggleVoiceReceive.isOn;
                        reqAlarmInfo.alarmOptItems.Add(i.Value);
                        break;

                    case S.ALARM_ID_MISSED_VOICE :
                        i.Value.alarm = _toggleVoiceMissed.isOn;
                        reqAlarmInfo.alarmOptItems.Add(i.Value);
                        break;

                    case S.ALARM_ID_REJECT_VOICE :
                        i.Value.alarm = _toggleVoiceRefuse.isOn;
                        reqAlarmInfo.alarmOptItems.Add(i.Value);
                        break;
                }
            }
                
            // alarm sound 정리
            reqAlarmInfo.alarmOptSounds.Clear();
            var children = _transformSoundContent.GetComponentInChildren<Transform>();
            foreach (Transform child in children) 
            {
                if (child.TryGetComponent<AlarmSoundItem>(out AlarmSoundItem item))
                {
                    Debug.Log($"{TAG} | alarm sound option, id : {item.ID}, usage : {item.Usage}");
                    dictAlarmSounds[item.ID].alarm = item.Usage;
                }
            }
            foreach (var i in dictAlarmSounds) 
            {
                reqAlarmInfo.alarmOptSounds.Add(i.Value);
            }

            return JsonUtility.ToJson(reqAlarmInfo);
        }

        private void SetConnections() 
        {
            // google 연결 확인
            bool isGoogle = R.singleton.isGoogleConnected;
            _txtGoogleId.text = isGoogle ? R.singleton.myGoogleId : string.Empty;
            _btnGoogleSignIn.interactable = ! isGoogle;
            _btnGoogleDisconnect.gameObject.SetActive(isGoogle);

            // zoom 연결 확인
            bool isZoom = R.singleton.isZoomConnected;
            _txtZoomId.text = isZoom ? R.singleton.myZoomId : string.Empty;
            _btnZoomSignIn.gameObject.SetActive(! isZoom);
            _imgZoomSignInDone.gameObject.SetActive(isZoom);
            _btnZoomDisconnect.gameObject.SetActive(isZoom);
        }

    #endregion  // value check


    #region auth callback functions

        public void OnGoogleCallback() 
        {
            Debug.Log($"{TAG} | {GOOGLE_AUTH_CALLBACK} call.");
            Refresh().Forget();
        }

        public void OnZoomCallback() 
        {
            Debug.Log($"{TAG} | {ZOOM_AUTH_CALLBACK} call.");
            Refresh().Forget();
        }

    #endregion  // auth callback functions
    }
}