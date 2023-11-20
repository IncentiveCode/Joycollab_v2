/// <summary>
/// 태그를 제어하는 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 11. 20
/// @version        : 0.1
/// @update
///     v0.1 (2023. 11. 20) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2 
{
    [RequireComponent(typeof(Text))]
    [RequireComponent(typeof(Button))]
    public class TagLink : MonoBehaviour
    {
        private const string Tag = "TagLink";

        private Text _text;
        private Button _button;


        public void InitTag(string s) 
        {
            _text = GetComponent<Text>();
            _text.text = s;
            _text.color = C.Link;

            _button = GetComponent<Button>();
            _button.interactable = true;
            _button.onClick.AddListener(() => {
                Debug.Log($"{Tag} | tag name : {s}, 추후 태그 검색 기능이 오픈됩니다.");
            });
        }

        public void InitPlain(string s) 
        {
            _text = GetComponent<Text>();
            _text.text = s;
            _text.color = C.Plain;

            _button = GetComponent<Button>();
            _button.interactable = false;
        }
    }
}