/// <summary>
/// [mobile]
/// 파일 경로를 출력하기 위한 Link 를 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 19
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 19) : 최초 생성.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Joycollab.v2 
{
    public class FileLinkM : MonoBehaviour
    {
        [SerializeField] private Button _btn;
        [SerializeField] private TMP_Text _txt;
        
        public void Init(string text) 
        {
            _txt.text = text;
            _btn.interactable = ! text.Equals("/");
        }
    }
}