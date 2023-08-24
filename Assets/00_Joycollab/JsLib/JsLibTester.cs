/// <summary>
/// Custom JsLib 테스트를 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 08. 23
/// @version        : 0.3
/// @update
///     v0.1 (2023. 02. 27) : 최초 생성, 간단 기능만 추가한 Tester 구현.
///     v0.2 (2023. 03. 14) : WebGLSupport package 에 있는 Focus, Blur 테스트.
///     v0.3 (2023. 08. 24) : OnFocus(), OnBlur() SystemManager 로 이관.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Joycollab.v2
{
    public class JsLibTester : MonoBehaviour
    {
        [Header("Simple Test")]
        [SerializeField] private TMP_InputField _inputAlertX;
        [SerializeField] private TMP_InputField _inputAlertY;
        [SerializeField] private Button _btnAlert;
        [SerializeField] private Button _btnLog;
        [SerializeField] private Button _btnRedirect;
        [SerializeField] private Button _btnOpenWebview;
        [SerializeField] private Button _btnCheckBrowser;
        [SerializeField] private Button _btnCheckSystem;

        [Header("Cookie Test")]
        [SerializeField] private TMP_InputField _inputValue;
        [SerializeField] private Button _btnWriteCookie;
        [SerializeField] private Button _btnReadCookie;

        // local variables
        private string objectName;


    #region Unity functions

        private void Awake()
        {
            // set button listener
            // _btnAlert.onClick.AddListener(() => JsLib.Alert("Joycollab JsLib tester."));
            _btnAlert.onClick.AddListener(() => {
                int x = 0, y = 0;
                int.TryParse(_inputAlertX.text, out x);
                int.TryParse(_inputAlertY.text, out y);
                PopupBuilder.singleton.OpenAlert("Joycollab JsLib tester.", x, y);
            });
            _btnLog.onClick.AddListener(() => JsLib.Log("Console.log"));
            _btnRedirect.onClick.AddListener(() => JsLib.Redirection("https://jcollab.com"));
            _btnOpenWebview.onClick.AddListener(() => JsLib.OpenWebview("https://jcollab.com", "Home"));
            _btnCheckBrowser.onClick.AddListener(() => JsLib.CheckBrowser(objectName, "PostCheckcBrowser"));
            _btnCheckSystem.onClick.AddListener(() => JsLib.CheckSystem(objectName, "PostCheckSystem"));

            _btnWriteCookie.onClick.AddListener(() => JsLib.SetCookie("ps", _inputValue.text));
            _btnReadCookie.onClick.AddListener(() => {
                string cookie = JsLib.GetCookie("ps");
                JsLib.Alert($"Read cookie. \nkey : 'ps', value : {cookie}");
            });


            // set local variables
            objectName = gameObject.name;
        }

    #endregion


    #region JsLib Callback functions

        public void PostCheckcBrowser(string result) 
        {
            var arr = result.Split('|');
            JsLib.Alert($"Browser check result.\nChrome based browser : {arr[0]}, Default language : {arr[1]}");
        }

        public void PostCheckSystem(string result) 
        {
            JsLib.Alert($"System check result : {result}");
        }

    #endregion
    }
}