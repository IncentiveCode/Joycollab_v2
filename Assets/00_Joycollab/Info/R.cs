/// <summary>
/// 시스템 상 저장 공간 (Repository) 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 04
/// @version        : 0.6
/// @update
///     v0.1 (2023. 03. 17) : 파일 생성, Joycollab 에서 사용하는 것들 정리 시작.
///     v0.2 (2023. 03. 31) : SimpleWorkspace, Alarm 관련 항목 정리 시작, Notify 에서 generic <T> 제거.
///     v0.3 (2023. 04. 07) : LanguageManager 내용 추가. 
///     v0.4 (2023. 05. 10) : LoginViewManager 에서 사용하던 Param 관련 함수 추가.
///     v0.5 (2023. 06. 16) : Locale 관련 코드 추가. (ChangeTextKorean, ChangeTextEnglish -> ChangeLocale)
///     v0.6 (2023. 07. 04) : Space Dictionary 관련 코드 추가.
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Joycollab.v2
{
    public class R : Singleton<R>, iRepositoryController
    {
        private List<Tuple<iRepositoryObserver, eStorageKey>> list;


    #region Common functions
    
        private void Awake() 
        {
            list = new List<Tuple<iRepositoryObserver, eStorageKey>>();
            list.Clear();

            dictParams = new Dictionary<string, string>();
            dictParams.Clear();

            // for alarm
            alarmList = new List<ResAlarmInfo>();
            alarmList.Clear();

            // for to-do, OKR
            listToDo = new List<ToDoData>();
            listToDo.Clear();

            listOkr = new List<OkrData>();
            listOkr.Clear();

            // for temp dictionary
            dictPhoto = new Dictionary<int, Texture2D>();
            dictPhoto.Clear();

            dictSpaceName = new Dictionary<int, string>();
            dictSpaceName.Clear();
            
            dictPart = new Dictionary<int, string>();
            dictPart.Clear();
        }

        public void Clear() 
        {
            // cookie delete
            ClearTokenInfo();
            ClearMemberInfo();
            ClearWorkspaceInfo();
            
            // for alarm
            ClearAlarmInfo();

            //for to-do, OKR
            ClearToDoList();
            ClearOkrList();

            // for temp dictionary
            ClearPhotoDict();
            ClearSpaceNameDict();
            ClearPartDict();
        }

    #endregion  // Common functions


    #region Storage Controller functions - implementations

        public void RegisterObserver(iRepositoryObserver observer, eStorageKey key)
        {
            bool exist = isExist(observer, key);
            if (exist) return; 

            list.Add(Tuple.Create(observer, key));
        }

        public void UnregisterObserver(iRepositoryObserver observer, eStorageKey key) 
        {
            bool exist = isExist(observer, key);
            if (exist)
            {
                list.Remove(Tuple.Create(observer, key));  
            }
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
            foreach (Tuple<iRepositoryObserver, eStorageKey> t in list) 
            {
                if (t.Item2 == key) Send(t.Item1, t.Item2);
            }
        }

        private bool isExist(iRepositoryObserver observer, eStorageKey key) 
        {
            return list.Any(i => i.Item1 == observer && i.Item2 == key);
        }

        private void Send(iRepositoryObserver observer, eStorageKey key) 
        {
            switch (key) 
            {
                case eStorageKey.UserInfo :
                    if (_memberInfo != null) observer.UpdateInfo(key);
                    break;

                case eStorageKey.Alarm :
                    // TODO. alarm 정보 예외 처리 추가
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
        private bool isChanging;

        public string Region 
        {
            get { 
                if (string.IsNullOrEmpty(_region)) 
                {
                    if (Application.systemLanguage == SystemLanguage.Korean) 
                        return S.REGION_KOREAN;
                    else 
                        return S.REGION_ENGLISH;
                }
                else 
                {
                    return _region;
                }
            }
        }
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

            _region = locale == 0 ? S.REGION_KOREAN : S.REGION_ENGLISH;

            isChanging = false;
        }

    #endregion  // Language, region


    #region Token & important information

        private ResToken _tokenInfo = null;
        private string _id;
        private string _domainName;
        private int _workspaceSeq;
        private int _memberSeq;
        private string _uiType;

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

        private void ClearTokenInfo() 
        {
            _tokenInfo = null;
            _id = _domainName = _uiType = string.Empty;
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
            set { _memberInfo = value; }
        }
        public string myMemberType {
            get { return _memberInfo.memberType; }
            set { _memberInfo.memberType = value; }
        }
        public int mySpaceSeq {
            get { return _memberInfo.space.seq; }
        }
        public string mySpaceName {
            get { return _memberInfo.space.nm; }
        }
        public string myName {
            get { return _memberInfo.nickNm; }
            set { _memberInfo.nickNm = value; }
        }
        public string myPhoto {
            get { return _memberInfo.photo; }
            set { _memberInfo.photo = value; }
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
        public float myLat {
            get { return _memberInfo.lat; }
        }
        public float myLon {
            get { return _memberInfo.lng; }
        }
        public string myXmppPw {
            get { return _memberInfo.xmppPw; }
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


    #region Alarm Info 

        private List<ResAlarmInfo> alarmList;
        private int unreadAlarmCount;

        public void AddAlarmInfo(ResAlarmInfo info) => alarmList.Add(info);
        public int AlarmCount {
            get { return alarmList.Count; }
        }

        public int UnreadAlarmCount {
            get { return unreadAlarmCount; }
            set { 
                unreadAlarmCount = Mathf.Clamp(value, 0, 100);
                NotifyAll(eStorageKey.Alarm);
            }
        }

        // TODO. 필요한 정보가 생길 때 마다 추가할 것.


        public void ClearAlarmInfo() 
        {
            alarmList.Clear();
            unreadAlarmCount = -1;
        }

    #endregion  // Alarm Info


    #region To-Do, OKR Info

        private List<ToDoData> listToDo;

        public void AddToDoInfo(int seq, ToDoData todo)
        {
            int index = listToDo.FindIndex(item => item.info.seq == seq);
            if (index == -1) 
                listToDo.Add(todo);
            else             
                listToDo[index].info = todo.info;
        }
        public ToDoData GetToDoInfo(int seq) 
        {
            int index = listToDo.FindIndex(item => item.info.seq == seq);
            if (index == -1) return null;
            
            return listToDo[index];
        }
        public void ClearToDoList() => listToDo.Clear(); 


        private List<OkrData> listOkr;

        public int AddOkrInfo(int seq, OkrData data)
        { 
            int index = listOkr.FindIndex(item => item.seq == seq);
            if (index == -1)
                listOkr.Add(data);
            else
                listOkr[index].info = data.info;

            return index;
	    }
        public OkrData GetOkrInfo(int seq)
        { 
            int index = listOkr.FindIndex(item => item.seq == seq);
            if (index == -1) return null;

			return listOkr[index];
	    }
        public void ClearOkrList() => listOkr.Clear();
        
    #endregion  // To-Do, OKR Info


    #region memory management

        private Dictionary<int, Texture2D> dictPhoto;
        public void AddPhoto(int seq, Texture2D photo) 
        {
            if (dictPhoto.ContainsKey(seq)) 
            {
                if (dictPhoto[seq] != null) Destroy(dictPhoto[seq]);
                dictPhoto[seq] = photo;
            }
            else 
            {
                dictPhoto.Add(seq, photo);
            }
        }
        public Texture2D GetPhoto(int seq) 
        {
            if (dictPhoto.ContainsKey(seq))
                return dictPhoto[seq];
            else
                return null;
        }
        private void ClearPhotoDict() => dictPhoto.Clear();


        private Dictionary<int, string> dictSpaceName;
        public void AddSpaceName(int seq, string name)
        { 
            if (dictSpaceName.ContainsKey(seq))
                dictSpaceName[seq] = name;
            else
                dictSpaceName.Add(seq, name);
	    }
        public string GetSpaceName(int seq)
        { 
            if (dictSpaceName.ContainsKey(seq))
		        return dictSpaceName[seq];	
            else
                return string.Empty;
	    }
        private void ClearSpaceNameDict() => dictSpaceName.Clear();


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

    #endregion  // memory management
    }
}