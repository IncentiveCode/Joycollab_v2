/// <summary>
/// [world]
/// 모임방 리스트 > 카테고리 항목 Script
/// @author         : HJ Lee
/// @last update    : 2023. 10. 04
/// @version        : 0.1
/// @update
///     v0.1 (2023. 10. 04) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2
{
    public class CategoryItem : InfiniteScrollItem, IPointerEnterHandler, IPointerExitHandler
    {
        private const string TAG = "CategoryItem";

        [Header("text")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _imgCheck;
        [SerializeField] private TMP_Text _text;

        // local variables
        private RoomCategoryData data;
        private ToggleGroup group;
        private RectTransform rect;
        private bool isSelect;


    #region Unity functions

        private void Awake() 
        {
            rect = GetComponent<RectTransform>();
            _button.onClick.AddListener(OnSelect);
        }

    #endregion  // Unity functions
    

    #region GPM functions

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);

            data = (RoomCategoryData) scrollData;

            // 문구 정리
            _text.text = data.info.nm;
            _text.ForceMeshUpdate();

            // select image
            _imgCheck.gameObject.SetActive(false);

            // 크기 정리
            Vector2 size = rect.sizeDelta;
            Vector2 textSize = _text.GetRenderedValues(false);
            size.x = textSize.x + 16f;
            SetSize(size);
            OnUpdateItemSize();

            // toggle group 정리
            /**
            if (_toggle.group == null) 
            {
                if (transform.parent.TryGetComponent<ToggleGroup>(out group)) 
                {
                    _toggle.group = group;
                }
                else 
                {
                    Debug.Log($"{TAG} | Toggle Group 을 지정할 수 없음.");
                }
            }
             */
        }

    #endregion  // GPM functions


    #region Pointer event implementation

        public void OnPointerEnter(PointerEventData data) 
        {
            _imgCheck.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData data) 
        {
            _imgCheck.gameObject.SetActive(false);
        }

    #endregion  // Pointer event implementation
    }
}