/// <summary>
/// [mobile]
/// Unity korea 강의에서 얻은, safe area 에 맞춰서 화면 scale 조절하는 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 03. 31
/// @version        : 0.2
/// @reference      : https://www.youtube.com/watch?v=_jW_D2vF9J8&list=PLhonFTbuFJQvUQvlkSiuPmD5sx2rU1EHQ&index=8&ab_channel=UnityKorea
/// @update
///     v0.1 : 외부에서 발견, 수정해서 사용. ApplicationChrome 적용.
///     v0.2 (2023. 03. 31) : 일부 내용 수정.
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2 
{
    public class CanvasPixelToUIKitSize : MonoBehaviour
    {
        private CanvasScaler scaler;


    #region Unity functions
        private void Awake() 
        {
            scaler = GetComponent<CanvasScaler>();
        }

        private void Start() 
        {
            Resize();
        }

    #endregion


    #region Private functions

        private void Resize() 
        {
            if (Application.isEditor) return;
            if (scaler == null) return;

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

        #if UNITY_ANDROID

            scaler.scaleFactor = Screen.dpi / 160;

            float fixedRatio = 9f / 19f;
            float currentRatio = (float) Screen.width / (float) Screen.height;
            scaler.matchWidthOrHeight = (currentRatio >= fixedRatio) ? 0 : 1;

        #elif UNITY_IOS

            // _canvasScaler.scaleFactor = ApplePlugin.GetNativeScaleFactor();

        #endif
        }

    #endregion
    }
}