/// <summary>
/// Xmpp 관련 매니저 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 10. 31
/// @version        : 0.4
/// @update
///     v0.1 (2023. 07. 18) : v1 에서 사용하던 것, 일부 수정해서 작성.
///     v0.2 (2023. 07. 31) : public variables (isXmpp, isWebView, isArrange) 주석 처리.
///     v0.3 (2023. 10. 26) : XmppLib 적용. World 에 적용.
///     v0.4 (2023. 10. 31) : world 에서는 권한 체크 안하게끔 조치.
/// </summary>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

using Xmpp;
using Xmpp.Net;
using Xmpp.Sasl;
using Xmpp.Xml.Dom;
using Xmpp.protocol.client;

namespace Joycollab.v2
{
    public class XmppManager : MonoBehaviour
    {
        // const value
        private const string TAG = "XmppManager";
        private const int XMPP_PORT = 5222;
        private const string MSG_PREFIX = "__HIDDEN__";
        public const string CONTENT_SPLITTER = "_P__!__S_";
        public const string TASK_SPLITTER = "_P__!!!__S_";

        // xmpp client, for connection check
        private XmppClientConnection xmppClient;
        [SerializeField] private string __id; 
        [SerializeField] private string __pw;

        // message queue
        private Queue<string> webglQueue;
        private Queue<Action> queue;

        // handling variables
        // public bool isXmpp = false;
        // public bool isWebView = false;
        // public bool isArrange = false;

        // TODO. 추후 사용할 때 주석 해제할 것.
        // instant alarm
        // private int seqForInstantAlarm;
        // private string txtForInstantAlarm;
        // private string typeForInstantAlarm;


    #region Unity functions

        private void Awake() 
        {
            xmppClient = null;
            __id = __pw = string.Empty;

        #if UNITY_WEBGL && !UNITY_EDITOR
            webglQueue = new Queue<string>();
        #else
            queue = new Queue<Action>();
        #endif

            // TODO. 추후 사용할 때 주석 해제할 것.
            // seqForInstantAlarm = 0;
            // txtForInstantAlarm = typeForInstantAlarm = string.Empty;

            // InitSingleton();
        }

        private void Update() 
        {
        #if UNITY_WEBGL && !UNITY_EDITOR
            if (webglQueue.Count > 0)
            {
                string msg = webglQueue.Dequeue();
                HandleMessage(msg);
            }
        #else 
            if (xmppClient != null && queue.Count > 0) 
            {
                Action act = queue.Dequeue();
                act();
            }
        #endif
        }

        /**
        #if UNITY_ANDROID && !UNITY_EDITOR
        private void OnApplicationPause(bool pauseStatus) 
        {
            Debug.Log($"{TAG} | OnApplicationPause(), pause status : {pauseStatus}, xmpp status : {xmppClient.Status}");

            // app 이탈
            if (pauseStatus) 
            {
                // TODO. 이탈시 이벤트 처리.
            }
            // app 복귀 
            else 
            {
                // TODO. 복귀시 이벤트 처리.
            }
        }
        #endif
         */

    #endregion  // Unity functions


    #region public functions for WebGL
    #if UNITY_WEBGL && !UNITY_EDITOR

        public void XmppLoginForWebGL(int memberSeq, string password) 
        {
            string url = string.Format(URL.XMPP_AUTH, memberSeq, password);
            Debug.Log($"{TAG} | XmppLoginForWebGL(), url : {url}");
            XmppLib.XmppLogin(this.name, url, "XmppOnMsgForWebGL");
        }

        public void XmppOnMsgForWebGL(string data) 
        {
            Debug.Log($"{TAG} | XmppOnMsgForWebGL(), received message : {data}");
            webglQueue.Enqueue(data);
        }

    #endif
    #endregion  // public functions for WebGL


    #region public functions

