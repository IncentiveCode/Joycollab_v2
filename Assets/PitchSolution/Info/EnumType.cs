/// <summary>
/// 열거형 관리를 위한 문서
/// @author         : HJ Lee
/// @last update    : 2023. 02. 25
/// @version        : 0.1
/// @update
///     v0.1 (2023. 02. 25) : Joycollab 에서 사용하던 열거형 정리. (정리 중)
/// </summary>

namespace PitchSolution
{
    /// <summary>
    /// PC 환경에서 사용하는 Scene 목록 정리
    /// </summary>
    public enum eScenes
    {
        Login = 0,
        GraphicUI, ArrangeSpace, TextUI,
        World, Square,
        Loading, 
        Test
    }

    /// <summary>
    /// 마우스 클릭으로 상호작용이 가능한 항목들 정리
    /// </summary>
    public enum eClickableObjectType 
    {
        None = 0,

        // World 에서 사용할 항목들.
        Building, Elevator, Information, WorldAvatar,
        
        // Joycollab 과 World 에서 공통으로 사용할 항목들.
        Board, Notice,

        // 추후 object 는 상세하게 쪼개질 가능성 있음.
        Object,

        // Dummy 또는 추후에 사용하게 될 수도 있는 jayco 같은 마스코트.
        Dummy
    }

    /// <summary>
    /// UI Image 를 사용하거나, SpriteRenderer 를 사용하는 항목 구분
    /// </summary>
    public enum eRendererType 
    {
        UI_Image, spriteRenderer
    }

    /// <summary>
    /// Sprite Sorting 을 위한 항목 구분
    /// </summary>
    public enum eSortingType 
    {
        Static, Update
    }
}