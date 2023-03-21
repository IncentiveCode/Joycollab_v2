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
        Main, TextUI,
        ArragneSpace, Management,
        World, Square,
        Loading, 
        Test
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
        Appearing, Appeared, Disappearing, Disappeared
    }


    /// <summary>
    /// NetworkTask 전송시 선택 가능한 요청 형태 구분
    /// </summary>
    public enum MethodType 
    {
        POST, PUT, GET, DELETE, PATCH, HEAD
    }


    /// <summary>
    /// Popup 형태 및 속성
    /// </summary>
    public enum PopupType 
    {
        None = 0,
        Alert, Confirm, Prompt, 
        ConfirmWithOption
    }

    public enum PopupButtonType 
    {
        Normal = 0,
        Success, Info, Warning, Error
    }

    public enum PopupStyle 
    {
        Office, World
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
        Board, Notice, Seminar, Display,

        // 추후 object 는 상세하게 쪼개질 가능성 있음.
        Object,

        // Dummy 또는 추후에 사용하게 될 수도 있는 jayco 같은 마스코트.
        Dummy,

        // 아직 정해지지 않은 항목들.
        Nothing
    }

    public enum eMenuTitle 
    {
        Detail = 0, Enter, Test
    }
    // -----


    /// <summary>
    /// Sprite Sorting 을 위한 항목 구분
    /// </summary>
    public enum eSortingType 
    {
        Static, Update
    }
}