/// <summary>
/// click 이 가능한 object 의 기능을 부여하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 11. 01
/// @version        : 0.4
/// @update         :
///     v0.1 (2023. 04. 19) : 최초 생성. v1 에서 사용했던 것들 가지고 와서 수정 후 적용.
/// 	v0.2 (2023. 09. 25) : world 에 적용하는 작업 시작.
/// 	v0.3 (2023. 10. 21) : Building, World Avatar 에 사용할 기능 적용.
///     v0.4 (2023. 11. 01) : summary 추가 및 기능 일부 정리.
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;

namespace Joycollab.v2
{
    public class Clickable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private const string TAG = "Clickable"; 
        
        [Header("common")]
        [SerializeField] private eClickableObjectType _objectType;
        [SerializeField] private eRendererType _rendererType;
        [SerializeField] private string[] _menuItems;
        [SerializeField, Range(0, 1)] private float _alphaValueOnEnter;
        [SerializeField, Range(0, 1)] private float _alphaValueOnExit;

        [Header("type : Building")]
        [SerializeField] private BuildingData _soBuildingData;   
        [SerializeField] private Image _imgTag;                  
        [SerializeField] private bool _alwaysOpenTag;            

        [Header("type : WorldAvatar")]
        private WorldAvatar _worldAvatarInfo;

        [Header("type : Elevator")]
        [SerializeField] private GameObject _goElevatorMenu;

        [Header("type : Information")]
        [SerializeField] private GameObject _goInformation;
        [SerializeField] private int _floorNo;

        [Header("type : Board")] 
        [SerializeField] private GameObject _goBoard;

        [Header("type : Notice")]
        [SerializeField] private GameObject _goNotice;

        [Header("type : Seminar")]
        [SerializeField] private GameObject _goSeminar;
        
        [Header("type : Meeting")]
        [SerializeField] private GameObject _goMeeting;

        [Header("type : Display")]
        [SerializeField] private GameObject _goDisplay;

        [Header("type : Dummy")]
        [SerializeField] private GameObject _goDummyMenu;

        // local variables
        private Image image;
        private SpriteRenderer spriteRenderer;
        private Color temp;


    #region Unity functions

