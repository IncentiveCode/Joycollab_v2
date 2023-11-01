/// <summary>
/// 열거형 정리 문서
/// @author         : HJ Lee
/// @last update    : 2023. 11. 01
/// @version        : 0.7
/// @update
///     v0.1 (2023. 03. 17) : Joycollab 에서 사용하던 열거형 정리.
///     v0.2 (2023. 07. 18) : Xmpp type, Webview type 추가.
///     v0.3 (2023. 08. 01) : Plan type 추가.
///     v0.4 (2023. 08. 10) : Scene type 수정. (Init, Map 추가. 순서 변경.)
///     v0.5 (2023. 09. 25) : Scene type 추가. clickable type 추가 (eScenes.Room, eClickableObjectType.FileBox 추가.)
///     v0.6 (2023. 10. 04) : storage key 추가 (elevator)
///     v0.7 (2023. 11. 01) : Resize direction 추가.
/// </summary>

namespace Joycollab.v2
{
    /// <summary>
    /// Joycollab 에서 사용하는 Scene 목록 정리
    /// </summary>
    public enum eScenes
    {
        Init = 0,
        Loading, 
        SignIn,
        LoadInfo,
        GraphicUI, TextUI,
        Arrange, 
        Management,
        World, Map, Square, Room,
        Sample,
        Mobile,
    }


    /// <summary>
    /// Joycollab 에서 사용하는 View Tag 목록 정리
    /// </summary>
    public enum eViewType 
    {
        Login,
        TextUI,
        Mobile,
    }


    /// <summary>
    /// Image 표현 방식 구분 (UGUI 의 Image / SpriteRenderer) 
    /// </summary>
    public enum eRendererType 
    {
        UI_Image, SpriteRenderer
    }


    /// <summary>
    /// View 출력 상태 구분. (alpha blending, Interactable 등의 처리를 하기 위함)
    /// </summary>
    public enum eVisibleState 
    {
        Appearing, Appeared, Disappeared
    }


    /// <summary>
    /// NetworkTask 전송시 선택 가능한 요청 형태 구분
    /// </summary>
    public enum eMethodType 
    {
        POST, PUT, GET, DELETE, PATCH, HEAD
    }


    /// <summary>
    /// Popup, Menu, Webview 형태 및 속성
    /// </summary>
    public enum ePopupType 
    {
        Alert, Prompt, Confirm, ConfirmWithOption,
    }
    public enum ePopupButtonType 
    {
        Normal = 0, Warning,
        worldNormal, worldWarning,

        // 아직 사용 하지 않는 항목들.
        Success, Info, Error
    }
    public enum ePopupStyle 
    {
        Office, World, Mobile
    } 
    public enum eMenuTitle 
    {
        Detail = 0, Enter, Test
    }
    public enum eWebviewType 
    {
        Normal = 0, Chat, VoiceCall, Meeting
    }
    // -----


    /// <summary>
    /// click 가능한 object 구분
    /// </summary>
    public enum eClickableObjectType 
    {
        // World 에서 사용할 항목들.
        Building, Elevator, Information, WorldAvatar,
        
        // Joycollab 과 World 에서 공통으로 사용할 항목들.
        Board, Notice, Seminar, Display, Meeting, FileBox,

        // 추후 object 는 상세하게 쪼개질 가능성 있음.
        Object,

        // Dummy 또는 추후에 사용하게 될 수도 있는 jayco 같은 마스코트.
        Dummy,

        // 아직 정해지지 않은 항목들.
        Nothing
    }


    /// <summary>
    /// Button object decoration 구분
    /// </summary>
    public enum eDecoratableType 
    {
        None = 0,
        SwapImage,              // 이미지만 변경할 때.
        SwapImageWithTooltip,   // 이미지 변경 + 툴팁 출력시
        ChangeTextStyle,        // 텍스트만 변경할 때.
        Both,                   // 두 가지 다 변경할 때.
        TooltipOnly,            // 툴팁만.
        WorldTooltip,           // 월드 캔버스에서 툴팁.
    }


    /// <summary>
    /// tooltip anchor
    /// </summary>
	public enum eTooltipAnchor 
	{
		TopCenter,
        MiddleLeft,
        MiddleRight,
        BottomCenter,
        Center
	}


    /// <summary>
    /// Mobile Gesture 구분
    /// </summary>
    public enum eGestureAct 
    {
        ScaleDown, ScaleUp, UpDown, LeftRight, TurnAround
    }
    public enum eGestureMotion 
    {
        LeftRight, UpDown, 
        RotateCCW, RotateCW,
        Other, LENGTH
    }
    // -----


    /// <summary>
    /// 일정 관련 정보를 볼 때, 일별/주별/월별 선택 구분
    /// </summary>
    public enum eViewOption 
    {
        Daily, Weekly, Monthly
    }


    /// <summary>
    /// 공유 정보를 볼 때, 전체/팀/회사/개인 선택 구분
    /// </summary>
    public enum eShareFilter
    {
        All, Team, Office, Individual
    }


    /// <summary>
    /// 저장소 알림을 받기 위한 key 구분
    /// </summary>
    public enum eStorageKey 
    {
        UserInfo, Alarm, InstantAlarm, Chat,

        FontSize, Locale, Elevator,
        
        WindowRefresh, UserCount,

        Test
    }


    /// <summary>
    /// 업무 구분
    /// </summary>
    public enum eWorkType
    {
        ToDo, Objective, KeyResult
    }
	
	
	/// <summary>
    /// 즐겨찾기 구분
    /// </summary>
    public enum eBookmarkType 
    {
        Notice, Board    
    }


    /// <summary>
    /// 멤버 선택창 타입
    /// </summary>
    public enum eAddMemberType 
    {
        Meeting, 
        SeminarMember, SeminarLecturer, 
        LobbyManager
    }


    /// <summary>
    /// XMPP message type
    /// </summary>
    public enum eXmppType 
    {
        알림,
        공간배치, 자리배치, 자리이동,
        상태변경, 감정변경,
        멤버변경, 멤버권한변경, 멤버추가, 멤버탈퇴,
        회의시작, 회의예약, 회의멤버변경, 회의종료,
        음성통화, 개인채팅, 그룹채팅, 개인및그룹채팅읽지않음총카운트,
        시스템공지, 공지,
        게스트알림, 휴게실방알림, 메세지,
    }


    /// <summary>
    /// sorting type (sprite, image, mesh, and etc)
    /// </summary>
    public enum eSortingType
    {
        Static, Update
    }


    /// <summary>
    /// subscription plan type
    /// </summary>
    public enum ePlanType 
    {
        Free, Basic, Standard, Premium, Trial, Custom
    }


    /// <summary>
    /// resize direction type
    /// </summary>
    public enum eDirection 
    {
        None = 0,
        Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left, TopLeft
    }
}