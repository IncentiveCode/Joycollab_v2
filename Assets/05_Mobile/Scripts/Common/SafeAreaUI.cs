/// <summary>
/// [mobile]
/// Device safe area 에 맞춰서 화면 scale 조절하는 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 05. 10
/// @version        : 0.1
/// @update
///     v0.1 : 외부에서 발견, v1 에서 진행했던 기존 수정사항들 업데이트.
/// </summary>

using UnityEngine;

namespace Joycollab.v2
{
    public class SafeAreaUI : MonoBehaviour
    {
        private const string TAG = "SafeAreaUI";
        private RectTransform rect;


    #region Unity function

        private void Start()
        {
        #if UNITY_ANDROID || UNITY_IOS
            Rect safeArea = Screen.safeArea;

            Vector2 newAnchorMin = safeArea.position;
            Vector2 newAnchorMax = safeArea.position + safeArea.size;

            newAnchorMin.x /= Screen.width;
            newAnchorMax.x /= Screen.width;
            newAnchorMin.y /= Screen.height;
            newAnchorMax.y /= Screen.height;

            rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = newAnchorMin;
            rect.anchorMax = newAnchorMax;

            Debug.Log($"{TAG} | Start(), width : {safeArea.width}, height : {safeArea.height}");
        #endif
        }

    #endregion
    }
}