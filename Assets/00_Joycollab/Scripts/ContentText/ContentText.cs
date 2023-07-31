/// <summary>
/// 게시판, 공지사항 등의 텍스트를 제어하는 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 07. 14
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 14) : 최초 생성
/// </summary>

using UnityEngine;
using TMPro;

namespace Joycollab.v2 
{
    [RequireComponent(typeof(TMP_InputField))]
    public class ContentText : MonoBehaviour
    {
        private const string TAG = "ContentText";
        private const string HTTP = "http://";
        private const string HTTPS = "https://";

        private TMP_InputField input;
        private string selectedString;

    
    #region Unity functions

        private void Update() 
        {
            if (string.IsNullOrEmpty(selectedString)) return;

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) 
            {
                if (Input.GetKeyUp(KeyCode.C)) 
                {
                    Debug.Log($"{TAG} | (window) selected string : {selectedString}");
                    JsLib.CopyToClipboard(selectedString);
                }
            }
            else if (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))
            {
                if (Input.GetKeyUp(KeyCode.C)) 
                {
                    Debug.Log($"{TAG} | (Mac OS) selected string : {selectedString}");
                    JsLib.CopyToClipboard(selectedString);
                }
            }
        }

    #endregion  // Unity functions


        public void InitText(string s) 
        {
            input = GetComponent<TMP_InputField>();
            input.text = s;

            selectedString = string.Empty;

            input.onTextSelection.AddListener((value, start, end) => {
                    if (end >= start)
                    selectedString = value.Substring(start, (end - start));
                else
                    selectedString = value.Substring(end, (start - end));
            });
            input.onDeselect.AddListener((value) => {
                selectedString = string.Empty;
            });
        }
    }
}