/// <summary>
/// 팝업에서 사용하는 버튼을 쉽게 사용하기 위해 만든 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 04. 14
/// @version        : 0.3
/// @update
///     v0.1 (2023. 03. 30) : 최초 생성
///     v0.2 (2023. 04. 07) : workspace 용 버튼 생성 추가
///     v0.3 (2023. 04. 14) : world 용 버튼 생성 추가
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2 
{
    public class PopupButton : MonoBehaviour
    {
        [Header("Button Option")]
        [SerializeField] private ePopupButtonType _type;
        [SerializeField] private LayoutElement _element;

        [Header("Button content")]
        [SerializeField] private Button _btn;
        [SerializeField] private Text _txt; 
        [SerializeField] private Outline _outline;

        public void Init(ePopupButtonType type, string text) 
        {
            _type = type;
            _txt.text = text;

            ColorBlock cb = _btn.colors;

            switch (type) 
            {
                case ePopupButtonType.Normal :
                default :
                    _element.minWidth = 120;
                    _element.minHeight = 40;

                    cb.normalColor = cb.selectedColor = C.NORMAL;
                    cb.highlightedColor = cb.pressedColor = C.NORMAL_ON;
                    _txt.color = Color.white;

                    _outline.enabled = false;
                    break;
                
                case ePopupButtonType.Warning :
                    _element.minWidth = 118;
                    _element.minHeight = 38;

                    cb.normalColor = cb.selectedColor = C.WARNING;
                    cb.highlightedColor = cb.pressedColor = C.WARNING_ON;
                    _txt.color = Color.black;

                    _outline.effectColor = C.WARNING_BORDER;
                    _outline.enabled = true;
                    break;

                case ePopupButtonType.worldNormal :
                    _element.minWidth = 120;
                    _element.minHeight = 40;

                    cb.normalColor = cb.selectedColor = C.WORLD_NORMAL;
                    cb.highlightedColor = cb.pressedColor = C.WORLD_NORMAL_ON;
                    _txt.color = Color.white;

                    _outline.enabled = false;
                    break;

                case ePopupButtonType.worldWarning :
                    _element.minWidth = 120;
                    _element.minHeight = 40;

                    cb.normalColor = cb.selectedColor = C.WORLD_WARNING;
                    cb.highlightedColor = cb.pressedColor = C.WORLD_WARNING_ON;
                    _txt.color = C.WORLD_NORMAL;

                    _outline.enabled = false;
                    break;
            }

            _btn.colors = cb;
        }
    }
}