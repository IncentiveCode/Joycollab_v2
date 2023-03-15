/// <summary>
/// Utility functions 집합 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 03. 15
/// @version        : 1.0
/// @update
///     v1.0 (2023. 03. 15) : 최초 생성, Joycollab & TechnoPark 등 작업을 하면서 작성한 것들을 수정 및 적용 (진행 중)
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace PitchSolution
{
	public static class Util
	{
		/// <summary>
        /// Canvas scaler 의 비율을 구하는 함수.
        /// </summary>
        /// <param name="scaler">비율을 구하고자 하는 캔버스의 scaler</param>
        /// <returns>계산의 결과로 얻은 비율</returns>
        public static float CalculateScalerRatio(CanvasScaler scaler) 
        {
            if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                Vector2 referencesResolution = scaler.referenceResolution;
                Vector2 currentResolution = new Vector2(Screen.width, Screen.height);

                float widthRatio = currentResolution.x / referencesResolution.x;
                float heightRatio = currentResolution.y / referencesResolution.y;
                float ratio = Mathf.Lerp(widthRatio, heightRatio, scaler.matchWidthOrHeight);

                return ratio;
            }
            else 
            {
                return 1.0f;
            }
        }
	}
}