        private void Start() 
        {
            switch (_rendererType) 
            {
                case eRendererType.UI_Image :
                    if (TryGetComponent<Image>(out image)) 
                    {
                        temp = image.color;
                        temp.a = _alphaValueOnExit;
                        image.color = temp;
                    }
                    break;

                case eRendererType.SpriteRenderer :
                    if (TryGetComponent<SpriteRenderer>(out spriteRenderer))
                    {
                        temp = spriteRenderer.color;
                        temp.a = _alphaValueOnExit;
                        spriteRenderer.color = temp;
                    }
                    break;

                default :
                    break;
            }

            // TODO. object type 에 따라 정보 설정.
            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                    SetBuildingInfo();
                    break;

                case eClickableObjectType.WorldAvatar :
                    SetWorldAvatarInfo();
                    break;

                default :
                    // Debug.Log($"현재 Clickable object 의 타입 : {_objectType.ToString()}, 준비 중...");
                    break;
            }
        }

    #endregion


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
                    if (_imgTag != null) 
                    {
                        _imgTag.gameObject.SetActive(true);
                    }
                    break;

                case eClickableObjectType.WorldAvatar :
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
            temp.a = _alphaValueOnExit;
            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                    image.color = temp;
                    if (_imgTag != null)
                    {
                        _imgTag.gameObject.SetActive(_alwaysOpenTag);
                    }
                    break;

                case eClickableObjectType.WorldAvatar :
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
            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                    if (data.button == PointerEventData.InputButton.Left)
                        BuildingLeftClick(data);
                    else if (data.button == PointerEventData.InputButton.Right)
                        BuildingRightClick(data);
                    else 
                        BuildingWheelClick(data);
                    break;

                case eClickableObjectType.WorldAvatar :
                    if (data.button == PointerEventData.InputButton.Right)
                        WorldAvatarRightClick(data);
                    break;

                case eClickableObjectType.Display :
                    if (data.button == PointerEventData.InputButton.Left)
                        DisplayLeftClick(data);
                    break;

                default :
                    Debug.Log($"현재 Clickable object 의 타입 : {_objectType.ToString()}, 클릭 처리 준비 중...");
                    break;
            }
        }

    #endregion  // Interface functions implementation (for UGUI Click)


    #region Mouse enter, exit functions (with SpriteRenderer)

        private void OnMouseEnter()
        {
            if (_rendererType != eRendererType.SpriteRenderer) return;

            temp.a = _alphaValueOnEnter;
            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                    spriteRenderer.color = temp;
                    break;

                case eClickableObjectType.Information :
                case eClickableObjectType.Board :
                case eClickableObjectType.Notice :
                case eClickableObjectType.Seminar :
                case eClickableObjectType.Display :
                case eClickableObjectType.Meeting :
                case eClickableObjectType.FileBox :
                    spriteRenderer.color = temp;
                    break;

                default :
                    Debug.Log($"현재 Clickable object 의 타입 : {_objectType.ToString()}, SpriteRenderer mouse enter");
                    break;
            }
        }

        private void OnMouseExit() 
        {
            if (_rendererType != eRendererType.SpriteRenderer) return;

            temp.a = _alphaValueOnExit;
            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                    spriteRenderer.color = temp;
                    break;

                case eClickableObjectType.Information :
                case eClickableObjectType.Board :
                case eClickableObjectType.Notice :
                case eClickableObjectType.Seminar :
                case eClickableObjectType.Display :
                case eClickableObjectType.Meeting :
                case eClickableObjectType.FileBox :
                    spriteRenderer.color = temp;
                    break;

                default :
                    Debug.Log($"현재 Clickable object 의 타입 : {_objectType.ToString()}, SpriteRenderer mouse exit");
                    break;
            }
        }

        private void OnMouseUp() 
        {
            if (_rendererType != eRendererType.SpriteRenderer) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;  

            PointerEventData data = new PointerEventData(EventSystem.current); 
            switch (_objectType) 
            {
                case eClickableObjectType.Display :
                    DisplayLeftClick(data);
                    break;
                
                case eClickableObjectType.Seminar:
                    if (data.button == PointerEventData.InputButton.Left)
                        SeminarLeftClick(data);
                    break;
                
                case eClickableObjectType.Meeting:
                    if (data.button == PointerEventData.InputButton.Left)
                        MeetingLeftClick(data);
                    break;
                
                default :
                    Debug.Log($"현재 Clickable object 의 타입 : {_objectType.ToString()}, SpriteRenderer clicked.");
                    break;
            }
        }

    #endregion


    #region about 'Building'

        private void SetBuildingInfo() 
        {
            if (_imgTag != null) 
            {
                _imgTag.gameObject.SetActive(_alwaysOpenTag);
            }
        }

        private void BuildingLeftClick(PointerEventData data) 
        {
            var pop = Instantiate(SystemManager.singleton.pfBuildingInfo, Vector3.zero, Quaternion.identity);
            if (pop.TryGetComponent<BuildingInfo>(out BuildingInfo popInfo)) 
            {
                var transform = GameObject.Find(S.Canvas_Popup).GetComponent<Transform>();
                pop.transform.SetParent(transform, false);
                popInfo.Open(_soBuildingData, data.position);
            }
            else 
            {
                Destroy(pop.gameObject);
            }
        }

        private void BuildingRightClick(PointerEventData data) 
        {
            if (_menuItems.Length == 0) 
            {
                Debug.LogError("이 건물에 등록된 메뉴가 하나도 없습니다.");
                return;
            }

            MenuController ctrl = MenuBuilder.singleton.Build();
            if (ctrl == null)
            {
                Debug.LogError("현재 scene 에 MenuBuilder instance 가 없습니다.");
                return;
            }

            ctrl.Init(_soBuildingData.BuildingName);
            foreach (var item in _menuItems) 
            {
                ctrl.AddMenu(item, () => {
                    switch (item) 
                    {
                        case S.MENU_DETAILS :
                            var pop = Instantiate(SystemManager.singleton.pfBuildingInfo, Vector3.zero, Quaternion.identity);
                            if (pop.TryGetComponent<BuildingInfo>(out BuildingInfo popInfo)) 
                            {
                                var transform = GameObject.Find(S.Canvas_Popup).GetComponent<Transform>();
                                pop.transform.SetParent(transform, false);
                                popInfo.Open(_soBuildingData, data.position);
                            }
                            else 
                            {
                                Destroy(pop.gameObject);
                            }
                            break;

                        case S.MENU_ENTER :
                            var manager = WorldNetworkManager.singleton;
                            manager.networkAddress = "dev.jcollab.com";
                            manager.StartClient();
                            break;
                        
                        case S.MENU_MOVE_TO_OFFICE :
                            if (_soBuildingData.UsingJoycollab)
                            {
                                JsLib.OpenWebview(_soBuildingData.JoycollabLink, "office");
                            }
                            else 
                            {
                                Debug.Log($"{TAG} | {_soBuildingData.BuildingName} 은 joycollab 주소가 없음.");
                            }
                            break;

                        default :
                            Debug.LogError($"아직 준비되지 않은 아이템 항목 : {item}");
                            break;
                    }
                });
            }
            ctrl.Open(data.position);
        }

        private void BuildingWheelClick(PointerEventData data) 
        {

        }
        
    #endregion  // about 'Building' 


    #region about 'World Avatar'

        private void SetWorldAvatarInfo() 
        {
            _worldAvatarInfo = GetComponent<WorldAvatar>();
        }

        private void WorldAvatarRightClick(PointerEventData data) 
        {
            if (_menuItems.Length == 0) 
            {
                Debug.LogError("이 아바타에 등록된 메뉴가 하나도 없습니다.");
                return;
            }

            MenuController ctrl = MenuBuilder.singleton.Build();
            if (ctrl == null)
            {
                Debug.LogError("현재 scene 에 MenuBuilder instance 가 없습니다.");
                return;
            }

            bool isMyMenu = (R.singleton.memberSeq == _worldAvatarInfo.avatarSeq);

            ctrl.Init(_worldAvatarInfo.avatarName);
            foreach (var item in _menuItems) 
            {
                if (string.IsNullOrEmpty(item)) continue;
                if (isMyMenu && item.Equals(S.MENU_CHAT)) continue;
                if (isMyMenu && item.Equals(S.MENU_CALL)) continue;
                if (isMyMenu && item.Equals(S.MENU_MEETING)) continue;

                ctrl.AddMenu(item, () => {
                    switch (item) 
                    {
                        case S.MENU_DETAILS :
                            PopupBuilder.singleton.OpenAlert(
                                LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "기능 준비 안내", R.singleton.CurrentLocale)
                            );
                            break;

                        case S.MENU_CHAT :
                            string chatLink = string.Format(URL.CHATVIEW_LINK, R.singleton.memberSeq, _worldAvatarInfo.avatarSeq, R.singleton.Region);
                            JsLib.OpenChat(chatLink, _worldAvatarInfo.avatarSeq);
                            break;

                        case S.MENU_CALL :
                            List<int> callTarget = new List<int> {
                                R.singleton.memberSeq,
                                _worldAvatarInfo.avatarSeq
                            };
                            SystemManager.singleton.CallOnTheSpot(callTarget).Forget();
                            break;

                        case S.MENU_MEETING :
                            List<int> meetingTarget = new List<int> { _worldAvatarInfo.avatarSeq };
                            SystemManager.singleton.MeetingOnTheSpot(meetingTarget).Forget();
                            break;
                        
                        default :
                            Debug.LogError($"아직 준비되지 않은 아이템 항목 : {item}");
                            break;
                    }
                });
            }
            ctrl.Open(data.position);
        }

    #endregion  // about 'World Avatar' 


    #region 'Seminar' Click event

        private void SeminarLeftClick(PointerEventData data) 
        {
            Debug.Log("seminar kiosk 가 클릭되었습니다. 세미나 정보를 출력합시다.");

            /**
            if(SquareSceneManager.Instance.seminarSelect != null)  //2023. 03. 29 박성일 - 세미나 선택창이 있으면 세미나 선택창 제거
                Destroy(SquareSceneManager.Instance.seminarSelect);
            
            SquareSceneManager.Instance.seminarSelect = Instantiate(_goSeminar);
            GameEvents.Instance.RequestMeetingNSeminarWorld(ID.MEETING_AND_SEMINAR, true);
             */
        }

    #endregion  // 'Seminar' Click event
    
    
    #region 'Meeting' Click event

        private void MeetingLeftClick(PointerEventData data) 
        {
            Debug.Log("meeting board 가 클릭되었습니다. 회의 정보를 출력합시다.");

            /**
            if(SquareSceneManager.Instance.meetingSelect != null)  //2023. 03. 29 박성일 - 세미나 선택창이 있으면 세미나 선택창 제거
                Destroy(SquareSceneManager.Instance.meetingSelect);
                
            SquareSceneManager.Instance.meetingSelect = Instantiate(_goMeeting);
            GameEvents.Instance.RequestMeetingNSeminarWorld(ID.MEETING_AND_SEMINAR, false);
            */
        }

    #endregion  // 'Meeting' Click event


    #region 'Display' Click event

        private void DisplayLeftClick(PointerEventData data) 
        {
            Debug.Log("display 가 클릭되었습니다. 광고 영역을 출력합시다.");
        }

    #endregion  // 'Display' Click event
    }
}