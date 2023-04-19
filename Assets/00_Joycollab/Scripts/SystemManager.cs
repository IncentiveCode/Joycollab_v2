/// <summary>
/// Joycollab 통합 매니저 클래스 
/// - singleton 남용을 막고, 기존 manager 클래스들에서 중복되어 있는 내용들을 수정/정리/최적화 하기 위해 작성.
/// @author         : HJ Lee
/// @last update    : 2023. 04. 19
/// @version        : 0.2
/// @update
///     v0.1 (2023. 04. 07) : 최초 작성.
///     v0.2 (2023. 04. 19) : singleton pattern 수정, LocaleManager 추가.
/// </summary>

using UnityEngine;

namespace Joycollab.v2
{
    public class SystemManager : MonoBehaviour
    {
        public static SystemManager singleton { get; private set; }

        [Header("Manager classes")]
        [SerializeField] private LocaleManager localeManager; 


    #region Unity functions
        private void Awake() 
        {
            InitSingleton();     
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
            return;
        }

    #endregion  // Initialize

    #region First Act (공지사항 확인 후 URL parsing)

    #endregion  // First Act (공지사항 확인 후 URL parsing)


    #region Popup

    #endregion  // Popup
    }
}