        public void Init() 
        {
            if (xmppClient == null)
            {
                xmppClient = new XmppClientConnection();
                Debug.Log($"{TAG} | Init(), XMPP client init.");

                // HANDLER SETTINGS
                xmppClient.OnSaslStart += new SaslEventHandler(XmppClientOnSaslStart);
                xmppClient.OnReadXml += (object sender, string xml) => { queue.Enqueue(() => XmppClientOnReadXml(sender, xml)); };
                xmppClient.OnWriteXml += new XmlHandler(XmppClientOnWriteXml);
                xmppClient.OnLogin += (object sender) => { queue.Enqueue(() => XmppClientOnLogin(sender)); };
                xmppClient.OnClose += (object sender) => { queue.Enqueue(() => XmppClientOnClose(sender)); };
                xmppClient.OnError += new ErrorHandler(XmppClientOnError);
                xmppClient.OnPresence += (object sender, Presence presence) => { queue.Enqueue(() => XmppClientOnPresence(sender, presence)); };
                xmppClient.OnMessage += (object sender, Message message) => { queue.Enqueue(() => XmppClientOnMessage(sender, message)); };
                xmppClient.OnAuthError += new XmppElementHandler(XmppClientOnAuthError);
                xmppClient.OnSocketError += new ErrorHandler(XmppClientOnSocketError);
                xmppClient.OnStreamError += new XmppElementHandler(XmppClientOnStreamError);
                xmppClient.OnXmppConnectionStateChanged += (object sender, XmppConnectionState state) => { queue.Enqueue(() => XmppClientOnConnectionStateChanged(sender, state)); };
                xmppClient.OnIq += (object sender, IQ iq) => { queue.Enqueue(() => XmppClientOnIq(sender, iq)); };

                // PROPERTY SETTINGS
                xmppClient.AutoRoster = true;
                xmppClient.AutoPresence = true;
                xmppClient.AutoResolveConnectServer = false;
                xmppClient.SocketConnectionType = SocketConnectionType.Direct;

                xmppClient.ConnectServer = URL.XMPP_SERVER;
                xmppClient.Port = XMPP_PORT;
                xmppClient.Status = "Login";
                xmppClient.Priority = 0;
                xmppClient.Show = ShowType.NONE;
            }
            else
            {
                Debug.Log($"{TAG} | Init(), XMPP client already init.");
            }
        }

        public void XmppLogin(string username, string password) 
        {
            if (xmppClient == null) 
            {
                Debug.Log($"{TAG} | XmppLogin() call, but client is null.");
                return;
            }
            Debug.Log($"{TAG} | XmppLogin() call");

            xmppClient.Username = username;
            xmppClient.Server = URL.XMPP_SERVER;
            xmppClient.Password = password;

            __id = username;
            __pw = password;

        #if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            xmppClient.Resource = S.WORKSPACE_MOBILE;
        #else
            xmppClient.Resource = S.WORKSPACE;
        #endif
            
            xmppClient.Open();
        }

        public void XmppLogout() 
        {
            if (xmppClient == null) return;

            xmppClient.Close();
            __id = __pw = string.Empty;
        }

    #endregion  // public functions


    #region private functions

        /**
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
         */

    #endregion  // private functions


    #region Xmpp client event handler

        private void XmppClientOnReadXml(object sender, string xml) 
        {
            // Debug.Log($"{TAG} | XmppClientOnReadXml(), xml : {xml}");
        }

        private void XmppClientOnWriteXml(object sender, string xml)
        {
            // Debug.Log($"{TAG} | XmppClientOnWriteXml(), xml : {xml}");
        }

        private void XmppClientOnLogin(object sender)
        {
            Debug.Log($"{TAG} | XmppClientOnLogin(), sender : {sender}");
        }

        private void XmppClientOnClose(object sender)
        {
            Debug.Log($"{TAG} | XmppClientOnClose(), sender : {sender}");

        #if UNITY_ANDROID && !UNITY_EDITOR
            // TODO. 모바일의 경우, 의도하지 않은 끊어짐이 발생시 재로그인 시도.
        #endif
        }

        private void XmppClientOnPresence(object sender, Presence e)
        {
            // Debug.Log($"{TAG} | XmppClientOnPresence(), sender : {sender}, Status : {e.Status}, Show : {e.Show}, priority : {e.Priority}");
        }

        private void XmppClientOnMessage(object sender, Message m)
        {
            Debug.Log($"{TAG} | XmppClientOnMessage(), sender : {sender}, message : {m.Body}");
            HandleMessage(m.Body);
        }

