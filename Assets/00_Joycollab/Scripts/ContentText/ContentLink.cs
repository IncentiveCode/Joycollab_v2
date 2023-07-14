/// <summary>
/// 게시판, 공지사항 등의 텍스트를 제어하는 클래스 (for 링크)
/// @author         : HJ Lee
/// @last update    : 2023. 07. 14
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 14) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2 
{
    [RequireComponent(typeof(Text))]
    [RequireComponent(typeof(Button))]
    public class ContentLink : MonoBehaviour
    {
        private const string TAG = "ContentLink";

        private Text text;
        private Button button;


        public void InitLink(string s) 
        {
            text = GetComponent<Text>();
            text.text = s;

            button = GetComponent<Button>();
            button.onClick.AddListener(() => JsLib.OpenWebview(s, string.Empty));
        }
    }
}