/// <summary>
/// Joycollab 통합 매니저 클래스 
/// - singleton 남용을 막고, 기존 manager 클래스들에서 중복되어 있는 내용들을 수정/정리/최적화 하기 위해 작성.
/// @author         : HJ Lee
/// @last update    : 2023. 10. 11
/// @version        : 0.9
/// @update
///     v0.1 (2023. 04. 07) : 최초 작성.
///     v0.2 (2023. 04. 19) : singleton pattern 수정
///     v0.3 (2023. 08. 01) : language, text 관련 초기화 추가
///     v0.4 (2023. 08. 10) : 공지사항 확인, URL parsing 기능 추가. (v1 에서 사용하던 항목들)
///     v0.5 (2023. 08. 16) : 일본어 적용 (진행 중)
///     v0.6 (2023. 08. 23) : Window - OnFocus, OnBlur, OnResize 처리 함수 추가
///     v0.7 (2023. 08. 28) : Localization 사용 방식 변경.
///     v0.8 (2023. 09. 21) : AudioSource 추가. AudioClip 관련 함수 추가.
///     v0.9 (2023. 10. 11) : Timezone 관련 기능 추가.
/// </summary>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using WebGLSupport;

namespace Joycollab.v2
{
    public class SystemManager : MonoBehaviour
    {
        private const string TAG = "SystemManager";

        public static SystemManager singleton { get; private set; }

        [Header("guide popup")]
        [SerializeField] private Transform _transform;
        [SerializeField] private GameObject _goUpdateGuide;

        [Header("world assets")]
        [SerializeField] public Transform pfChatBubble;
        [SerializeField] public GameObject pfWorldAlarmSoundItem;
        [SerializeField] public GameObject pfBuildingInfo;

        // xmpp manager
        private XmppManager xmpp;
        public XmppManager XMPP => xmpp;

        // audio
        private AudioSource audioSource;

        // timezone
        private TimezoneList timezoneList;
        private TimezoneItem timezoneSeoul;


    #region Unity functions

        private void Awake() 
        {
            // test only
            PlayerPrefs.DeleteAll();

            InitSingleton();
            SetTransform();

            // get components
            audioSource = GetComponent<AudioSource>();

            // set local variables
            timezoneList = new TimezoneList();

            // manager initialize
            xmpp = GetComponent<XmppManager>();
        }

        private async UniTaskVoid Start() 
        {
        #if UNITY_WEBGL && !UNITY_EDITOR 
            Application.targetFrameRate = -1;
        #else
            Application.targetFrameRate = 30;
        #endif

            xmpp.Init();

            var (repoRes, timezoneRes) = await UniTask.WhenAll(
                R.singleton.Init(),
                InitTimezone()
            );

            if (repoRes == false) 
            {
                Debug.Log($"{TAG} | Start(), R class init fail.");
            }

            if (! string.IsNullOrEmpty(timezoneRes)) 
            {
                PopupBuilder.singleton.OpenAlert(timezoneRes);
                return;
            }

            GetSystemNotice().Forget();
        }

        private void OnEnable() 
        {
            WebGLWindow.OnFocusEvent += OnFocus;
            WebGLWindow.OnBlurEvent += OnBlur;
            WebGLWindow.OnResizeEvent += OnResize;
        }

        private void OnDisable() 
        {
            WebGLWindow.OnFocusEvent -= OnFocus;
            WebGLWindow.OnBlurEvent -= OnBlur;
            WebGLWindow.OnResizeEvent -= OnResize;
        }

    #endregion  // Unity functions


    #region Initialize

