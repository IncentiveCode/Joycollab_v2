/// <summary>
/// Utility functions 집합 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 03. 15
/// @version        : 1.0
/// @update
///     v1.0 (2023. 03. 15) : 최초 생성, Joycollab & TechnoPark 등 작업을 하면서 작성한 것들을 수정 및 적용 (진행 중)
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
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


        /// <summary>
        /// RawImage 를 원본 크기의 scale 에 맞춰서 resize 하는 함수.
        /// </summary>
        /// <param name="rect">RawImage 를 담고 있는 RectTransform</param>
        /// <param name="image">resizing 이 필요한 RawImage</param>
        /// <param name="width">이미지 최대 너비</param>
        /// <param name="height">이미지 최대 높이</param>
        /// <param name="padding">패딩 값</param>
        public static void ResizeRawImage(RectTransform rect, RawImage image, float width, float height, float padding = 0) 
        {
            if (image.texture == null) 
            {
                Debug.Log("Tools | ResizeRawImage() image texture null");
                rect.sizeDelta = new Vector2(width, height); 
                return;
            }

            float w = 0, h = 0;
            float p = 1 - padding; 
            float r = image.texture.width / (float) image.texture.height;

            var bounds = new Rect(0, 0, width, height);
            if (Mathf.RoundToInt(rect.eulerAngles.z) % 180 == 90) 
            {
                bounds.size = new Vector2(bounds.height, bounds.width);
            }

            h = bounds.height * p;
            w = h * r;
            if (w > bounds.width * p) 
            {
                w = bounds.width * p;
                h = w / r;
            }

            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        }


    #region Enum
        /// <summary>
        /// Enumtype 의 항목을 문자열로 변경하는 함수.
        /// </summary>
        /// <param name="item">enum item</param>
        /// <returns>문자열로 변경된 item</returns>
        public static string EnumToString<T>(T item) 
        {
            return item.ToString();
        }

        /// <summary>
        /// String 값을 받아서 enum 형태로 변경하는 함수.
        /// </summary>
        /// <param name="item">Enum 형태로 변경하고자 하는 string 값</param>
        /// <typeparam name="T">원하는 Enum class name</typeparam>
        /// <returns>해당 class 에 속한 enum 값</returns>
        public static T StringToEnum<T>(string item) 
        {
            return (T)Enum.Parse(typeof(T), item);
        } 
    #endregion  // enum
	}
}