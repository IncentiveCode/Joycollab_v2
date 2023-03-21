using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Joycollab.v2
{
    public class Clickable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private eClickableObjectType _objectType;
        [SerializeField] private eRendererType _rendererType;
        // [SerializeField] private PopupMenuTitle[] _menuItems;    // TODO. 추가 예정
        [SerializeField, Range(0, 1)] private float _alphaValueOnEnter;

        [Header("type : Building")]
        [SerializeField] private GameObject _goPopupBuilding;
        // [SerializeField] private BuildingData _soBuildingData;
        // [SerializeField] private ResBuildingInfo _buildingInfo;  // TODO. 추가 예정.
        // [SerializeField] private Image _imgTag;                  // TODO. 추가 예정.
        // [SerializeField] private bool _alwaysOpenTag;            // TODO. 추가 예정.

        [Header("type : Elevator")]
        [SerializeField] private GameObject _goElevatorMenu;

        [Header("type : Information")]
        [SerializeField] private GameObject _goInformation;
        [SerializeField] private int _floorNo;

        [Header("type : Board")] 
        [SerializeField] private GameObject _goBoard;

        [Header("type : Notice")]
        [SerializeField] private GameObject _goNotice;

        [Header("type : Dummy")]
        [SerializeField] private GameObject _goDummyMenu;

        // local variables
        private Image image;
        private SpriteRenderer spriteRenderer;
        private Color temp;


    #region Unity functions
        private void Awake() 
        {
            // TODO. 팝업 등을 출력할 canvas 와 scaler 설정

            switch (_rendererType) 
            {
                case eRendererType.UI_Image :
                    image = GetComponent<Image>();
                    temp = image.color;
                    temp.a = 0f;
                    image.color = temp;
                    break;

                case eRendererType.SpriteRenderer :
                    spriteRenderer = GetComponent<SpriteRenderer>();
                    temp = spriteRenderer.color;
                    temp.a = 0f;
                    spriteRenderer.color = temp;
                    break;

                default :
                    break;
            }

            // TODO. object type 에 따라 정보 설정.
            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                    break;

                default :
                    Debug.Log($"현재 Clickable object 의 타입 : {_objectType.ToString()}, 준비 중...");
                    break;
            }
        }
    #endregion


    #region Object setting functions
        private void SetBuildingInfo() 
        {

        }
    #endregion  // Object setting functions


    #region Click event handler 
        private void LeftClick(PointerEventData data) 
        {

        }

        private void RightClick(PointerEventData data) 
        {

        }

        private void WheelClick(PointerEventData data) 
        {

        }
    #endregion  // Click event handler


    #region Interface functions implementation (for UGUI Click)
        public void OnPointerEnter(PointerEventData data) 
        {
            if (_rendererType != eRendererType.UI_Image) return;

            // TODO. object type 에 따라 Pointer Enter 이벤트 처리.
            temp.a = _alphaValueOnEnter;
            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                    image.color = temp;

                    // TODO. 건물 이름표 출력할 경우 추가.

                    break;

                default :
                    Debug.Log($"현재 Clickable object 의 타입 : {_objectType.ToString()}, 준비 중...");
                    break;
            }
        }

        public void OnPointerExit(PointerEventData data) 
        {
            if (_rendererType != eRendererType.UI_Image) return;

            // TODO. object type 에 따라 Pointer Enter 이벤트 처리.
            temp.a = 0f;
            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                    image.color = temp;

                    // TODO. 건물 이름표 출력할 경우 추가.

                    break;

                default :
                    Debug.Log($"현재 Clickable object 의 타입 : {_objectType.ToString()}, 준비 중...");
                    break;
            }

        }

        public void OnPointerClick(PointerEventData data) 
        {
            if (_rendererType != eRendererType.UI_Image) return;

            // TODO. click 된 버튼에 따라 Pointer Enter 이벤트 처리.
            switch (data.button) 
            {
                case PointerEventData.InputButton.Left :

                    break;

                case PointerEventData.InputButton.Right :

                    break;

                case PointerEventData.InputButton.Middle :

                    break;

                default :

                    break;
            }
        }
    #endregion  // Interface functions implementation (for UGUI Click)


    #region Mouse enter, exit functions (without canvas)
        private void OnMouseEnter()
        {
            if (_rendererType != eRendererType.SpriteRenderer) return;

            // TODO. object type 에 따라 Pointer Enter 이벤트 처리.
            temp.a = _alphaValueOnEnter;
            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                    spriteRenderer.color = temp;

                    // TODO. 건물 이름표 출력할 경우 추가.

                    break;

                default :
                    Debug.Log($"현재 Clickable object 의 타입 : {_objectType.ToString()}, 준비 중...");
                    break;
            }
        }

        private void OnMouseExit() 
        {
            if (_rendererType != eRendererType.SpriteRenderer) return;

            // TODO. object type 에 따라 Pointer Enter 이벤트 처리.
            temp.a = 0f;
            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                    spriteRenderer.color = temp;

                    // TODO. 건물 이름표 출력할 경우 추가.

                    break;

                default :
                    Debug.Log($"현재 Clickable object 의 타입 : {_objectType.ToString()}, 준비 중...");
                    break;
            }
        }

        private void OnMouseDown() 
        {
            if (_rendererType != eRendererType.SpriteRenderer) return;
            
            Debug.Log("sprite renderer clicked.");
        }
    #endregion
    }
}