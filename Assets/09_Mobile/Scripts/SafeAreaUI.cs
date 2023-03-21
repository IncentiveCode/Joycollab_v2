/// <summary>
/// Mobile device 에서 Notch 영역을 체크하고, 화면을 그릴 수 있는 영역을 체크하는 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 03. 21
/// @version        : 0.1
/// @update
///     v0.1 (2023. 03. 21) : Joycollab 에서 사용하던 클래스 정리.
/// </summary>

using UnityEngine;

public class SafeAreaUI : MonoBehaviour
{
    private RectTransform rect;

    private void Start()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        // 세이프 에어리어 영역값 가져오기. 노치가 없으면, new Rect(0, 0, Screen.Width, Screen.Height) 값과 동일하다.
        Rect safeArea = Screen.safeArea;

        // 이 게이오브젝트의 RectTransform 앵커최대최소값을 다시 설정해서 세이프 에어리어의 영역만큼 잡히도록 한다.
        Vector2 newAnchorMin = safeArea.position;
        Vector2 newAnchorMax = safeArea.position + safeArea.size;

#if UNITY_ANDROID
        if (safeArea.height == Screen.height)
        {
            // 노치가 없는 경우. 안드로이드에서 한정으로 status bar 높이를 구해서 빼줘야 한다.
            int height = ApplicationChrome.GetStatusBarHeight();
            newAnchorMax.y -= height;
        }
#endif

        newAnchorMin.x /= Screen.width;
        newAnchorMax.x /= Screen.width;
        newAnchorMin.y /= Screen.height;
        newAnchorMax.y /= Screen.height;

        rect = gameObject.GetComponent<RectTransform>();
        rect.anchorMin = newAnchorMin;
        rect.anchorMax = newAnchorMax;

        Debug.Log("Mobile | width : "+ safeArea.width +", height : "+ safeArea.height);
#endif
    }

    public int Height 
    {
        get {
            Debug.Log("Mobile | height : "+ rect.rect.height);
            return (int) rect.rect.height;
        }
    }
}