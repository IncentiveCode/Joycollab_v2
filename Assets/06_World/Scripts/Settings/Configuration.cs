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
        [SerializeField] private GameObject _goSoundContent;

        [Header("notification _ meeting")]
        [SerializeField] private Toggle _toggleMeetingAll;
        [SerializeField] private Toggle _toggleMeetingCreated;
        [SerializeField] private Toggle _toggleMeetingStarted;
        [SerializeField] private Toggle _toggleMeetingClosed;

        [Header("notification _ voice call")]
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
        private ReqMemberEnvironmentInfo environmentInfo;
        private Dictionary<string, bool> dictAlarmSounds;

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

        public async override UniTaskVoid Show() 
        {
            Debug.Log($"{TAG} | Show() call.");
            base.Show().Forget();

            await Refresh();
            base.Appearing();
        }

    #endregion  // WindowPage functions


    #region event handling

        private async UniTask<int> Refresh() 
        {
            flagParentToggle = flagChildToggle = false;

            var (memberInfoRes, alarmSoundRes) = await UniTask.WhenAll(
                _module.GetMyInfo(),
                _module.GetAlarmContents()
            );

            if (! string.IsNullOrEmpty(memberInfoRes.message))
            {
                PopupBuilder.singleton.OpenAlert(memberInfoRes.message);
                return -1;
            }
            
            if (! string.IsNullOrEmpty(alarmSoundRes.message))
            {
                PopupBuilder.singleton.OpenAlert(alarmSoundRes.message);
                return -2;
            }

            R.singleton.MemberInfo = memberInfoRes.data;
            environmentInfo = new ReqMemberEnvironmentInfo(memberInfoRes.data);
            SetEnvironment();

            SetAlarmSound(alarmSoundRes.data);

            return 0;
        }

        private void SetEnvironment() 
        {
            _dropdownTimezone.value = 0;
            _dropdownLanguage.value = environmentInfo.lanId switch {
                S.REGION_KOREAN => ID.LANGUAGE_KOREAN,
                S.REGION_JAPANESE => ID.LANGUAGE_JAPANESE,
                _ => ID.LANGUAGE_ENGLISH 
            };
            _dropdownFontSize.value = Mathf.Clamp(environmentInfo.fontSize - 1, 0, 3);
            _dropdownWeekStart.value = (environmentInfo.weekStart == 1) ? 1 : 0;
            _dropdownTimeFormat.value = (environmentInfo.hourFormatStr.Equals("HH")) ? 1 : 0;
        }

        private void SetAlarmSound(TpsList data) 
        {
            // current data 정리 
            if (dictAlarmSounds == null) dictAlarmSounds = new Dictionary<string, bool>();
            dictAlarmSounds.Clear();
            foreach (var item in R.singleton.myAlarmOpt.alarmOptSounds) 
            {
                dictAlarmSounds.Add(item.tp.id, item.alarm);
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
                    item.Usage = dictAlarmSounds.ContainsKey(i.id) ? dictAlarmSounds[i.id] : false;
                    go.transform.SetParent(_transformSoundContent, false);
                }
                else 
                {
                    Destroy(go.gameObject);
                }
            }
        }

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

        public async UniTask<string> UpdateConfiguration() 
        {
            environmentInfo.lanId = R.singleton.Region;
            environmentInfo.fontSize = R.singleton.FontSizeOpt;
            environmentInfo.weekStart = _dropdownWeekStart.value;
            environmentInfo.hourFormatStr = (_dropdownTimeFormat.value == 1) ? "hh" : "HH";

            string res = await _module.UpdateEnvironment(environmentInfo);
            if (! string.IsNullOrEmpty(res)) 
                return res;

            return string.Empty;
        }

    #endregion  // event handling
    }
}