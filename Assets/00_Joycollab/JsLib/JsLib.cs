/// <summary>
/// PitchSolution - javascript library 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 13 
/// @version        : 0.4
/// @update
///     v0.1 (2023. 02. 22) : Joycollab 에서 사용하던 클래스 정리 및 통합 (진행 중)
///     v0.2 (2023. 02. 28) : unity 2021.3.13f1 으로 업그레이드 후, windows 에서 build 안되는 문제 해결. (한글 주석이 원인으로 보임)
///     v0.3 (2023. 03. 17) : Graphic UI 와 Text UI 전환시 unity-canvas 에 min-widht 값을 추가하는 함수 추가. 추후 고도화 예정.
///     v0.4 (2023. 07. 13) : WebGL 이 아닌 곳에서 Alert 사용시 Popup builder 를 이용한 open alert 출력하게 수정.
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
        public static extern void psCheckBrowser(string gameObjectName, string methodName);
        [DllImport("__Internal")] 
        public static extern void psCheckSystem(string gameObjectName, string methodName);
    #endregion  // check 

    #region scheme
        [DllImport("__Internal")] 
        public static extern void psRunScheme(string gameObjectName, string url, string methodName);
    #endregion  // scheme

    #region change style
        [DllImport("__Internal")] 
        public static extern void psSetTextUI(bool isOn);
    #endregion  // change style

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

#endif
    }


    public class JsLib : MonoBehaviour
    {
    #region Cookie

        public static string GetCookie(string name) 
        {
            string cookie = string.Empty;
            cookie = JsLibPlugin.psGetCookie(name);
            return cookie;
        }

        public static void SetCookie(string name, string value) 
        {
            JsLibPlugin.psSetCookie(name, value);
        }

    #endregion  // cookie

    #region simple functions

        public static void Alert(string message) 
        {
            JsLibPlugin.psAlert(message);
        }

        public static void Log(string message) 
        {
            JsLibPlugin.psLog(message);
        }

        public static void Redirection(string url) 
        {
            JsLibPlugin.psRedirection(url);
        }

        public static void OpenWebview(string url, string id) 
        {
            JsLibPlugin.psOpenWebview(url, id);
        }

    #endregion  // simple functions

    #region check

        public static void CheckBrowser(string gameObjectName, string methodName)
        {
            JsLibPlugin.psCheckBrowser(gameObjectName, methodName);
        }

        public static void CheckSystem(string gameObjectName, string methodName)
        {
            JsLibPlugin.psCheckSystem(gameObjectName, methodName);
        }

    #endregion  // check

    #region scheme

        public static void RunScheme(string gameObjectName, string url, string methodName)
        {
            JsLibPlugin.psRunScheme(gameObjectName, url, methodName);
        }

    #endregion  // scheme

    #region change style

        public static void SetTextUI(bool isOn) 
        { 
            JsLibPlugin.psSetTextUI(isOn);
        }

    #endregion  // change style
    }
}