        private void InitSingleton() 
        {
            if (singleton != null && singleton == this) return;
            if (singleton != null) 
            {
                Destroy(gameObject);
                return;
            }

            singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        private void SetTransform() 
        {
        #if UNITY_ANDROID || UNITY_IOS
            _transform = GameObject.Find(S.Canvas_Popup_M).GetComponent<Transform>();
        #else
            _transform = GameObject.Find(S.Canvas_Popup).GetComponent<Transform>();
        #endif
        }

        private async UniTask<string> InitTimezone() 
        {
            if (timezoneList.list.Count > 0) 
            {
                Debug.Log($"{TAG} | InitTimezone(), 이미 초기화가 되어 있는 상태.");
                timezoneList.list.Clear();
            }

            await UniTask.Yield();
            return string.Empty;

            // TODO. api 추가되면 반영할 것.
            /** 
            PsResponse<TimezoneList> res = await NetworkTask.RequestAsync<TimezoneList>(URL.TIMEZONE_LIST, eMethodType.GET);
            if (res.message.Equals(string.Empty)) 
            {
                timezoneList = res.data;                 
                foreach (var ti in timezoneList.list) 
                {
                    if (ti.id.Equals("Asia/Seoul"))
                    {
                        timezoneSeoul = ti;
                        break;
                    }
                } 
            }
            else 
            {
                Debug.Log($"{TAG} | InitTimezone(), code : {res.code}, error : {res.message}");
            }

            return res.message;
             */
        }

    #endregion  // Initialize


    #region First Act (공지사항 확인 후 URL parsing)

        private async UniTaskVoid GetSystemNotice() 
        {
            PsResponse<UpdateNoticeList> res = await NetworkTask.RequestAsync<UpdateNoticeList>(URL.SYSTEM_NOTICE_PATH, eMethodType.GET);
            if (res.message.Equals(string.Empty)) 
            {
                if (res.data.list.Count >= 1) 
                {
                    string checkNode = JsLib.GetCookie(Key.SYSTEM_UPDATE_FLAG);
                    if (checkNode.Equals(S.TRUE))
                    {
                        JsLib.CheckBrowser(gameObject.name, "PostCheckBrowser");
                    }
                    else 
                    {
                        string title = res.data.list[0].title;
                        string message = string.Format("{0}\n\n점검시간 : {1} ~ {2}", 
                            res.data.list[0].content,
                            res.data.list[0].sdtm, 
                            res.data.list[0].edtm
                        );
                    
                        ShowSystemUpdate(title, message);
                    }
                }
                else 
                {
                    JsLib.SetCookie(Key.SYSTEM_UPDATE_FLAG, S.FALSE);
                    JsLib.CheckBrowser(gameObject.name, "PostCheckBrowser");
                }
            }
            else 
            {
                Debug.Log($"{TAG} | GetSystemNotice(), code : {res.code}, error : {res.message}");
            }
        }

        private void ShowSystemUpdate(string title, string content) 
        {
            var popup = Instantiate(_goUpdateGuide, Vector3.zero, Quaternion.identity);
            SystemUpdateGuide script = popup.GetComponent<SystemUpdateGuide>();
            script.Init(title, content); 
            popup.transform.SetParent(_transform, false);            
        }

        public void PostCheckBrowser(string result) 
        {
            Debug.Log($"{TAG} | Check browser result : {result}");
            string[] arrResult = result.Split('|');
            string browserType = arrResult[0];
            string defaultLanguage = arrResult[1];

            Debug.Log($"{TAG} | browser language : {defaultLanguage}");
            switch (defaultLanguage) 
            {
                case S.REGION_KOREAN :
                    R.singleton.ChangeLocale(ID.LANGUAGE_KOREAN);
                    break;

                case S.REGION_JAPANESE :
                    R.singleton.ChangeLocale(ID.LANGUAGE_JAPANESE);
                    break;

                default :
                    R.singleton.ChangeLocale(ID.LANGUAGE_ENGLISH);
                    break;
            }
            
            if (browserType.Contains(S.TRUE) || browserType.Equals(S.EDITOR)) 
            {
                CheckUrlParams();
            }
            else 
            {
                string content = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "브라우저 안내", R.singleton.CurrentLocale);
                PopupBuilder.singleton.OpenAlert(
                    content,
                    () => CheckUrlParams()
                );
            }
        }

        private void CheckUrlParams() 
        {
            R.singleton.ClearParamValues();

            string testURL = string.Empty;
            // testURL = URL.INDEX;
            testURL = URL.WORLD_INDEX;

            // font size 조절.
            SetFontOpt(1);

            string absURL = Application.isEditor ? testURL : Application.absoluteURL; 
            if (string.IsNullOrEmpty(absURL)) absURL = URL.INDEX;
            string nextScene = ParseUrl(absURL);
            SceneLoader.Load(nextScene.Contains(S.WORLD) ? eScenes.World : eScenes.SignIn);
        }

    #endregion  // First Act (공지사항 확인 후 URL parsing)


    #region URL parsing 

        private string ParseUrl(string url) 
        {
            string domain, scene, param;
            domain = scene = param = string.Empty;

            // step 1. URL split
            var arr = url.Split('/');
            switch (arr.Length) 
            {
                case 5 :    // sub domain 없이 입장한 경우
                    scene = arr[3];
                    param = arr[4];
                    break;

                case 6 :    // sub domain 과 함께 입장한 경우
                    domain = arr[3];
                    scene = arr[4];
                    param = arr[5];
                    break;
                
                default :   
                    // 기타 케이스, WebGL 이라면 주소를 바꿔서 다시 접속하도록 유도한다.
                    Debug.Log($"{TAG} | ParseUrl(), 잘못된 주소 : {url}");
                    break;
            }
            Debug.Log($"{TAG} | Url split result - sub domain : {domain}, scene : {scene}, param : {param}");
            R.singleton.AddParam(Key.DOMAIN, domain);


            // step 2. parameter check
            bool isJoin = param.Contains(Key.JOIN);
            int index = param.Contains("?") ? param.IndexOf("?") : -1;
            string[] arrParam = (index == -1) ? new string[]{ } : param.Substring(index + 1).Split('&');
            foreach (string s in arrParam)
            {
                string[] pair = s.Split('=');
                R.singleton.AddParam(pair[0], pair[1]);
            }

            return scene;
        }

    #endregion  // URL parsing 


    #region focus callback

