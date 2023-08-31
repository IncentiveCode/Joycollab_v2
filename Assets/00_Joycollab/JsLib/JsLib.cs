/// <summary>
/// PitchSolution - javascript library 
/// @author         : HJ Lee
/// @last update    : 2023. 08. 29 
/// @version        : 0.8
/// @update
///     v0.1 (2023. 02. 22) : Joycollab 에서 사용하던 클래스 정리 및 통합.
///     v0.2 (2023. 02. 28) : unity 2021.3.13f1 으로 업그레이드 후, windows 에서 build 안되는 문제 해결. (한글 주석이 원인으로 보임)
///     v0.3 (2023. 03. 17) : Graphic UI 와 Text UI 전환시 unity-container 에 min-widht 값을 추가하는 함수 추가. 추후 고도화 예정.
///     v0.4 (2023. 07. 14) : WebGL 이 아닌 곳에서 Alert 사용시 Popup builder 를 이용한 open alert 출력하게 수정.
///                         copyToClipboard() 추가. 참고 : https://pudding-entertainment.medium.com/unity-webgl-add-a-share-button-93831b3555e9
///     v0.5 (2023. 07. 19) : Joycollab 에서 사용하던 popup 관련 클래스 정리 및 통합.
///     v0.6 (2023. 07. 26) : ConnectInnerWebview() 의 오류로 인해 빌드 실패하는 문제 해결.
///     v0.7 (2023. 08. 28) : ClearTokenCookie() 추가.
///     v0.8 (2023. 08. 29) : kakao search address 추가.
/// </summary>

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Joycollab.v2
{
    internal class JsLibPlugin
    {
#if UNITY_WEBGL && !UNITY_EDITOR

    #region cookie

        [DllImport("__Internal")]
        public static extern string psGetCookie(string name);
        [DllImport("__Internal")]
        public static extern void psSetCookie(string name, string value);

    #endregion  // cookie


    #region simple functions

        [DllImport("__Internal")]
        public static extern void psAlert(string message);
        [DllImport("__Internal")] 
        public static extern void psLog(string message);
        [DllImport("__Internal")] 
        public static extern void psRedirection(string url);
        [DllImport("__Internal")] 
        public static extern void psOpenWebview(string url, string id);

    #endregion  // simple function


    #region check 

        [DllImport("__Internal")]
        public static extern void psCheckBrowser(string gameObjectName, string callbackMethodName);
        [DllImport("__Internal")] 
        public static extern void psCheckSystem(string gameObjectName, string callbackMethodName);

    #endregion  // check 


    #region scheme

        [DllImport("__Internal")] 
        public static extern void psRunScheme(string gameObjectName, string url, string callbackMethodName);

    #endregion  // scheme


    #region change style

        [DllImport("__Internal")] 
        public static extern void psSetTextUI(bool isOn);

    #endregion  // change style


    #region copy and paste

        [DllImport("__Internal")] 
        public static extern void psCopyToClipboard(string text);
        // [DllImport("__Internal")] public static extern string psPasteText();

    #endregion


    #region popup, webview

        [DllImport("__Internal")] 
        public static extern void psOpenVoiceCall(string gameObjectName, string url, string receivedMethodName);
        [DllImport("__Internal")] 
        public static extern void psOpenPayment(string gameObjectName, string url, string doneMethodName);
        [DllImport("__Internal")] 
        public static extern void psOpenAuth(string gameObjectName, string url, string callbackMethodName);
        [DllImport("__Internal")] 
        public static extern void psOpenChat(string url, int targetSeq);
        [DllImport("__Internal")] 
        public static extern bool psCheckChat();
        [DllImport("__Internal")] 
        public static extern void psConnectInnerWebview(string gameObjectName, string closeMethodName);

    #endregion  // popup, webview


    #region location, address

        [DllImport("__Internal")] 
        public static extern void psGetLocation(string gameObjectName, string callbackMethodName);
        [DllImport("__Internal")] 
        public static extern void psSearchAddress(string gameObjectName, string callbackMethodName);

    #endregion  // location, address

#else

    #region cookie

        public static string psGetCookie(string name) { return PlayerPrefs.GetString(name, string.Empty); }
        public static void psSetCookie(string name, string value) { PlayerPrefs.SetString(name, value); }

    #endregion  // cookie


    #region simple functions

        public static void psAlert(string message) { PopupBuilder.singleton.OpenAlert(message); }
        public static void psLog(string message) { Debug.Log(message); }
        public static void psRedirection(string url) { }
        public static void psOpenWebview(string url, string id) { Application.OpenURL(url); }

    #endregion  // simple function


    #region check 

        public static void psCheckBrowser(string gameObjectName, string methodName) 
        {
            string lan = (Application.systemLanguage == SystemLanguage.Korean) ? "ko" : "en";
            string result = $"editor|{lan}";
            GameObject.Find(gameObjectName).SendMessage(methodName, result);
        }
        public static void psCheckSystem(string gameObjectName, string methodName)
        {
            GameObject.Find(gameObjectName).SendMessage(methodName, SystemInfo.operatingSystem);
        }

    #endregion  // check 


    #region scheme

        public static void psRunScheme(string gameObjectName, string url, string methodName)
        {
            Application.OpenURL(url);
            GameObject.Find(gameObjectName).SendMessage(methodName, "true");
        }

    #endregion  // scheme


    #region change style

        public static void psSetTextUI(bool isOn) { }

    #endregion  // change style


    #region copy and paste

        private static TextEditor te = new TextEditor();
        public static void psCopyToClipboard(string text)
        { 
            te.text = text;
            te.SelectAll();
            te.Copy();
        }
        /**
        public static string psPasteText() 
        {
            te.Paste();
            return te.text;
        }
         */

    #endregion  // copy and paste


    #region popup, webview

        public static void psOpenVoiceCall(string gameObjectName, string url, string receivedMethodName)
        {
            Application.OpenURL(url);
            GameObject.Find(gameObjectName).SendMessage(receivedMethodName);
        }
        public static void psOpenPayment(string gameObjectName, string url, string doneMethodName)
        {
            Application.OpenURL(url);
            GameObject.Find(gameObjectName).SendMessage(doneMethodName);
        }
        public static void psOpenAuth(string gameObjectName, string url, string callbackMethodName)
        {
            Application.OpenURL(url);
            GameObject.Find(gameObjectName).SendMessage(callbackMethodName);
        }
        public static void psOpenChat(string url, int targetSeq) => Application.OpenURL(url);
        public static bool psCheckChat() { return false; }
        public static void psConnectInnerWebview(string gameObjectName, string closeMethodName) { }

    #endregion  // popup, webview


    #region location

        public static void psGetLocation(string gameObjectName, string callbackMethodName)
        {
            string result = $"editor|위치 정보를 가지고 오지 못함.";
            GameObject.Find(gameObjectName).SendMessage(callbackMethodName, result);
        }

        public static void psSearchAddress(string gameObjectName, string callbackMethodName)
        {
            string result = $"서울시 금천구 가산디지털1로 205|37.483621|126.879779";
            GameObject.Find(gameObjectName).SendMessage(callbackMethodName, result);
        }

    #endregion  // location

#endif
    }


    public class JsLib : MonoBehaviour
    {
    #region Cookie

        public static string GetCookie(string name) 
        {
            string cookie = JsLibPlugin.psGetCookie(name);
            return cookie;
        }
        public static void SetCookie(string name, string value) => JsLibPlugin.psSetCookie(name, value);
        public static void ClearTokenCookie() 
        {
            SetCookie(Key.TOKEN_TYPE, string.Empty);
            SetCookie(Key.ACCESS_TOKEN, string.Empty);
            SetCookie(Key.WORKSPACE_SEQ, string.Empty);
            SetCookie(Key.MEMBER_SEQ, string.Empty);
            SetCookie(Key.GUEST_ID, string.Empty);
            SetCookie(Key.GUEST_PASSWORD, string.Empty);
        }

    #endregion  // cookie


    #region simple functions

        public static void Alert(string message) => JsLibPlugin.psAlert(message);
        public static void Log(string message) => JsLibPlugin.psLog(message);
        public static void Redirection(string url) => JsLibPlugin.psRedirection(url);
        public static void OpenWebview(string url, string id) => JsLibPlugin.psOpenWebview(url, id);

    #endregion  // simple functions


    #region check

        public static void CheckBrowser(string gameObjectName, string methodName) => JsLibPlugin.psCheckBrowser(gameObjectName, methodName);
        public static void CheckSystem(string gameObjectName, string methodName) => JsLibPlugin.psCheckSystem(gameObjectName, methodName);

    #endregion  // check


    #region scheme

        public static void RunScheme(string gameObjectName, string url, string methodName) => JsLibPlugin.psRunScheme(gameObjectName, url, methodName);

    #endregion  // scheme


    #region change style

        public static void SetTextUI(bool isOn) => JsLibPlugin.psSetTextUI(isOn);

    #endregion  // change style


    #region copy and paste

        public static void CopyToClipboard(string text) => JsLibPlugin.psCopyToClipboard(text);
        /**
        public static string PasteText() 
        {
            return JsLibPlugin.psPasteText();
        }
         */

    #endregion  // copy and paste


    #region popup, webview

        public static void OpenVoiceCall(string gameObjectName, string url, string receivedMethodName) => JsLibPlugin.psOpenVoiceCall(gameObjectName, url, receivedMethodName);
        public static void OpenPayment(string gameObjectName, string url, string doneMethodName) => JsLibPlugin.psOpenPayment(gameObjectName, url, doneMethodName);
        public static void OpenAuth(string gameObjectName, string url, string callbackMethodName) => JsLibPlugin.psOpenAuth(gameObjectName, url, callbackMethodName);
        public static void OpenChat(string url, int targetSeq=0) => JsLibPlugin.psOpenChat(url, targetSeq);
        public static bool CheckChat() 
        {
            return JsLibPlugin.psCheckChat();
        }
        public static void ConnectInnerWebview(string gameObjectName, string closeMethodName) => JsLibPlugin.psConnectInnerWebview(gameObjectName, closeMethodName);

    #endregion  // popup, webview


    #region location, address

        public static void GetLocation(string gameObjectName, string callbackMethodName) => JsLibPlugin.psGetLocation(gameObjectName, callbackMethodName);
        public static void SearchAddress(string gameObjectName, string callbackMethodName) => JsLibPlugin.psSearchAddress(gameObjectName, callbackMethodName);

    #endregion  // location, address
    }
}