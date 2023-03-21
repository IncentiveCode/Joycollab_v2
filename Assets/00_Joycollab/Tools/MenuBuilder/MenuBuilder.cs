/// <summary>
/// 메뉴 생성 툴
/// @author         : HJ Lee
/// @last update    : 2023. 03. 06 
/// @version        : 1.0
/// @update
///     - v1.0 (2023. 03. 06) : 최초 생성
/// </summary>

using UnityEngine;

namespace Joycollab.v2
{
    public class MenuBuilder : MonoBehaviour
    {
        public static MenuBuilder Instance;
        private const string TAG = "PopupMenu";

        [SerializeField, Tooltip("for test")] private Transform canvas;
        [SerializeField] private GameObject _goMenu;


    #region Unity functions
        private void Awake() 
        {
            Instance = this;
            canvas = GameObject.Find(S.POPUP_CANVAS).GetComponent<Transform>();
        }
    #endregion  // Unity function


    #region Public function
        public MenuController Build() 
        {
            if (_goMenu == null || canvas == null) return null;
            Clear();

            var view = Instantiate(_goMenu, Vector3.zero, Quaternion.identity);
            var lib = view.GetComponent<MenuController>();
            view.transform.SetParent(canvas, false);
            return lib;
        }
    #endregion  // Public function


    #region Private function
        private void Clear() 
        {
            Transform children = canvas.GetComponentInChildren<Transform>();
            foreach (Transform child in children) 
            {
                if (child.CompareTag(TAG)) 
                {
                    Destroy(child.gameObject);
                }
            }
        }
    #endregion  // Private function
    }
}