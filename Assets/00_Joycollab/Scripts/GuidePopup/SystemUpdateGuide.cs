using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Joycollab.v2
{
    public class SystemUpdateGuide : MonoBehaviour
    {
        private const string TAG = "SystemUpdateGuide";

        [Header("common")]
        [SerializeField] private Text _txtTitle;
        [SerializeField] private Text _txtContent;
        
        [Header("test door")]
        [SerializeField] private Button _btnForTest;


        public void Init(string title, string content) 
        {
            // set content text
            _txtTitle.text = title;
            _txtContent.text = content;


            // set button listener
            _btnForTest.onClick.AddListener(() => {
                PopupBuilder.singleton.OpenPrompt(
                    title : "패스워드 확인",
                    content : string.Empty,
                    (value) => {
                        if (value.Equals(S.SYSTEM_UPDATE_CODE)) 
                        {
                            Debug.Log($"{TAG} | 테스트를 진행합니다.");
                            JsLib.SetCookie(Key.SYSTEM_UPDATE_FLAG, S.TRUE);
                            SceneLoader.Load(eScenes.SignIn);
                        }
                        else 
                        {
                            string alert = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "비밀번호 불일치", R.singleton.CurrentLocale);
                            PopupBuilder.singleton.OpenAlert(alert);
                        }
                    },
                    isPassword : true
                );  
            });
        }
    }
}