        private void OnFocus() 
        {
            Debug.Log($"{TAG} | OnFocus()");
        }

        private void OnBlur() 
        {
            Debug.Log($"{TAG} | OnBlur()");
        }

        private void OnResize() 
        {
            Debug.Log($"{TAG} | OnResize(), width : {Screen.width}, height : {Screen.height}");
        }

    #endregion  // focus callback


    #region Temp

        public void SetFontOpt(int opt) 
        {
            R.singleton.FontSizeOpt = opt;
        }

        public void Exit() 
        {
            SetFontOpt(1);
            SceneLoader.Load(eScenes.SignIn);
        }

    #endregion  // Temp


    #region AudioSource

        public void PlayAudioClip(AudioClip clip) 
        {
            if (audioSource == null) 
            {
                Debug.Log($"{TAG} | PlayAudioClip(), AudioSource is null");
                return;
            }

            audioSource.clip = clip;
            audioSource.Play();
        }

        public void StopAudioClip() 
        {
            if (audioSource == null) 
            {
                Debug.Log($"{TAG} | StopAudioClip(), AudioSource is null");
                return;
            }

            audioSource.Stop();
            audioSource.clip = null;
        }

    #endregion AudioSource


    #region Timezone 

		public DateTime GetSeoulTime() 
		{
			DateTime now = DateTime.Now;
			long tick = Util.Datetime2Utc(now);

			return Util.Utc2CurrentZoneTime(tick, timezoneSeoul.utcOffset);
		}

		public List<TimezoneItem> TimeZones 
		{
			get {
                return (timezoneList.list.Count == 0) ? null : timezoneList.list;
			}
		}

		public TimezoneItem GetTimezone(int index) 
		{
			if (index < 0) return null;
            return (timezoneList.list.Count == 0) ? null : timezoneList.list[index];
		}

		public int GetTimezoneIndex(string key) 
		{
			if (string.IsNullOrEmpty(key)) return -1;

			int index = -1;
            TimezoneItem ti = null;
            for (int i = 0; i < timezoneList.list.Count; i++) 
            {
                ti = timezoneList.list[i];
                if (ti.id.Equals(key)) 
                {
                    index = i; 
                    break;
                }
            }

			return index;
		}            

    #endregion  // Timezone 


    #region communication 

        public async UniTaskVoid CallOnTheSpot(List<int> seqs) 
        {
            string url = string.Format(URL.MAKE_CALL, R.singleton.workspaceSeq);

            ReqCallInfo info = new ReqCallInfo();
            foreach (int seq in seqs) 
            {
                info.members.Add(new Seq(seq));
            }

            string body = JsonUtility.ToJson(info);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.POST, body, R.singleton.token, true);
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return;
            }

            if (string.IsNullOrEmpty(res.stringData)) 
            {
                PopupBuilder.singleton.OpenAlert("결과값이 비어있음.");
                return;
            }

            string[] split = res.stringData.Split('/');
            int.TryParse(split[split.Length - 1], out int callSeq);
            string callLink = string.Format(URL.CALL_LINK, R.singleton.workspaceSeq, callSeq, R.singleton.memberSeq, R.singleton.Region);
            JsLib.OpenVoiceCall(this.name, callLink, "StopAudioClip");
        }

        public void ReceiveCall(string link) 
        {
            JsLib.OpenVoiceCall(this.name, link, "StopAudioClip");
        }

        public async UniTaskVoid MeetingOnTheSpot(List<int> seqs) 
        {
            string url = string.Format(URL.CREATE_MEETING, R.singleton.workspaceSeq);

            List<Seq> members = new List<Seq>();
            members.Clear();
            foreach (int seq in seqs) 
            {
                members.Add(new Seq(seq));
            }

            ReqMeetingInfo info = new ReqMeetingInfo();
            info.title = LocalizationSettings.StringDatabase.GetLocalizedString("word", "즉석회의", R.singleton.CurrentLocale);
            info.useYn = "Y";
            info.members = members;
            info.email = false;
            info.sms = false;
            info.run = true;
            info.pin = string.Empty;
            info.isPrivate = false;
            info.isOpen = false;
            var datetime = DateTime.Now;
            info.dt = datetime.ToString("yyyy-MM-dd");
            info.stm = datetime.ToString("HH:mm");
            datetime = datetime.AddHours(1);
            info.etm = datetime.ToString("HH:mm");
            string body = JsonUtility.ToJson(info);

            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.POST, body, R.singleton.token, true);
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return;
            }

            if (string.IsNullOrEmpty(res.stringData)) 
            {
                PopupBuilder.singleton.OpenAlert("결과값이 비어있음.");
                return;
            }

            string[] split = res.stringData.Split('/');
            int.TryParse(split[split.Length - 1], out int meetingSeq);
            string meetingLink = string.Format(URL.MEETING_LINK, R.singleton.workspaceSeq, meetingSeq, R.singleton.memberSeq, R.singleton.Region);
            JsLib.OpenWebview(meetingLink, "meeting");
        }

    #endregion  // communication
    }
}