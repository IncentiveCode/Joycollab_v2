/// <summary>
/// click 이 가능한 object 의 기능을 부여하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 11. 16
/// @version        : 0.6
/// @update         :
///     v0.1 (2023. 04. 19) : 최초 생성. v1 에서 사용했던 것들 가지고 와서 수정 후 적용.
/// 	v0.2 (2023. 09. 25) : world 에 적용하는 작업 시작.
/// 	v0.3 (2023. 10. 21) : Building, World Avatar 에 사용할 기능 적용.
///     v0.4 (2023. 11. 01) : summary 추가 및 기능 일부 정리.
///     v0.5 (2023. 11. 14) : alpha value 대신 enter, exit color 추가. 
///     v0.6 (2023. 11. 16) : webview 를 출력해야 하는 eClickableObjectType 을 모두 link 로 통폐합.
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using Mirror;

namespace Joycollab.v2
{
    public class Clickable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private const string TAG = "Clickable"; 
        
        [Header("common")]
        [SerializeField] private eClickableObjectType _objectType;
        [SerializeField] private eRendererType _rendererType;
        [SerializeField] private string[] _menuItems;
        [SerializeField] private Color _colorOnEnter;
        // [SerializeField, Range(0, 1)] private float _alphaValueOnEnter;
        [SerializeField] private Color _colorOnExit;
        // [SerializeField, Range(0, 1)] private float _alphaValueOnExit;

        [Header("type : Building")]
        [SerializeField] private BuildingData _soBuildingData;   
        [SerializeField] private Image _imgTag;                  
        [SerializeField] private bool _alwaysOpenTag;            

        [Header("type : Elevator")]
        [SerializeField] private GameObject _goElevatorMenu;

        [Header("type : WorldAvatar")]
        private WorldAvatar _worldAvatarInfo;
        private WorldPlayer _worldPlayerInfo;