        private void XmppClientOnAuthError(object sender, Element e)
        {
            Debug.LogError($"{TAG} | XmppClientOnAuthError(), sender : {sender}, tag : {e.TagName}, prefix : {e.Prefix}, attr : {e.Attributes}, value : {e.InnerXml}");
        }

        private void XmppClientOnSaslStart(object sender, SaslEventArgs e)
        {
            Debug.Log($"{TAG} | XmppClientOnSaslStart(), sender : {sender}, log : {e}");
        }

        private void XmppClientOnError(object sender, Exception e) 
        {
            Debug.LogError($"{TAG} | XmppClientOnError(), sender : {sender}, exception : {e.Message}, stacktrace : {e.StackTrace}");
        }

        private void XmppClientOnSocketError(object sender, Exception e)
        {
            Debug.LogError($"{TAG} | XmppClientOnSocketError(), sender : {sender}, exception : {e.Message}, stacktrace : {e.StackTrace}");
        }

        private void XmppClientOnStreamError(object sender, Element e)
        {
            Debug.LogError($"{TAG} | XmppClientOnStreamError(), sender : {sender}, tag : {e.TagName}, prefix : {e.Prefix}, attr : {e.Attributes}, value : {e.InnerXml}");
        }

        private void XmppClientOnConnectionStateChanged(object sender, XmppConnectionState state) 
        {
            Debug.Log($"{TAG} | XmppClientOnConnectionStateChanged(), sender : {sender}, xmpp state : {state}");
        }

        private void XmppClientOnStreamStart(object sender, Xmpp.Xml.Dom.Node e)
        {
            Debug.Log($"{TAG} | XmppClientOnStreamStart(), sender : {sender}, node : {e}");
        }

        private void XmppClientOnStreamEnd(object sender, Xmpp.Xml.Dom.Node e) 
        {
            Debug.Log($"{TAG} | XmppClientOnStreamEnd(), sender : {sender}, node : {e}");
        }

        private void XmppClientOnIq(object sender, IQ iq) 
        {
            // get xmpp ping and send xmpp pong
            IqType type = iq.Type;
            string id = iq.GetAttribute("id");
            Element ping = iq.SelectSingleElement("ping");
            if (ping != null)
            {
                if (type == IqType.get && ping.Namespace.Equals(Xmpp.Uri.PING)) 
                {
                    XmppClientOnIqAnswer(id);
                }
            }
        }

        private void XmppClientOnIqAnswer(string id) 
        {
            if (xmppClient == null) return;

            IQ pong = new IQ(
                IqType.result,
                xmppClient.MyJID,
                new Jid(xmppClient.Server)
            );
            pong.SetAttribute("id", id);

            // Debug.Log($"{TAG} | XmppClientOnIqAnswer(), answer : {pong}");
            xmppClient.Send(pong);
        }

