/// <summary>
/// 웹뷰를 제어하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 16
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 16) : 최초 생성, gree webview 적용.
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2 
{
    public class WebviewController : MonoBehaviour
    {
        [Header("top")]
        [SerializeField] private GameObject _goTop;
        [SerializeField] private Button _btnClose;

        [Header("body")]
        [SerializeField] private GameObject _goPanel;

        private WebViewObject webViewObject;


    #region Unity functions

        private void Awake() 
        {
            webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
            webViewObject.Init(
                cb: (msg) =>
                {
                    Debug.Log("MobileWebView | msg : "+ msg);

                    if (msg.Equals("closeWebview")) 
                    {
                        Close();
                    }
                },
                err: (msg) =>
                {
                    Debug.LogError(string.Format("CallOnError[{0}]", msg));
                },
                httpErr: (msg) =>
                {
                    Debug.Log(string.Format("CallOnHttpError[{0}]", msg));
                },
                started: (msg) =>
                {
                    Debug.Log(string.Format("CallOnStarted[{0}]", msg));
                },
                hooked: (msg) =>
                {
                    Debug.Log(string.Format("CallOnHook ed[{0}]", msg));
                },
                ld: (msg) =>
                {
                    Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
                }
            );

            _btnClose.onClick.AddListener(() => Close());
        }

    #endregion  // Unity functions


    #region Public functions

        public void OpenMobileWeb(string url) 
        {
            _goTop.SetActive(true);
            _goPanel.SetActive(true);
            webViewObject.transform.SetParent(_goPanel.transform);
            webViewObject.LoadURL(url);

            int top = 0;
        #if UNITY_ANDROID
            top = (int) (60 * (Screen.dpi / 160));
        #elif UNITY_IOS

        #endif

            webViewObject.SetMargins(0, top, 0, 0);
            webViewObject.SetCameraAccess(false);
            webViewObject.SetMicrophoneAccess(false);
            webViewObject.SetVisibility(true);
        }


        public void OpenMobileChat(string url) 
        {
            _goTop.SetActive(false);
            _goPanel.SetActive(true);
            webViewObject.transform.SetParent(_goPanel.transform);
            webViewObject.LoadURL(url);

            int bottom = 0;
        #if UNITY_ANDROID
            bottom = (int) (80 * (Screen.dpi / 160));
        #elif UNITY_IOS

        #endif

            webViewObject.SetMargins(0, 0, 0, bottom);
            webViewObject.SetCameraAccess(false);
            webViewObject.SetMicrophoneAccess(false);
            webViewObject.SetVisibility(true);
        }

        public void OpenMobileVoiceCall(string url) 
        {
            _goTop.SetActive(false);
            _goPanel.SetActive(true);
            webViewObject.transform.SetParent(_goPanel.transform);
            webViewObject.LoadURL(url);

            webViewObject.SetMargins(0, 0, 0, 0);
            webViewObject.SetCameraAccess(false);
            webViewObject.SetMicrophoneAccess(true);
            webViewObject.SetVisibility(true);
        }

        public void OpenMobileMeeting(string url)
        {
            _goTop.SetActive(false);
            _goPanel.SetActive(true);
            webViewObject.transform.SetParent(_goPanel.transform);
            webViewObject.LoadURL(url);

            webViewObject.SetMargins(0, 0, 0, 0);
            webViewObject.SetCameraAccess(true);
            webViewObject.SetMicrophoneAccess(true);
            webViewObject.SetVisibility(true);
        }

        public void GoBack() 
        {
            if (webViewObject == null)
            {
                Debug.Log("WebviewController | object 가 null 인데...?");
                return;
            }

            if (webViewObject.CanGoBack())
                webViewObject.GoBack();
            else 
                Debug.Log("WebviewController | 첫 페이지 입니다.");
        }

        public void Close() => WebviewBuilder.singleton.RequestClear();

    #endregion  // Public functions
    }
}