/// <summary>
/// 열거형 정리 문서
/// @author         : HJ Lee
/// @last update    : 2023. 03. 17
/// @version        : 0.1
/// @update
///     v0.1 (2023. 03. 17) : Joycollab 에서 사용하던 열거형 정리. (정리 중)
/// </summary>

namespace Joycollab.v2
{
    /// <summary>
    /// Joycollab 에서 사용하는 Scene 목록 정리
    /// </summary>
    public enum eScenes
    {
        Login = 0,
        Loading, 
        LoadInfo,
        GraphicUI, TextUI,
        Arrange, 
        Management,
        Mobile,
        World, Square,
        Sample,
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
    /// Popup, Menu 형태 및 속성
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
    // -----


    /// <summary>
    /// click 가능한 object 구분
    /// </summary>
    public enum eClickableObjectType 
    {
        // World 에서 사용할 항목들.
        Building, Elevator, Information, WorldAvatar,
        
        // Joycollab 과 World 에서 공통으로 사용할 항목들.
        Board, Notice, Seminar, Display, Meeting,

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
        UserInfo, Alarm,

        Test
    }


    /// <summary>
    /// 업무 구분
    /// </summary>
    public enum eWorkType
    {
        ToDo, Objective, KeyResult
    }
}