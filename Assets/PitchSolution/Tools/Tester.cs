using UnityEngine;
using UnityEngine.UI;

namespace PitchSolution
{
    public class Tester : MonoBehaviour
    {
        [Header("JsLib Test")]
        [SerializeField] private Button _btnAlert;
        [SerializeField] private Button _btnLog;
        [SerializeField] private Button _btnRedirect;
        [SerializeField] private Button _btnOpenWebview;
        [SerializeField] private Button _btnCheckBrowser;
        [SerializeField] private Button _btnCheckSystem;

        // local variables
        private string objectName;

    #region Unity functions
        private void Awake()
        {
            // set button listener
            _btnAlert.onClick.AddListener(() => JsLib.Alert("help me."));
            _btnLog.onClick.AddListener(() => JsLib.Log("what the..."));
            _btnRedirect.onClick.AddListener(() => JsLib.Redirection("https://incentivecode.blogspot.com"));
            _btnOpenWebview.onClick.AddListener(() => JsLib.OpenWebview("https://incentivecode.blogspot.com", "Home"));
            _btnCheckBrowser.onClick.AddListener(() => JsLib.CheckBrowser(objectName, "PostCheckcBrowser"));
            _btnCheckSystem.onClick.AddListener(() => JsLib.CheckSystem(objectName, "PostCheckSystem"));

            // set local variables
            objectName = gameObject.name;
        }
    #endregion

    #region JsLib Callback functions
        public void PostCheckcBrowser(string result) 
        {
            JsLib.Alert($"Browser check result : {result}");
        }

        public void PostCheckSystem(string result) 
        {
            JsLib.Alert($"System check result : {result}");
        }
    #endregion
    }
}