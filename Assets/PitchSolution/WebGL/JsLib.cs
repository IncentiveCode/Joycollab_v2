using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace PitchSolution
{
    public static class JsLib
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    #region cookie
        [DllImport("__Internal")] private static extern string psGetCookie(string name);
        [DllImport("__Internal")] private static extern void psSetCookie(string name, string value);
    #endregion // cookie

    #region simple functions
        [DllImport("__Internal")] private static extern void psAlert(string message);
        [DllImport("__Internal")] private static extern void psLog(string message);
        [DllImport("__Internal")] private static extern void psRedirection(string url);
        [DllImport("__Internal")] private static extern void psOpenWebview(string url, string id);
    #endregion // simple function

    #region check 
        [DllImport("__Internal")] private static extern string psCheckBrowser(string gameObjectName, string methodName);
        [DllImport("__Internal")] private static extern string psCheckSystem(string gameObjectName, string methodName);
    #endregion // check 

    #region scheme
        [DllImport("__Internal")] private static extern string psRunScheme(string gameObjectName, string url, string methodName);
    #endregion // scheme
#endif


    #region Cookie
        public static string GetCookie(string name) 
        {
            string cookie = string.Empty;

        #if UNITY_WEBGL && !UNITY_EDITOR
            cookie = psGetCookie(name);
        #else
            cookie = PlayerPrefs.GetString(name, string.Empty);
        #endif

            return cookie;
        }

        public static void SetCookie(string name, string value) 
        {
        #if UNITY_WEBGL && !UNITY_EDITOR
            psSetCookie(name, value);
        #else 
            PlayerPrefs.SetString(name, value);
        #endif
        }
    #endregion // cookie


    #region simple functions
        public static void Alert(string message) 
        {
        #if UNITY_WEBGL && !UNITY_EDITOR
            psAlert(message);
        #else
            Debug.Log(message);
        #endif
        }

        public static void Log(string message) 
        {
        #if UNITY_WEBGL && !UNITY_EDITOR
            psLog(message);
        #else
            Debug.Log(message);
        #endif
        }

        public static void Redirection(string url) 
        {
        #if UNITY_WEBGL && !UNITY_EDITOR
            psRedirection(url);
        #endif
        }

        public static void OpenWebview(string url, string id) 
        {
        #if UNITY_WEBGL && !UNITY_EDITOR
            psOpenWebview(url, id);
        #else
            Application.OpenURL(url);
        #endif
        }
    #endregion // simple functions


    #region check
        public static void CheckBrowser(string gameObjectName, string methodName)
        {
        #if UNITY_WEBGL && !UNITY_EDITOR
            psCheckBrowser(gameObjectName, methodName);
        #else
            string lan = (Application.systemLanguage == SystemLanguage.Korean) ? "ko" : "en";
            string result = $"editor|{lan}";
            GameObject.Find(gameObjectName).SendMessage(methodName, result);
        #endif
        }

        public static void CheckSystem(string gameObjectName, string methodName)
        {
        #if UNITY_WEBGL && !UNITY_EDITOR
            psCheckSystem(gameObjectName, methodName);
        #else
            GameObject.Find(gameObjectName).SendMessage(methodName, SystemInfo.operatingSystem);
        #endif
        }
    #endregion // check

    #region scheme
        public static void RunScheme(string gameObjectName, string url, string methodName)
        {
        #if UNITY_WEBGL && !UNITY_EDITOR
            psRunScheme(gameObjectName, url, methodName);
        #else
            Application.OpenURL(url);
            GameObject.Find(gameObjectName).SendMessage(methodName, "true");
        #endif
        }
    #endregion // scheme


    }
}