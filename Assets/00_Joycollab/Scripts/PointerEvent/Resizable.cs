/// <summary>
/// Window Panel 의 크기 조정 기능을 부여하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 11. 01
/// @version        : 0.1
/// @update         :
///     v0.1 (2023. 11. 01) : 최초 생성. v1 에서 사용했던 것들 가지고 와서 수정 후 적용.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Joycollab.v2
{
    public class Resizable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private const string TAG = "Resizable";

        [Header("common")]
        [SerializeField] private Texture2D _cursor;

        [Header("option")]
        [SerializeField] private eDirection _direction;
        [SerializeField] private bool _isTop;

        // local variables
        private WindowView window;
        private RectTransform rect;
        private CanvasScaler canvasScaler;

        private float minWidth, minHeight;
        private float width, height, halfWidth, halfHeight, ratio;
        private Vector2 v2Pivot, v2Anchor, v2TempAnchor;
        private Vector2 v2Max;
        private Vector2 v2Hotspot;


    #region readonly info

        private readonly Vector2 v2Top = new Vector2(0.5f, 1f);
        private readonly Vector2 v2TopRight = new Vector2(1f, 1f);
        private readonly Vector2 v2Right = new Vector2(1f, 0.5f);
        private readonly Vector2 v2BottomRight = new Vector2(1f, 0f);
        private readonly Vector2 v2Bottom = new Vector2(0.5f, 0f);
        private readonly Vector2 v2BottomLeft = new Vector2(0f, 0f);
        private readonly Vector2 v2Left = new Vector2(0f, 0.5f);
        private readonly Vector2 v2TopLeft = new Vector2(0f, 1f);

    #endregion  // readonly info


    #region Unity functions

        private void Awake() 
        {
            if (! transform.parent.parent.TryGetComponent<WindowView>(out window)) 
            {
                Debug.Log($"{TAG} | WindowView 를 확인할 수 없습니다.");
                return;
            }
            minWidth = window.MinWidth;
            minHeight = window.MinHeight;

            if (! transform.parent.parent.TryGetComponent<RectTransform>(out rect))
            {
                Debug.Log($"{TAG} | Rect Transform 을 확인할 수 없습니다.");
                return;
            }

            width = height = halfHeight = halfHeight = ratio = 0f;
            v2Pivot = v2Anchor = v2TempAnchor = Vector2.zero;
            v2Max = Vector2.zero;
            v2Hotspot = Vector2.zero;
        } 

        private void OnDisable() 
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

    #endregion  // Unity functions


    #region Interface implementations

        public void OnBeginDrag(PointerEventData eventData) 
        {
            width = rect.sizeDelta.x;
            halfWidth = width / 2;
            height = rect.sizeDelta.y;
            halfHeight = height / 2;

            v2Anchor = rect.anchoredPosition;
            v2Pivot = rect.pivot;

            switch (_direction) 
            {
                case eDirection.Top:
                    v2TempAnchor = v2Bottom;

                    switch (v2Pivot.x)
                    {
                        case 0f:    v2Anchor.x += halfWidth;    break;
                        case 1f:    v2Anchor.x -= halfWidth;    break;
                    }
                    switch (v2Pivot.y)
                    {
                        case 0.5f:  v2Anchor.y -= halfHeight;   break;
                        case 1f:    v2Anchor.y -= height;       break;
                    }
                    break;

                case eDirection.TopRight:
                    v2TempAnchor = v2BottomLeft;

                    switch (v2Pivot.x)
                    {
                        case 0.5f:  v2Anchor.x -= halfWidth;    break;
                        case 1f:    v2Anchor.x -= width;        break;
                    }
                    switch (v2Pivot.y)
                    {
                        case 0.5f:  v2Anchor.y -= halfHeight;   break;
                        case 1f:    v2Anchor.y -= height;       break;
                    }
                    break;

                case eDirection.Right:
                    v2TempAnchor = v2Left;

                    switch (v2Pivot.x)
                    {
                        case 0.5f:  v2Anchor.x -= halfWidth;    break;
                        case 1f:    v2Anchor.x -= width;        break;
                    }
                    switch (v2Pivot.y)
                    {
                        case 0f:    v2Anchor.y += halfHeight;   break;
                        case 1f:    v2Anchor.y -= halfHeight;   break;
                    }
                    break;

                case eDirection.BottomRight:
                    v2TempAnchor = v2TopLeft;

                    switch (v2Pivot.x)
                    {
                        case 0.5f:  v2Anchor.x -= halfWidth;    break;
                        case 1f:    v2Anchor.x -= width;        break;
                    }
                    switch (v2Pivot.y)
                    {
                        case 0f:    v2Anchor.y += height;       break;
                        case 0.5f:  v2Anchor.y += halfHeight;   break;
                    }
                    break;

                case eDirection.Bottom:
                    v2TempAnchor = v2Top;
                    switch (v2Pivot.x)
                    {
                        case 0f:    v2Anchor.x += halfWidth;    break;
                        case 1f:    v2Anchor.x -= halfWidth;    break;
                    }
                    switch (v2Pivot.y)
                    {
                        case 0f:    v2Anchor.y += height;       break;
                        case 0.5f:  v2Anchor.y += halfHeight;   break;
                    }
                    break;

                case eDirection.BottomLeft:
                    v2TempAnchor = v2TopRight;

                    switch (v2Pivot.x)
                    {
                        case 0f:    v2Anchor.x += width;        break;
                        case 0.5f:  v2Anchor.x += halfWidth;    break;
                    }
                    switch (v2Pivot.y)
                    {
                        case 0f:    v2Anchor.y += height;       break;
                        case 0.5f:  v2Anchor.y += halfHeight;   break;
                    }
                    break;

                case eDirection.Left:
                    v2TempAnchor = v2Right;

                    switch (v2Pivot.x)
                    {
                        case 0f:    v2Anchor.x += width;        break;
                        case 0.5f:  v2Anchor.x += halfWidth;    break;
                    }
                    switch (v2Pivot.y)
                    {
                        case 0f:    v2Anchor.y += halfHeight;   break;
                        case 1f:    v2Anchor.y -= halfHeight;   break;
                    }
                    break;

                case eDirection.TopLeft:
                    v2TempAnchor = v2BottomRight;

                    switch (v2Pivot.x)
                    {
                        case 0f:    v2Anchor.x += width;        break;
                        case 0.5f:  v2Anchor.x += halfWidth;    break;
                    }
                    switch (v2Pivot.y)
                    {
                        case 0.5f:  v2Anchor.y -= halfHeight;   break;
                        case 1f:    v2Anchor.y -= height;       break;
                    }
                    break;    

                default :
                    Debug.Log($"{TAG} | OnBeginDrag(), direction 설정이 none 입니다.");
                    break;
            }

            // apply pivot & position
            rect.pivot = v2TempAnchor;
            rect.anchoredPosition = v2Anchor;
            rect.SetAsLastSibling();

            // calcurate ratio
            if (! window.transform.parent.TryGetComponent<CanvasScaler>(out canvasScaler)) 
            {
                Debug.Log($"{TAG} | canvas scaler 를 얻을 수 없습니다.");
                return;
            }
            ratio = Util.CalculateScalerRatio(canvasScaler);
            v2Max = new Vector2((float)(Screen.width / ratio), (float)(Screen.height / ratio));
        }

        public void OnDrag(PointerEventData eventData) 
        {
            switch (_direction) 
            {
                case eDirection.Top :
                    height += (eventData.delta.y / ratio);
                    break;

                case eDirection.TopRight:
                    width += (eventData.delta.x / ratio);
                    height += (eventData.delta.y / ratio);
                    break;

                case eDirection.Right:
                    width += (eventData.delta.x / ratio);
                    break;

                case eDirection.BottomRight:
                    width += (eventData.delta.x / ratio); 
                    height -= (eventData.delta.y / ratio);
                    break;

                case eDirection.Bottom:
                    height -= (eventData.delta.y / ratio);
                    break;

                case eDirection.BottomLeft:
                    width -= (eventData.delta.x / ratio);
                    height -= (eventData.delta.y / ratio);
                    break;

                case eDirection.Left:
                    width -= (eventData.delta.x / ratio);
                    break;

                case eDirection.TopLeft:
                    width -= (eventData.delta.x / ratio);
                    height += (eventData.delta.y / ratio);
                    break; 

                default :
                    Debug.Log($"{TAG} | OnDrag(), direction 설정이 none 입니다.");
                    break;
            }

            // apply 'size' 
            rect.sizeDelta = new Vector2(
                Mathf.Clamp(width, minWidth, v2Max.x), 
                Mathf.Clamp(height, minHeight, v2Max.y)
            );
        }
        
        public void OnEndDrag(PointerEventData eventData) 
        {
            // apply 'size'
            width = Mathf.Clamp(rect.sizeDelta.x, minWidth, v2Max.x);
            halfWidth = width / 2;
            height = Mathf.Clamp(rect.sizeDelta.y, minHeight, v2Max.y);
            halfHeight = height / 2;
            rect.sizeDelta = new Vector2(width, height);

            // reset 'pivot'
            v2Pivot = rect.pivot;
            switch (v2Pivot.x)
            {
                case 0.5f:  v2Anchor.x -= halfWidth;    break;
                case 1f:    v2Anchor.x -= width;        break;
            }
            switch (v2Pivot.y)
            {
                case 0.5f:  v2Anchor.y -= halfHeight;   break;
                case 1f:    v2Anchor.y -= height;       break;
            }
            rect.pivot = Vector2.zero;

            // apply position
            Vector2 temp = new Vector2(
                Mathf.Clamp(v2Anchor.x, (-width * 0.75f), v2Max.x - width),
                Mathf.Clamp(v2Anchor.y, (-height * 0.75f), v2Max.y - height)
            );
            rect.anchoredPosition = temp;
        }

        public void OnPointerEnter(PointerEventData eventData) 
        {
            if (_cursor == null) return;

            v2Hotspot = new Vector2(_cursor.width / 2, _cursor.height / 2);
            // Cursor.SetCursor(_cursor, v2Hotspot, CursorMode.ForceSoftware);
            Cursor.SetCursor(_cursor, v2Hotspot, CursorMode.Auto);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

    #endregion  // Interface implementations
    }
}