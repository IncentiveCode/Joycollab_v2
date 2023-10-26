/// <summary>
/// PitchSolution - javascript library for XMPP
/// @author         : HJ Lee
/// @last update    : 2023. 10. 26 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 02. 22) : v1 에서 사용하던 클래스 정리 및 통합.
/// </summary>

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Joycollab.v2
{
    internal class XmppLibPlugin 
    {
#if UNITY_WEBGL && !UNITY_EDITOR

    #region XMPP

        [DllImport("__Internal")]
        public static extern void psXmppLogin(string gameObjectName, string url, string callbackMethodName);

        [DllImport("__Internal")]
	    public static extern void psXmppLogout();

        [DllImport("__Internal")]
	    public static extern void psXmppRefresh();

    #endregion  // XMPP

#else

    #region XMPP

        public static void psXmppLogin(string gameObjectName, string url, string callbackMethodName) 
        { 
            Debug.Log("XmppLibPlugin | xmpp login - callback method test.");
            GameObject.Find(gameObjectName).SendMessage(callbackMethodName); 
        }
        public static void psXmppLogout() { }
        public static void psXmppRefresh() { }

    #endregion  // XMPP

#endif
    }

    public class XmppLib : MonoBehaviour
    {
    #region XMPP

        public static void XmppLogin(string gameObjectName, string url, string callbackMethodName) 
            => XmppLibPlugin.psXmppLogin(gameObjectName, url, callbackMethodName);
        public static void XmppLogout() => XmppLibPlugin.psXmppLogout();
        public static void XmppRefresh() => XmppLibPlugin.psXmppRefresh();

    #endregion  // XMPP
    }
}