        private void HandleMessage(string message) 
        {
            if (! message.Contains(MSG_PREFIX)) 
            {
                Debug.Log($"{TAG} | hidden 이 붙어있지 않은 메시지. 이 클래스에서는 처리하지 않습니다. message : {message}");
                return;
            }

            // check message
            int prefix = MSG_PREFIX.Length;
            string serializedString = message.Substring(prefix);
            Debug.Log($"{TAG} | serialized string : {serializedString}");

            // check type
            XmppContent xcMsg = JsonUtility.FromJson<XmppContent>(serializedString);
            string alarmType = xcMsg.alarmType;
            Debug.Log($"{TAG} | alarm type : {alarmType}");

            // temp variables for common
            bool isMyInfo = false;
            bool isGuest = R.singleton.isGuest;
            string tempStr = string.Empty;
            int tempSeq = -1;

            // temp variables for instant alarm
            int seqForInstantAlarm = 0;
            string typeForInstantAlarm = string.Empty;
            string txtForInstantAlarm = string.Empty;

            // xmpp message parse
            eXmppType type = Util.StringToEnum<eXmppType>(alarmType);
            switch (type) 
            {
                case eXmppType.알림 :
                    if (xcMsg.tp.Equals("일감")) 
                    {
                        XmppTaskInfo xcTask = JsonUtility.FromJson<XmppTaskInfo>(xcMsg.contentJson);
                        string content = xcMsg.content.Replace(CONTENT_SPLITTER, "|");
                        var arrContent = content.Split('|');
                        string data = arrContent[0].Replace(TASK_SPLITTER, "|");
                        var arr = data.Split('|');

                        string title, key, value;
                        if (arr.Length > 1) 
                        {
                            key = "Kanban."+ arr[0];
                            value = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", key, R.singleton.CurrentLocale);
                            title = string.Format(value, arr[1]); 
                        }
                        else
                        {
                            key = "Kanban."+ content;
                            title = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", key, R.singleton.CurrentLocale);
                        }

                        if (arrContent.Length > 1) 
                        {
                            title += " ";

                            value = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", "Kanban.여러 항목 수정", R.singleton.CurrentLocale);
                            title += string.Format(value, (arrContent.Length - 1)); 
                        }
                    }
                    else
                    {
                        // isXmpp = true;

                        tempSeq = -1;
                        int.TryParse(xcMsg.contentJson, out tempSeq);
                        tempStr = string.Format("{0} {1}", xcMsg.title, xcMsg.content);

                        seqForInstantAlarm = tempSeq;
                        txtForInstantAlarm = tempStr;
                        typeForInstantAlarm = xcMsg.tp;
                        Debug.Log($"{TAG} | HandleMessage(), seq : {seqForInstantAlarm}, type : {typeForInstantAlarm}, text : {txtForInstantAlarm}");
                    }

                    // alarm 처리
                    NotifyAlarm(tempSeq, xcMsg.tp, isGuest);
                    break;

                case eXmppType.공간배치 :
                    // if (isWebView) isArrange = true;
                    break;

                case eXmppType.자리배치 :
                    if (SceneLoader.isGraphicUI()) 
                    {
                        Debug.Log($"{TAG} | HandleMessage(), update avatar seat : {xcMsg.seq}");
                        // AvatarManager.Instance.UpdateAvatarSeat(xcMsg.seq);
                    }
                    break;

                case eXmppType.자리이동 :
                    if (SceneLoader.isGraphicUI()) 
                    {
                        Debug.Log($"{TAG} | HandleMessage(), update avatar position : {xcMsg.seq}");
                        // AvatarManager.Instance.RelocateMember(xcMsg.seq, SpaceManager.Instance.Curspace);
                    }
                    break;

                case eXmppType.상태변경 :
                    isMyInfo = R.singleton.memberSeq == xcMsg.seq; 
                    if (xcMsg.cd == 0) 
                    {
                        Debug.Log($"{TAG} | HandleMessage(), update avatar seq : {xcMsg.seq}, need to refresh");
                        // if (isMyInfo)   StartCoroutine(AvatarManager.Instance.UpdateMyInfo());
                        // else            AvatarManager.Instance.UpdateOtherAvatar(xcMsg.seq);
                    }
                    else 
                    {
                        // TpsInfo info = R.singleton.GetMemberState(xcMsg.cd);
                        TpsInfo info = SystemManager.singleton.GetState(xcMsg.cd);
                        if (info != null) 
                        {
                            Debug.Log($"{TAG} | HandleMessage(), update avatar seq : {xcMsg.seq}, state : {info.id}");
                            // if (isMyInfo)   AvatarManager.Instance.UpdateMyState(info);
                            // else            AvatarManager.Instance.UpdateMemberState(xcMsg.seq, info);
                        }
                    }
                    break;

                case eXmppType.감정변경 :
                case eXmppType.멤버변경 :
                case eXmppType.멤버권한변경 :
                    isMyInfo = R.singleton.memberSeq == xcMsg.seq;
                    Debug.Log($"{TAG} | HandleMessage(), update avatar : {xcMsg.seq}, need to refresh");
                    // if (isMyInfo)   StartCoroutine(AvatarManager.Instance.UpdateMyInfo());
                    // else            AvatarManager.Instance.UpdateOtherAvatar(xcMsg.seq);
                    break;

                case eXmppType.멤버추가 :
                    // TODO. 상황을 보고 필요하면 기능 추가
                    break;

                case eXmppType.멤버탈퇴 :
                    Debug.Log($"{TAG} | HandleMessage(), delete member : {xcMsg.seq}, need to destroy");
                    break;

                case eXmppType.회의시작 :
                case eXmppType.회의예약 :
                case eXmppType.회의멤버변경 :
                    if (SceneLoader.isWorld()) 
                    {
                        Debug.Log($"{TAG} | HandleMessage(), World 에서는 무조건 회의실 업데이트.");
                    }
                    else 
                    {
                        if (R.singleton.CheckHasAuth(R.singleton.MeetingRoomSeq, S.AUTH_READ_MEETING)) 
                        {
                            Debug.Log($"{TAG} | HandleMessage(), 회의실 업데이트 필요.");
                        }
                        else 
                        {
                            Debug.Log($"{TAG} | HandleMessage(), 회의 조회 권한이 없기에, 회의실 업데이트를 하지 않습니다.");
                        }
                    }
                    break;

                case eXmppType.회의종료 :
                    if (SceneLoader.isWorld()) 
                    {
                        Debug.Log($"{TAG} | HandleMessage(), World 에서는 무조건 회의실 업데이트.");
                    }
                    else 
                    {
                        if (R.singleton.CheckHasAuth(R.singleton.MeetingRoomSeq, S.AUTH_READ_MEETING)) 
                        {
                            Debug.Log($"{TAG} | HandleMessage(), 회의실 업데이트 필요.");
                        }
                        else 
                        {
                            Debug.Log($"{TAG} | HandleMessage(), 회의 조회 권한이 없기에, 회의 종료 알림을 전달 하지 않습니다.");
                        }
                    }
                    break;

                case eXmppType.음성통화 :
                    int workspaceSeq = R.singleton.workspaceSeq;
                    int memberSeq = R.singleton.memberSeq; 
                    string lan = R.singleton.Region;
                    string callLink = string.Empty;

                #if UNITY_WEBGL

                    // TODO. 음성통화 소리 출력
                    callLink = string.Format(URL.CALL_LINK, workspaceSeq, xcMsg.seq, memberSeq, lan);
                    Debug.Log($"{TAG} | 음성통화 링크 : {callLink}");
                    SystemManager.singleton.ReceiveCall(callLink);

                #elif UNITY_ANDROID

                    string token = R.singleton.accessToken;

                    link = string.Format(URL.CALL_LINK_M, workspaceSeq, xcMsg.seq, memberSeq, lan, token);
                    AndroidLib.singleton.CheckVoicePermission(() => {
                        // TODO. 음성통화 소리 출력

                        WebviewBuilder.singleton.OpenMobileWebview(link, eWebviewType.VoiceCall);
                    }).Forget();

                #else



                #endif
                    break;

                case eXmppType.개인채팅 :
                case eXmppType.그룹채팅 :
                case eXmppType.개인및그룹채팅읽지않음총카운트 :
                    // TODO. 채팅 소리 출력

                    R.singleton.UnreadChatCount = xcMsg.unReadCnt;
                    break;

                case eXmppType.시스템공지 :
                    break;

                case eXmppType.공지 :
                    break;

                case eXmppType.게스트알림 :
                    break;

                case eXmppType.휴게실방알림 :
                    break;

                case eXmppType.메세지 :
                    break;

                default :
                    Debug.LogWarning($"{TAG} | HandleMessage(), 알 수 없는 타입의 xmpp message : {alarmType}");
                    break;
            }
        }

    #endregion  // Xmpp client event handler


    #region message execute 

        private void NotifyAlarm(int seq, string tp, bool isGuest=false) 
        {
            bool ringtoneOn = true;

            switch (tp) 
            {
                case "공간 배치 변경" :
                case "자리 배치 변경" :
                case "멤버 배치 변경" :
                case "멤버 정보 변경" :
                case "음성 통화" :
                case "일감" :
                    if (isGuest) ringtoneOn = false;
                    break;
                
                case "회의시작"  :
                    if (R.singleton.CheckHasAuth(R.singleton.MeetingRoomSeq, S.AUTH_READ_MEETING) || isGuest) 
                    {
                        // TODO. 미팅 시작 알림.
                    }
                    else 
                    {
                        Debug.Log($"{TAG} | 회의 조회 권한이 없기에, 회의 시작 알림을 전달 하지 않습니다.");
                        ringtoneOn = false;
                    }
                    break;
            }

            // event 처리
            if (ringtoneOn)
            {
                R.singleton.UnreadAlarmCount ++;
                // TODO. 알림창 알림음 소리 출력
            }
        }

    #endregion  // message execute
    }
}