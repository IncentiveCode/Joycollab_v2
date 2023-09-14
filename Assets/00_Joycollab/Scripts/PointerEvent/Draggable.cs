/// <summary>
/// Window Panel 이동을 위한 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 03. 15
/// @version        : 1.0
/// @update
///     v1.0 (2023. 03. 15) : 최초 생성, Joycollab & TechnoPark 등 작업을 하면서 작성한 것들을 수정 및 적용
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Joycollab.v2
{
    public class Draggable : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IPointerUpHandler 
    {
        private const string TAG = "Draggable";

        [Header("panel option")]
        [SerializeField] private CanvasScaler scaler;
        [SerializeField] private RectTransform rect;
        [SerializeField] private bool m_isMovable;

        // local variables
        private Vector2 v2Max, v2Offset, v2Temp;
        private float fRatio, fWidth, fHeight;


    #region Unity functions

        private void Start() 
        {
            // set rect transform
            if (rect == null) 
            {
                rect = transform.parent.GetComponent<RectTransform>();
            }
            if (rect == null) 
            {
                Debug.Log($"{TAG} | RectTransform 설정이 누락되었습니다.");
            }

            // set canvas scaler
            if (scaler == null) 
            {
                scaler = transform.parent.parent.GetComponent<CanvasScaler>();
            }
            if (scaler == null) 
            {
                Debug.Log($"{TAG} | CanvasScaler 설정이 누락되었습니다.");
            }
        }

    #endregion  // Unity functions


    #region Interface implementations

        public void OnPointerDown(PointerEventData data) 
        {
            if (rect == null || scaler == null) return;

            rect.SetAsLastSibling();
            fRatio = Util.CalculateScalerRatio(scaler);

            v2Offset = rect.anchoredPosition - (data.position / fRatio);
            v2Max.x = (float) (Screen.width / fRatio);
            v2Max.y = (float) (Screen.height / fRatio);
        }

        public void OnDrag(PointerEventData data) 
        {
            if (rect == null || scaler == null) return;
            if (! m_isMovable) return;

            rect.anchoredPosition = (data.position / fRatio) + v2Offset;
        }

        public void OnEndDrag(PointerEventData data) 
        {
            if (rect == null || scaler == null) return;
            if (! m_isMovable) return;
            
            fWidth = rect.sizeDelta.x;
            fHeight = rect.sizeDelta.y;

            v2Temp = (data.position / fRatio) + v2Offset;
            v2Temp.x = Mathf.Clamp(v2Temp.x, (-fWidth * 0.5f), v2Max.x - (fWidth * 0.5f));
            v2Temp.y = Mathf.Clamp(v2Temp.y, (-fHeight * 0.5f), v2Max.y - fHeight - 40f);

            rect.anchoredPosition = v2Temp;
        }

        public void OnPointerUp(PointerEventData data) 
        {
            // TODO. 대기 중. 추후 카메라 이동과 연결지을 예정.
        }

    #endregion  // Interface implementations
    }
}