        [Header("type : Link")]
        [SerializeField] private eClickableLinkType _linkType;
        [SerializeField] private string _linkPath;

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
                        image.color = _colorOnExit;
                    }
                    break;

                case eRendererType.SpriteRenderer :
                    if (TryGetComponent<SpriteRenderer>(out spriteRenderer))
                    {
                        spriteRenderer.color = _colorOnExit;
                    }
                    break;

                default :
                    break;
            }

            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                    SetBuildingInfo();
                    break;

                case eClickableObjectType.WorldAvatar :
                    SetWorldAvatarInfo();
                    break;

                case eClickableObjectType.Link :
                    SetLinkInfo();
                    break;

                default :
                    Debug.Log($"{TAG} | Start(), 현재 Clickable object 의 타입 : {_objectType.ToString()}, 준비 중...");
                    break;
            }
        }

    #endregion


    #region Interface functions implementation (for UGUI Click)

        public void OnPointerEnter(PointerEventData data) 
        {
            if (_rendererType != eRendererType.UI_Image) return;

            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                    image.color = _colorOnEnter;
                    if (_imgTag != null) 
                    {
                        _imgTag.gameObject.SetActive(true);
                    }
                    break;

                case eClickableObjectType.WorldAvatar :
                    break;

                default :
                    Debug.Log($"{TAG} | OnPointerEnter(), 현재 Clickable object 의 타입 : {_objectType.ToString()}, 준비 중...");
                    break;
            }
        }

        public void OnPointerExit(PointerEventData data) 
        {
            if (_rendererType != eRendererType.UI_Image) return;

            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                    image.color = _colorOnExit;
                    if (_imgTag != null)
                    {
                        _imgTag.gameObject.SetActive(_alwaysOpenTag);
                    }
                    break;

                case eClickableObjectType.WorldAvatar :
                    break;

                default :
                    Debug.Log($"{TAG} | OnPointerEixt, 현재 Clickable object 의 타입 : {_objectType.ToString()}, 준비 중...");
                    break;
            }
        }

        public void OnPointerClick(PointerEventData data) 
        {
            if (_rendererType != eRendererType.UI_Image) return;

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

                case eClickableObjectType.Link :
                    if (data.button == PointerEventData.InputButton.Left)
                        LinkLeftClick(data);
                    break;

                case eClickableObjectType.Meeting :
                    if (data.button == PointerEventData.InputButton.Left)
                        MeetingLeftClick(data);
                    break;

                default :
                    Debug.Log($"{TAG} | OnPointerClick(), 현재 Clickable object 의 타입 : {_objectType.ToString()}, 클릭 처리 준비 중...");
                    break;
            }
        }

    #endregion  // Interface functions implementation (for UGUI Click)


    #region Mouse enter, exit functions (with SpriteRenderer)

        private void OnMouseEnter()
        {
            if (_rendererType != eRendererType.SpriteRenderer) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                case eClickableObjectType.Link :
                case eClickableObjectType.Board :
                case eClickableObjectType.Notice :
                case eClickableObjectType.Seminar :
                case eClickableObjectType.Meeting :
                case eClickableObjectType.FileBox :
                    spriteRenderer.color = _colorOnEnter;
                    break;

                default :
                    Debug.Log($"{TAG} | OnMouseEnter(), 현재 Clickable object 의 타입 : {_objectType.ToString()}, SpriteRenderer mouse enter");
                    break;
            }
        }

        private void OnMouseExit() 
        {
            if (_rendererType != eRendererType.SpriteRenderer) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            switch (_objectType) 
            {
                case eClickableObjectType.Building :
                case eClickableObjectType.Link :
                case eClickableObjectType.Board :
                case eClickableObjectType.Notice :
                case eClickableObjectType.Seminar :
                case eClickableObjectType.Meeting :
                case eClickableObjectType.FileBox :
                    spriteRenderer.color = _colorOnExit;
                    break;

                default :
                    Debug.Log($"{TAG} | OnMouseExit(), 현재 Clickable object 의 타입 : {_objectType.ToString()}, SpriteRenderer mouse exit");
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
                case eClickableObjectType.Link :
                    LinkLeftClick(data);
                    break;

                case eClickableObjectType.Board :
                    if (data.button == PointerEventData.InputButton.Left)
                        BoardLeftClick(data);
                    break;

                case eClickableObjectType.Notice :
                    if (data.button == PointerEventData.InputButton.Left)
                        NoticeLeftClick(data);
                    break;
                
                case eClickableObjectType.Seminar:
                    if (data.button == PointerEventData.InputButton.Left)
                        SeminarLeftClick(data);
                    break;
                
                case eClickableObjectType.Meeting:
                    if (data.button == PointerEventData.InputButton.Left)
                        MeetingLeftClick(data);
                    break;

                case eClickableObjectType.FileBox:
                    if (data.button == PointerEventData.InputButton.Left)
                        FileBoxLeftClick(data);
                    break;
                
                default :
                    Debug.Log($"{TAG} | OnMouseUp(), 현재 Clickable object 의 타입 : {_objectType.ToString()}, SpriteRenderer clicked.");
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
            if (_soBuildingData.BuildingName.Equals("Community City"))
            {
                Enter().Forget();
            }
            else 
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
        }

        private void BuildingRightClick(PointerEventData data) 
        {
            if (_menuItems.Length == 0) 
            {
                Debug.LogError($"{TAG} | BuildingRightClick(), 이 건물에 등록된 메뉴가 하나도 없습니다.");
                return;
            }

            MenuController ctrl = MenuBuilder.singleton.Build();
            if (ctrl == null)
            {
                Debug.LogError($"{TAG} | BuildingRightClick(), 현재 scene 에 MenuBuilder instance 가 없습니다.");
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
                            Enter().Forget();
                            break;
                        
                        case S.MENU_MOVE_TO_OFFICE :
                            if (_soBuildingData.UsingJoycollab)
                            {
                                JsLib.OpenWebview(_soBuildingData.JoycollabLink, "office");
                            }
                            else 
                            {
                                Debug.Log($"{TAG} | BuildingRightClick(), {_soBuildingData.BuildingName} 은 joycollab 주소가 없음.");
                            }
                            break;

                        default :
                            Debug.LogError($"{TAG} | BuildingRightClick(), 아직 준비되지 않은 아이템 항목 : {item}");
                            break;
                    }
                });
            }
            ctrl.Open(data.position);
        }

        private void BuildingWheelClick(PointerEventData data) 
        {

        }

        private async UniTaskVoid Enter() 
        {
            // 사용자 정보 로드 
            string url = string.Format(URL.MEMBER_INFO, R.singleton.memberSeq);
            PsResponse<ResMemberInfo> res = await NetworkTask.RequestAsync<ResMemberInfo>(url, eMethodType.GET, string.Empty, R.singleton.token);
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return;
            }

            // world avatar 정보 설정.
            R.singleton.MemberInfo = res.data;
            WorldAvatarInfo info = new WorldAvatarInfo(
                res.data.seq, 
                res.data.nickNm,
                res.data.photo,
                res.data.memberType,
                S.ONLINE,
                res.data.compName,
                res.data.jobGrade
            );
            WorldAvatar.localPlayerInfo = info;
            WorldChatView.localPlayerInfo = info;
            WorldPlayer.localPlayerInfo = info;

            // 센터 접속
            var manager = NetworkManager.singleton;
            manager.networkAddress = "dev.jcollab.com";
            manager.StartClient();
        }
        
    #endregion  // about 'Building' 


    #region about 'World Avatar'

        private void SetWorldAvatarInfo() 
        {
            // _worldAvatarInfo = GetComponent<WorldAvatar>();
            TryGetComponent<WorldAvatar>(out _worldAvatarInfo);
            TryGetComponent<WorldPlayer>(out _worldPlayerInfo);
        }

        private void WorldAvatarRightClick(PointerEventData data) 
        {
            if (_menuItems.Length == 0) 
            {
                Debug.LogError($"{TAG} | WorldAvatarRightClick(), 이 아바타에 등록된 메뉴가 하나도 없습니다.");
                return;
            }

            MenuController ctrl = MenuBuilder.singleton.Build();
            if (ctrl == null)
            {
                Debug.LogError($"{TAG} | WorldAvatarRightClick(), 현재 scene 에 MenuBuilder instance 가 없습니다.");
                return;
            }

            int avatarSeq = _worldAvatarInfo == null ? _worldPlayerInfo.avatarSeq : _worldAvatarInfo.avatarSeq;
            string avatarName = _worldAvatarInfo == null ? _worldPlayerInfo.avatarName : _worldAvatarInfo.avatarName;

            bool isMyMenu = (R.singleton.memberSeq == avatarSeq);
            ctrl.Init(avatarName);
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
                            WindowManager.singleton.Push(S.WorldScene_MemberProfile, avatarSeq, data.position.x, data.position.y);
                            break;

                        case S.MENU_CHAT :
                            string chatLink = string.Format(URL.CHATVIEW_LINK, R.singleton.memberSeq, avatarSeq, R.singleton.Region);
                            JsLib.OpenChat(chatLink, avatarSeq);
                            break;

                        case S.MENU_CALL :
                            List<int> callTarget = new List<int> {
                                R.singleton.memberSeq,
                                avatarSeq
                            };
                            SystemManager.singleton.CallOnTheSpot(callTarget).Forget();
                            break;

                        case S.MENU_MEETING :
                            List<int> meetingTarget = new List<int> { avatarSeq };
                            SystemManager.singleton.MeetingOnTheSpot(meetingTarget).Forget();
                            break;
                        
                        default :
                            Debug.LogError($"{TAG} | WorldAvatarRightClick(), 아직 준비되지 않은 아이템 항목 : {item}");
                            break;
                    }
                });
            }
            ctrl.Open(data.position);
        }

    #endregion  // about 'World Avatar' 


    #region 'Board' Click event

        private void BoardLeftClick(PointerEventData data) 
        {
            Debug.Log($"{TAG} | BoardLeftClick(), Board 가 클릭되었습니다. 게시판 정보를 출력합시다.");
        }

    #endregion  // 'Board' Click event


    #region 'Notice' Click event

        private void NoticeLeftClick(PointerEventData data) 
        {
            Debug.Log($"{TAG} | NoticeLeftClick(), Notice 가 클릭되었습니다. 공지사항 정보를 출력합시다.");
        }

    #endregion  // 'Notice' Click event


    #region 'Seminar' Click event

        private void SeminarLeftClick(PointerEventData data) 
        {
            Debug.Log($"{TAG} | SeminarLeftClick(), seminar kiosk 가 클릭되었습니다. 세미나 정보를 출력합시다.");
        }

    #endregion  // 'Seminar' Click event
    
    
    #region 'Meeting' Click event

        private void MeetingLeftClick(PointerEventData data) 
        {
            Debug.Log($"{TAG} | MeetingLeftClick(), Meeting board 가 클릭되었습니다. 회의 정보를 출력합시다.");
        }

    #endregion  // 'Meeting' Click event


    #region 'FileBox' Click event

        private void FileBoxLeftClick(PointerEventData data) 
        {
            Debug.Log($"{TAG} | FileBoxLeftClick(), File Box 가 클릭되었습니다. 파일함 정보를 출력합시다.");
        }

    #endregion  // 'Meeting' Click event


    #region 'Link' Click event

        private void SetLinkInfo() 
        {
            switch (_linkType) 
            {
                case eClickableLinkType.TV :
                case eClickableLinkType.Instagram :
                case eClickableLinkType.Youtube :
                case eClickableLinkType.Homepage :
                    // Debug.Log($"{TAG} | SetLinkInfo(), type : {_linkType} : 환경설정 링크 참조. 현재는 고정값 사용.");
                    break;

                case eClickableLinkType.Information :
                case eClickableLinkType.Tutorial :
                    // Debug.Log($"{TAG} | SetLinkInfo(), type : {_linkType} : 추후 튜토리얼 팝업 출력. 현재는 빈값 사용.");
                    _linkPath = string.Empty;
                    break;

                case eClickableLinkType.BuilletinBoard :
                    _linkPath = string.Format(URL.BUILLETIN_BOARD, R.singleton.memberSeq, R.singleton.accessToken, R.singleton.Region);
                    break;

                case eClickableLinkType.MiniMap :
                    // Debug.Log($"{TAG} | SetLinkInfo(), type : {_linkType} : 추후 별도의 팝업 출력. 현재는 빈값 사용.");
                    // _linkPath = string.Empty;
                    break;

                case eClickableLinkType.GuestBook :
                    _linkPath = string.Format(URL.GUEST_BOOK_PATH, R.singleton.accessToken, R.singleton.memberSeq, R.singleton.Region);
                    break;

                case eClickableLinkType.Display :
                    // Debug.Log($"{TAG} | SetLinkInfo(), type : {_linkType} : 추후 별도의 팝업 출력. 현재는 빈값 사용.");
                    _linkPath = string.Empty;
                    break;

                case eClickableLinkType.None :
                default :
                    // Debug.Log($"{TAG} | SetLinkInfo(), type : {_linkType} : 빈값 사용. 클릭해도 반응하지 않음.");
                    _linkPath = string.Empty;
                    break;
            }
        }

        private void LinkLeftClick(PointerEventData data) 
        {
            switch (_linkType) 
            {
                case eClickableLinkType.Information :
                case eClickableLinkType.Tutorial :
                    Debug.Log($"{TAG} | LinkLeftClick(), type : {_linkType} : Tutorial popup 연결 예정.");
                    break;

                case eClickableLinkType.MiniMap :
                    SystemManager.singleton.ShowTutorialPopup(eTutorialType.MiniMap);
                    break;

                case eClickableLinkType.None :
                    Debug.Log($"{TAG} | LinkLeftClick(), type : {_linkType} : 아무 것도 하지 않음.");
                    break;

                default :
                    OpenLink();
                    break;
            }
        }

        private void OpenLink() 
        {
            if (string.IsNullOrEmpty(_linkPath))
            {
                PopupBuilder.singleton.OpenAlert( 
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "링크 설정 안내", R.singleton.CurrentLocale)
                );
                return;
            }

            JsLib.OpenWebview(_linkPath, Util.EnumToString(_linkType));
        }

    #endregion  // 'Link' Click event
    }
}