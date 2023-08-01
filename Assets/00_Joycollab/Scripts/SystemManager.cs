/// <summary>
/// Joycollab 통합 매니저 클래스 
/// - singleton 남용을 막고, 기존 manager 클래스들에서 중복되어 있는 내용들을 수정/정리/최적화 하기 위해 작성.
/// @author         : HJ Lee
/// @last update    : 2023. 08. 01
/// @version        : 0.3
/// @update
///     v0.1 (2023. 04. 07) : 최초 작성.
///     v0.2 (2023. 04. 19) : singleton pattern 수정
///     v0.3 (2023. 08. 01) : language, text 관련 초기화 추가
/// </summary>

using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Joycollab.v2
{
    public class SystemManager : MonoBehaviour
    {
        public static SystemManager singleton { get; private set; }


    #region Unity functions

        private void Awake() 
        {
            InitSingleton();     

            // language, text 관련된 것만 정리.
            R.singleton.Init();

            Debug.Log("current language : "+ Application.systemLanguage);
            switch (Application.systemLanguage) 
            {
                case SystemLanguage.English :
                    R.singleton.ChangeLocale(ID.LANGUAGE_ENGLISH);
                    break;

                case SystemLanguage.Japanese :
                    R.singleton.ChangeLocale(ID.LANGUAGE_JAPANESE);
                    break;

                case SystemLanguage.Korean :
                default :
                    R.singleton.ChangeLocale(ID.LANGUAGE_KOREAN);
                    break;
            }
            SetFontOpt(0);
        }

        private void Start() 
        {
        #if UNITY_WEBGL && !UNITY_EDITOR 
            Application.targetFrameRate = -1;
        #else
            Application.targetFrameRate = 30;
        #endif
        }

    #endregion  // Unity functions


    #region Initialize

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

    #endregion  // Initialize


    #region First Act (공지사항 확인 후 URL parsing)

        // TODO. 

    #endregion  // First Act (공지사항 확인 후 URL parsing)


    #region Temp

        public void SetFontOpt(int opt) 
        {
            R.singleton.FontSizeOpt = opt;
        }

        public void Exit() 
        {
            SceneLoader.Load(eScenes.Login);
        }

    #endregion  // Temp
    }
}