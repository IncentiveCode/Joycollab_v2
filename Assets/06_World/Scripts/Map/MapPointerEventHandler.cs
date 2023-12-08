/// <summary>
/// MapScene 에서 사용할 포인터 클릭 제어 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 08. 31 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 08. 31) : v1 에서 사용하던 항목 수정 후 적용.
///     v0.2 (2023. 12. 08) : class name 변경. (WorldPointerEventHandler -> MapPointerEventHandler)
/// </summary>

using UnityEngine;
using UnityEngine.EventSystems;

namespace Joycollab.v2
{
    public class MapPointerEventHandler : MonoBehaviour, IScrollHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private float fSize;


    #region interface implementations

        public void OnScroll(PointerEventData data) 
        {
            fSize = Input.GetAxis("Mouse ScrollWheel");
            MapCamera.singleton.HandleWheelEvent(fSize);
            return;
        }

        public void OnBeginDrag(PointerEventData data) 
        {
            MapCamera.singleton.HandleBeginDrag(data.position);
        }

        public void OnDrag(PointerEventData data) 
        {
            MapCamera.singleton.HandleDrag(data.position);
        }

        public void OnEndDrag(PointerEventData data) 
        {
            MapCamera.singleton.HandleDrag(data.position, true);
        }

    #endregion  // interface implementations
    }
}