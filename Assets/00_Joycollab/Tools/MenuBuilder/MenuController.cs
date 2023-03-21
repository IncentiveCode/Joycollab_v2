/// <summary>
/// MenuBuilder 로 생성한 메뉴 제어 스크립트
/// @author         : HJ Lee
/// @last update    : 2023. 03. 06 
/// @version        : 1.0
/// @update
///     - v1.0 (2023. 03. 06) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;
// using Assets.SimpleLocalization;

namespace Joycollab.v2
{
    public class MenuController : MonoBehaviour
    {
        private const string TAG = "PopupMenuItem";

        [Header("area, title")]        
        [SerializeField] private Image _imgPanel;
        [SerializeField] private TMP_Text _TxtTitle;

        [Header("menu item")]        
        [SerializeField] private Transform _transformItems;
        [SerializeField] private GameObject _goMenuItem;

        // area check
        private Vector2 v2Cursor;
        private RectTransform rect;

        // canvas scaler, ratio
        private CanvasScaler scaler;
        private float ratio;


    #region Unity functions
        private void Awake() 
        {
            _imgPanel.alphaHitTestMinimumThreshold = 0.1f;
            rect = _imgPanel.GetComponent<RectTransform>();

            Clear();
        }

        private void Update() 
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2)) 
            {
                v2Cursor = rect.InverseTransformPoint(Input.mousePosition);
                if (! rect.rect.Contains(v2Cursor)) Close();
            }
        }
    #endregion


    #region Public functions
        public void Init(string title) 
        {
            scaler = transform.parent.GetComponent<CanvasScaler>();
            ratio = Util.CalculateScalerRatio(scaler);

            _TxtTitle.text = title;
        }

        public void AddMenu(string itemName, System.Action func) 
        {
            var item = Instantiate(_goMenuItem, Vector3.zero, Quaternion.identity);
            item.GetComponent<MenuItem>().ItemName = itemName;
            item.GetComponent<Button>().onClick.AddListener(() => {
                func?.Invoke(); 
                Close();
            });
            item.transform.SetParent(_transformItems, false);
        }

        public void AddMenu<T>(T enumItem, System.Action func) 
        {
            var item = Instantiate(_goMenuItem, Vector3.zero, Quaternion.identity);

            string key = $"Menu.{Util.EnumToString(enumItem)}";
            // string itemName = LocalizationManager.Localize(key);
            string itemName = Util.EnumToString(enumItem);
            item.GetComponent<MenuItem>().ItemName = itemName;

            item.GetComponent<Button>().onClick.AddListener(() => {
                func?.Invoke(); 
                Close();
            });
            item.transform.SetParent(_transformItems, false);
        }

        public void Open(Vector2 pos) 
        {
            Vector2 temp = Vector2.zero;
            float maxX = (float) (Screen.width / ratio);
            float maxY = (float) (Screen.height / ratio);
            float panelX = rect.sizeDelta.x;
            float panelY = rect.sizeDelta.y;

            temp.x = Mathf.Clamp(pos.x, 0, maxX - panelX);
            temp.y = Mathf.Clamp(pos.y, 0, maxY - panelY);
            rect.anchoredPosition = temp;

            gameObject.SetActive(true);
        }
    #endregion


    #region Private functions
        private void Clear() 
        {
            Transform children = _transformItems.GetComponentInChildren<Transform>();    
            foreach (Transform child in children) 
            {
                if (child.CompareTag(TAG)) 
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private void Close() => Destroy(gameObject);
    #endregion  // Private functions
    }
}