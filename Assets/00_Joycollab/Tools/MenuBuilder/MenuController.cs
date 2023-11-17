/// <summary>
/// MenuBuilder 로 생성한 메뉴 제어 스크립트
/// @author         : HJ Lee
/// @last update    : 2023. 10. 06 
/// @version        : 0.2
/// @update
///     v0.1 (2023. 03. 06) : 최초 생성
///     v0.2 (2023. 10. 06) : 기능 수정 및 테스트 진행
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;

namespace Joycollab.v2
{
    public class MenuController : MonoBehaviour
    {
        private const string TAG = "MenuController";
        private const string TABLE = "Menu";

        [Header("area, title")]        
        [SerializeField] private Image _imgPanel;
        [SerializeField] private TMP_Text _txtTitle;

        [Header("menu item")]        
        [SerializeField] private Transform _transformItems;
        [SerializeField] private GameObject _goMenuItem;

        // area check
        private Vector2 v2Cursor;
        private RectTransform rect;

        // canvas scaler, ratio
        private CanvasScaler scaler;
        private float ratio;


    #region Unity function

        private void Awake() 
        {
            // _imgPanel.alphaHitTestMinimumThreshold = 0.1f;
            rect = _imgPanel.GetComponent<RectTransform>();

            Clear();
        }

        private void Update() 
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2) || Input.GetAxis("Mouse ScrollWheel") != 0) 
            {
                v2Cursor = rect.InverseTransformPoint(Input.mousePosition);
                if (! rect.rect.Contains(v2Cursor)) Close();
            }
        }

    #endregion  // Unity function


    #region Public functions

        public void Init(string title) 
        {
            scaler = transform.parent.GetComponent<CanvasScaler>();
            _txtTitle.text = title;
        }

        public void AddMenu(string itemName, System.Action func) 
        {
            var item = Instantiate(_goMenuItem, Vector3.zero, Quaternion.identity);
            var sc = item.GetComponent<MenuItem>();
            sc.Init(itemName);
            item.GetComponent<Button>().onClick.AddListener(() => {
                func?.Invoke(); 
                Close();
            });
            item.transform.SetParent(_transformItems, false);
        }

        public void AddMenu<T>(T enumItem, System.Action func) 
        {
            var item = Instantiate(_goMenuItem, Vector3.zero, Quaternion.identity);
            var sc = item.GetComponent<MenuItem>();
            sc.Init(Util.EnumToString(enumItem));
            item.GetComponent<Button>().onClick.AddListener(() => {
                func?.Invoke(); 
                Close();
            });
            item.transform.SetParent(_transformItems, false);
        }

        public void Open(Vector2 pos) 
        {
            ratio = Util.CalculateScalerRatio(scaler);
            Vector2 v2Temp = (pos / ratio);

            float maxX = (float) (Screen.width / ratio);
            float maxY = (float) (Screen.height / ratio);
            Vector2 sizeDelta = rect.sizeDelta;
            float panelX = sizeDelta.x;
            float panelY = sizeDelta.y;

            v2Temp.x = Mathf.Clamp(v2Temp.x, 0, maxX - panelX);
            v2Temp.y = Mathf.Clamp(v2Temp.y - panelY, 0, maxY - panelY);
            rect.anchoredPosition = v2Temp;

            gameObject.SetActive(true);
        }

    #endregion


    #region Private functions

        private void Clear() 
        {
            Transform children = _transformItems.GetComponentInChildren<Transform>();    
            foreach (Transform child in children) 
            {
                if (child.name.Equals(_transformItems.name) || child.name.Equals(_txtTitle.name)) continue;
                Destroy(child.gameObject);
            }
        }

        private void Close() => Destroy(gameObject);

    #endregion  // Private functions
    }
}