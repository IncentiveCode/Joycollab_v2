#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace PitchSolution
{
    internal class JsLibPlugin
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    #region cookie
        [DllImport("__Internal")]
        public static extern string psGetCookie(string name);
        [DllImport("__Internal")]
        public static extern void psSetCookie(string name, string value);
    #endregion // cookie

    #region simple functions
        [DllImport("__Internal")]
        public static extern void psAlert(string message);
        [DllImport("__Internal")] 
        public static extern void psLog(string message);
        [DllImport("__Internal")] 
        public static extern void psRedirection(string url);
        [DllImport("__Internal")] 
        public static extern void psOpenWebview(string url, string id);
    #endregion // simple function

    #region check 
        [DllImport("__Internal")]
        public static extern void psCheckBrowser(string gameObjectName, string methodName);
        [DllImport("__Internal")] 
        public static extern void psCheckSystem(string gameObjectName, string methodName);
    #endregion // check 

    #region scheme
        [DllImport("__Internal")] 
        public static extern void psRunScheme(string gameObjectName, string url, string methodName);
    #endregion // scheme

#else

    #region cookie
        public static string psGetCookie(string name) { return PlayerPrefs.GetString(name, string.Empty); }
        public static void psSetCookie(string name, string value) { PlayerPrefs.SetString(name, value); }
    #endregion // cookie

    #region simple functions
        public static void psAlert(string message) { Debug.Log(message); }
        public static void psLog(string message) { Debug.Log(message); }
        public static void psRedirection(string url) { }
        public static void psOpenWebview(string url, string id) { Application.OpenURL(url); }
    #endregion // simple function

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
    #endregion // check 

    #region scheme
        public static void psRunScheme(string gameObjectName, string url, string methodName)
        {
            // Application.OpenURL(url);
            GameObject.Find(gameObjectName).SendMessage(methodName, "true");
            Debug.Log("잠시 막아둠.");
        }
    #endregion // scheme
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
    #endregion // cookie

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
    #endregion // simple functions

    #region check
        public static void CheckBrowser(string gameObjectName, string methodName)
        {
            JsLibPlugin.psCheckBrowser(gameObjectName, methodName);
        }

        public static void CheckSystem(string gameObjectName, string methodName)
        {
            JsLibPlugin.psCheckSystem(gameObjectName, methodName);
        }
    #endregion // check

    #region scheme
        public static void RunScheme(string gameObjectName, string url, string methodName)
        {
            JsLibPlugin.psRunScheme(gameObjectName, url, methodName);
        }
    #endregion // scheme
    }
}