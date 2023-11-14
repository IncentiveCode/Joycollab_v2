/// <summary>
/// 시스템 상 저장 공간 (Repository) 
/// @author         : HJ Lee
/// @last update    : 2023. 11. 14
/// @version        : 0.16
/// @update
///     v0.1 (2023. 03. 17) : 파일 생성, Joycollab 에서 사용하는 것들 정리 시작.
///     v0.2 (2023. 03. 31) : SimpleWorkspace, Alarm 관련 항목 정리 시작, Notify 에서 generic <T> 제거.
///     v0.3 (2023. 04. 07) : LanguageManager 내용 추가. 
///     v0.4 (2023. 05. 10) : LoginViewManager 에서 사용하던 Param 관련 함수 추가.
///     v0.5 (2023. 06. 16) : Locale 관련 코드 추가. (ChangeTextKorean, ChangeTextEnglish -> ChangeLocale)
///     v0.6 (2023. 07. 04) : Space Dictionary 관련 코드 추가.
///     v0.7 (2023. 07. 07) : To-Do, OKR 등 임시로 저장하는 항목은 Tmp 로 이동. Bookmark 저장소 추가.
///     v0.8 (2023. 07. 18) : 기존에 사용했던 member state 관리 추가.
///     v0.9 (2023. 07. 19) : 읽지 않은 채팅 카운트를 위해 변수와 getter 추가.
///     v0.10 (2023. 08. 10) : Localization Init 을 위해 Init() 을 async 로 변경.
///     v0.11 (2023. 08. 28) : Locale 을 반환하는 public 변수 추가.
///     v0.12 (2023. 09. 15) : Google, Zoom 관련 getter 추가.
///     v0.13 (2023. 09. 19) : ResMemberInfo 에 추가된 필드들에 대한 getter 추가 
///     v0.14 (2023. 11. 03) : avatar state 관련 정보를 System manager 로 이관.
///     v0.15 (2023. 11. 08) : time format 확인 함수와 culture info list 추가.
///     v0.16 (2023. 11. 14) : photo dict 형태 변경. Dictionary<int, texture2d> 에서 Dictionary <int, Tuple<string, texture2d>> 로 변경.
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class R : Singleton<R>, iRepositoryController
    {
        private const string TAG = "R";
        private List<Tuple<iRepositoryObserver, eStorageKey>> listObserver;


    #region Common functions
    
        private void Awake() 
        {
            // for common 
            listObserver = new List<Tuple<iRepositoryObserver, eStorageKey>>();
            listObserver.Clear();

            listCulture = new List<CultureInfo>();
            listCulture.Clear();
            listCulture.Add(new CultureInfo("ko-KR"));
            listCulture.Add(new CultureInfo("en-US"));
            listCulture.Add(new CultureInfo("ja-JP"));

            dictParams = new Dictionary<string, string>();
            dictParams.Clear();

            // for alarm
            listAlarm = new List<ResAlarmInfo>();
            listAlarm.Clear();

            // for bookmark
            listBookmark = new List<Bookmark>();
            listBookmark.Clear(); 

            // for temp dictionary
            dictPhoto = new Dictionary<int, Tuple<string, Texture2D>>();
            dictPhoto.Clear();
            dictSpace = new Dictionary<int, ResSpaceInfo>();
            dictSpace.Clear();
            dictPart = new Dictionary<int, string>();
            dictPart.Clear();
        }

        public async UniTask<bool> Init() 
        {
            Clear();

            _fontSizeOpt = 1;
            while (LocalizationSettings.InitializationOperation.Status != AsyncOperationStatus.Succeeded) 
                await UniTask.Yield();

            Debug.Log($"{TAG} | Init() done.");
            return true;
        }

        public void Clear() 
        {
            // cookie delete
            ClearTokenInfo();
            ClearMemberInfo();
            ClearWorkspaceInfo();
            
            // for alarm
            ClearAlarmInfo();

            // for bookmark
            ClearBookmark();

            // for temp dictionary
            ClearPhotoDict();
            ClearSpaceDict();
            ClearPartDict();
        }

    #endregion  // Common functions


    #region Storage Controller functions - implementations

        public void RegisterObserver(iRepositoryObserver observer, eStorageKey key)
        {
            bool exist = isExist(observer, key);
            if (exist) return; 

            // Debug.Log($"{TAG} | RegisterObserver(), observer : {observer}, key : {key}");
            listObserver.Add(Tuple.Create(observer, key));
        }

        public void UnregisterObserver(iRepositoryObserver observer, eStorageKey key) 
        {
            bool exist = isExist(observer, key);
            if (! exist) return;

            // Debug.Log($"{TAG} | UnregisterObserver(), observer : {observer}, key : {key}");
            listObserver.Remove(Tuple.Create(observer, key));  
        }

        public void RequestInfo(iRepositoryObserver observer, eStorageKey key) 
        {
            bool exist = isExist(observer, key);
            if (exist) Send(observer, key);
        }

        public void Notify(iRepositoryObserver observer, eStorageKey key) 
        {
            bool exist = isExist(observer, key);
            if (exist) Send(observer, key);
        }

        public void NotifyAll(eStorageKey key) 
        {
            foreach (Tuple<iRepositoryObserver, eStorageKey> t in listObserver)
            {
                if (t.Item2 == key) Send(t.Item1, t.Item2);
            }
        }

        private bool isExist(iRepositoryObserver observer, eStorageKey key) 
        {
            return listObserver.Any(i => i.Item1 == observer && i.Item2 == key);
        }

        private void Send(iRepositoryObserver observer, eStorageKey key) 
        {
            switch (key) 
            {
                case eStorageKey.MemberInfo :
                    // if (_memberInfo != null) observer.UpdateInfo(key);
                    // break;

                case eStorageKey.Alarm :
                case eStorageKey.InstantAlarm :
                case eStorageKey.Chat :
                case eStorageKey.FontSize :
                case eStorageKey.Locale :
                case eStorageKey.Elevator :
                case eStorageKey.WindowRefresh :
                case eStorageKey.UserCount :
                case eStorageKey.UserInfo :
                case eStorageKey.UserPhoto :
                    observer.UpdateInfo(key);
                    break;

                default :
                    Debug.Log("R | 줄 수 있는 것이 없음.");
                    break;
            }
        }

    #endregion  // ModuleController functions - implementations


    #region Language, region

        private string _region;
        private List<CultureInfo> listCulture;
        private CultureInfo _culture;
        private bool isChanging;

        public string Region 
        {
            get { 
                if (string.IsNullOrEmpty(_region)) 
                {
                    return Application.systemLanguage switch {
                        SystemLanguage.Korean => S.REGION_KOREAN,
                        SystemLanguage.Japanese => S.REGION_JAPANESE,
                        _ => S.REGION_ENGLISH
                    };
                }
                else 
                {
                    return _region;
                }
            }
            set {
                _region = value;
                NotifyAll(eStorageKey.Locale);
            }
        }
        public Locale CurrentLocale 
        {
            get {
                return LocalizationSettings.SelectedLocale;
            }
        }
        public CultureInfo CurrentCulture => _culture;
        public bool isKorean 
        {
            get {
                return Region.Equals(S.REGION_KOREAN);
            }
        }

        public void ChangeLocale(int locale) 
        {
            if (isChanging) return; 
            StartCoroutine(Change(locale));
        }
        private IEnumerator Change(int locale) 
        {
            isChanging = true;

            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[locale];

            _culture = listCulture[locale]; 

            Region = locale switch {
                0 => S.REGION_KOREAN,
                2 => S.REGION_JAPANESE,
                _ => S.REGION_ENGLISH
            };

            isChanging = false;
        }

    #endregion  // Language, region


    #region font setting

        private int _fontSizeOpt;

        public int FontSizeOpt 
        {
            get {
                return Mathf.Clamp(_fontSizeOpt, 1, 3);
            }
            set {
                _fontSizeOpt = Mathf.Clamp(value, 1, 3);
                NotifyAll(eStorageKey.FontSize);
            }
        }

    #endregion  // font setting


    #region elevator

        private int _elevatorOpt;

        public int ElevaotrOpt 
        {
            get {
                return Mathf.Clamp(_elevatorOpt, 0, 10);
            }
            set {
                _elevatorOpt = Mathf.Clamp(value, 0, 10);
                NotifyAll(eStorageKey.Elevator);
            }
        }

    #endregion  // elevator


    #region Token & important information

        private ResToken _tokenInfo = null;
        private string _id;
        private string _domainName;
        private int _workspaceSeq;
        private int _memberSeq;
        private string _uiType;
        private string _profile;
        private string _userName;

        public ResToken TokenInfo {
            set { _tokenInfo = value; }
        }
        public string accessToken {
            get { return _tokenInfo.access_token; }
            set { _tokenInfo.access_token = value; }
        }
        public string tokenType {
            get { return _tokenInfo.token_type; } 
            set { _tokenInfo.token_type = value; }
        }
        public string refreshToken {
            get { return _tokenInfo.refresh_token; }
            set { _tokenInfo.refresh_token = value; }
        }
        public int tokenExpire {
            get { return _tokenInfo.expires_in; }
            set { _tokenInfo.expires_in = value; }
        } 
        public string tokenScope {
            get { return _tokenInfo.scope; }
            set { _tokenInfo.scope = value; }
        }
        public string token {
            get { return string.Format("{0} {1}", tokenType, accessToken); }
        }

        public string ID {
            get { return _id; }
            set { _id = value; }
        }
        public string domainName {
            get { return _domainName; }
            set { _domainName = value; }
        }
        public int workspaceSeq {
            get { return _workspaceSeq; }
            set { _workspaceSeq = value; }
        }
        public int memberSeq {
            get { return _memberSeq; }
            set { _memberSeq = value; }
        }
        public string uiType {
            get { return _uiType; }
            set { _uiType = value; }
        }
        public string Profile {
            get { return _profile; }
            set { _profile = value; }
        }
        public string UserName {
            get { return _userName; }
            set { _userName = value; }
        }

        private void ClearTokenInfo() 
        {
            _tokenInfo = null;
            _id = _domainName = _uiType = _profile = _userName = string.Empty;
            _workspaceSeq = _memberSeq = 0;
        }

    #endregion   


    #region Param values

        private Dictionary<string, string> dictParams;

        public void AddParam(string key, string value) 
        {
            // Debug.Log($"R | AddParam(), key : {key}, value : {value}");
            if (dictParams.ContainsKey(key)) 
                dictParams[key] = value;
            else 
                dictParams.Add(key, value);
        }

        public string GetParam(string key) 
        {
            if (dictParams.ContainsKey(key))
                return dictParams[key];
            else
                return string.Empty;
        }

        public bool ExistParamKey(string key) 
        {
            return dictParams.ContainsKey(key);
        }

        public int GetParamCount() 
        {
            return dictParams.Count;
        }

        public void ClearParamValues() 
        {
            dictParams.Clear();
        }

    #endregion  // Param values


    #region Member Info 

        private ResMemberInfo _memberInfo = null;

        public ResMemberInfo MemberInfo {
            set { 
                _memberInfo = value; 
                Debug.Log($"{TAG} | member info notify all");
                NotifyAll(eStorageKey.MemberInfo);
            }
        }
        public string myMemberType {
            get { return _memberInfo.memberType; }
            set { _memberInfo.memberType = value; }
        }
        public bool isGuest {
            get { return _memberInfo.memberType.Equals(S.GUEST); }
        }
        public int mySpaceSeq {
            get { return _memberInfo.space.seq; }
        }
        public string mySpaceName {
            get { return _memberInfo.space.nm; }
        }
        public string myId {
            get { return _memberInfo.user.id; }
        }
        public string myName {
            get { return _memberInfo.nickNm; }
            set { _memberInfo.nickNm = value; }
        }
        public string myPhoto {
            get { return _memberInfo.photo; }
            set { _memberInfo.photo = value; }
        }
        public string myPhone {
            get { return _memberInfo.user.tel; }
            set { _memberInfo.user.tel = value; }
        }
        public string myGrade {
            get { return _memberInfo.jobGrade; }
            set { _memberInfo.jobGrade = value; }
        }
        public string myMessage {
            get { return string.IsNullOrEmpty(_memberInfo.description) ? string.Empty : _memberInfo.description; }
            set { _memberInfo.description = value; }
        }
        public string myAddr {
            get { return _memberInfo.addr; }
            set { _memberInfo.addr = value; }
        }
        public string myAddr2 {
            get { return _memberInfo.addrDtl; }
            set { _memberInfo.addrDtl = value; }
        }
        public string myEmotionId {
            get { return _memberInfo.emotion.id; }
        }
        public SettingInfo myAlarmOpt
        {
            get { return _memberInfo.alarmOpt; }
        }
        public float myLat {
            get { return _memberInfo.lat; }
        }
        public float myLon {
            get { return _memberInfo.lng; }
        }
        public bool isTimeFormatEquals24 {
            get { return _memberInfo.hourFormatStr.Equals("HH"); }
        }
        public string myStateId {
            get { return _memberInfo.status.id; }
        }
        public string myXmppId {
            get { return string.Format("jc-user-{0}", memberSeq); }
        }
        public string myXmppPw {
            get { return _memberInfo.xmppPw; }
        }
        public string myGoogleId {
            get { return _memberInfo.user.googleId; }
        }
        public bool isGoogleConnected {
            get { return !(string.IsNullOrEmpty(myGoogleId)); }
        }
        public string myZoomId {
            get { return _memberInfo.user.zoomId; }
        }
        public string myZoomMail {
            get { return _memberInfo.user.zoomEmail; }
        }
        public bool isZoomConnected {
            get { return !(string.IsNullOrEmpty(myZoomId)); }
        }
        public string ourPlan {
            get { return _memberInfo.plan; }
        }
        public string myStartDate {
            get { return _memberInfo.sdt; }
        }
        public string ourBusinessNumber {
            get { return _memberInfo.businessNum; }
        }
        public string ourCompanyName {
            get { return _memberInfo.compName; }
            set { _memberInfo.compName = value; }
        }
        public string ourBusiness {
            get { return _memberInfo.business; }
        }
        public string ourTel {
            get { return _memberInfo.tel; }
        }
        public string ourCeoName {
            get { return _memberInfo.ceoNm; }
        }
        public string ourMainBusiness {
            get { return _memberInfo.mainBusiness; }
        }
        public string ourHomepage {
            get { return _memberInfo.homepage; }
        }

        public string myInfoSerialize {
            get { return JsonUtility.ToJson(_memberInfo); }
        }

        // alarm enable check
        public bool CheckInUse(string opt) 
        {
            bool isUse = false;
            foreach (alarmOptItemInfo item in _memberInfo.alarmOpt.alarmOptItems) 
            {
                if (item.tp.id.Equals(opt) && item.alarm) 
                {
                    isUse = true;
                    break;
                }
            }

            return isUse;
        }

        // auth check
        public bool CheckHasAuth(int spaceSeq, string authName) 
        {
            MemberAuthInfo temp = null;

            // step 1. 공간에 대한 권한을 가지고 있는지 확인.
            foreach (MemberAuthInfo info in _memberInfo.memberAuths) 
            {
                if (info.space.seq == spaceSeq && info.useYn.Equals("Y"))
                {
                    temp = info;
                    break;
                }
            }
            if (temp == null) 
                return false;

            // step 2. 해당 공간에 대한 요청한 권한이 있는지 확인.
            bool result = false;
            foreach (MemberAuthMngInfo info in temp.authMngs) 
            {
                if (info.nm.Equals(authName) && info.useYn.Equals("Y"))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        // member info clear
        private void ClearMemberInfo() 
        {
            _memberInfo = null;
        }

    #endregion  // Member Info


    #region Workspace Info

        private SimpleWorkspace _simpleWorkspaceInfo = null;
        private LobbyInfo _lobbyInfo = null;

        public SimpleWorkspace SimpleWorkspaceInfo {
            set { _simpleWorkspaceInfo = value; }
        }
        public LobbyInfo CurrentLobbyInfo {
            set { _lobbyInfo = value; }
        }

        // TODO. 필요한 정보가 생길 때 마다 추가할 것.


        private void ClearWorkspaceInfo() 
        {
            _simpleWorkspaceInfo = null;
        }

    #endregion  // Workspace Info 


    #region Alarm Info  (ADD. 'chat count', 'world user count')

        private List<ResAlarmInfo> listAlarm;
        private int unreadAlarmCount;
        private int unreadChatCount;
        private int currentUserCount;

        public void AddAlarmInfo(ResAlarmInfo info) => listAlarm.Add(info);
        public int AlarmCount {
            get { return listAlarm.Count; }
        }

        public int UnreadAlarmCount {
            get { return unreadAlarmCount; }
            set { 
                unreadAlarmCount = Mathf.Clamp(value, 0, 100);
                NotifyAll(eStorageKey.Alarm);
            }
        }

        public int GetIndex(int seq) 
        {
            int index = listAlarm.FindIndex(item => item.seq == seq);    
            return index;
        }

        public void ClearAlarmInfo() 
        {
            listAlarm.Clear();
            unreadAlarmCount = 0;
            unreadChatCount = 0;
        }

        public int UnreadChatCount {
            get { return unreadChatCount; }
            set {
                unreadChatCount = Mathf.Clamp(value, 0, 100);
                NotifyAll(eStorageKey.Chat);
            }
        }

        public int CurrentUserCount {
            get { return currentUserCount; }
            set {
                currentUserCount = Mathf.Clamp(value, 0, 100);
                NotifyAll(eStorageKey.UserCount);
            }
        }

    #endregion  // Alarm Info


    #region Bookmark Info

        private List<Bookmark> listBookmark;
        
        public void AddBookmark(Bookmark info) => listBookmark.Add(info);
        public bool Marked(eBookmarkType type, int seq) 
        {
            int index = listBookmark.FindIndex(item => item.seq == seq && item.type == type);
            return (index != -1);
        }
        public void ClearBookmark() => listBookmark.Clear();

    #endregion  // Bookmark Info


    #region for temp dectionary

        private Dictionary<int, Tuple<string, Texture2D>> dictPhoto;
        public void AddPhoto(int seq, string path, Texture2D photo) 
        {
            if (dictPhoto.ContainsKey(seq)) 
            {
                if (dictPhoto[seq] != null) Destroy(dictPhoto[seq].Item2);
                dictPhoto[seq] = Tuple.Create(path, photo); 
            }
            else 
            {
                dictPhoto.Add(seq, Tuple.Create(path, photo));
            }
        }
        public string GetPhotoPath(int seq) 
        {
            if (dictPhoto.ContainsKey(seq))
                return dictPhoto[seq].Item1;
            else
                return string.Empty;
        }
        public Texture2D GetPhoto(int seq) 
        {
            if (dictPhoto.ContainsKey(seq))
                return dictPhoto[seq].Item2;
            else
                return null;
        }
        public void RemovePhoto(int seq) 
        {
            Debug.Log($"{TAG} | Remove Photo()... seq : {seq}");
            if (dictPhoto.ContainsKey(seq))
                dictPhoto.Remove(seq);

            Debug.Log($"{TAG} | Remove Photo() after : {dictPhoto.ContainsKey(seq)}");
        }
        private void ClearPhotoDict() => dictPhoto.Clear();


        private Dictionary<int, ResSpaceInfo> dictSpace;
        public int LobbySeq { get; private set; }
        public int LoungeSeq { get; private set; }
        public int MeetingRoomSeq { get; private set; }

        public void AddSpace(int seq, ResSpaceInfo info)
        { 
            if (dictSpace.ContainsKey(seq))
                dictSpace[seq] = info;
            else
                dictSpace.Add(seq, info);

            switch (info.spaceMng.spaceTp.id) 
            {
                case S.LOBBY :     
                    LobbySeq = info.seq;
                    break;

                case S.LOUNGE :
                    LoungeSeq = info.seq;
                    break;

                case S.MEETING_ROOM : 
                    MeetingRoomSeq = info.seq;
                    break;
                
                default : 
                    // Debug.Log($"{TAG} | 사무실은 따로 저장하지 않습니다.");
                    break;
            }
	    }
        public string GetSpaceName(int seq)
        { 
            if (dictSpace.ContainsKey(seq))
		        return dictSpace[seq].nm;	
            else
                return string.Empty;
	    }
        public int GetTopSpaceSeq(int seq) 
        {
            if (dictSpace.ContainsKey(seq)) 
            {
                if (dictSpace[seq].topSpace.seq == 0)
                    return dictSpace[seq].seq;
                else
                    return dictSpace[seq].topSpace.seq;
            }
            else 
                return -1;
        }
        public string GetTopSpaceName(int seq) 
        {
            if (dictSpace.ContainsKey(seq)) 
            {
                if (dictSpace[seq].topSpace.seq == 0) 
                    return dictSpace[seq].nm; 
                else 
                    return dictSpace[seq].topSpace.nm;
            }
            else 
                return string.Empty;
        }
        private void ClearSpaceDict() 
        {
            dictSpace.Clear();
            LobbySeq = LoungeSeq = MeetingRoomSeq = -1;
        }


        private Dictionary<int, string> dictPart;
        public void AddPartName(int seq, string name)
        { 
            if (dictPart.ContainsKey(seq))
                dictPart[seq] = name;
            else
                dictPart.Add(seq, name);
	    }
        public string GetPart(int seq)
        { 
            if (dictPart.ContainsKey(seq))
		        return dictPart[seq];	
            else
                return string.Empty;
	    }
        private void ClearPartDict() => dictPart.Clear();

    #endregion  // for temp dectionary
    }
}