/// <summary>
/// MenuBuilder 로 생성한 메뉴 항목 제어 스크립트
/// @author         : HJ Lee
/// @last update    : 2023. 03. 06 
/// @version        : 1.0
/// @update
///     - v1.0 (2023. 03. 06) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Joycollab.v2
{
    public class MenuItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text _txtTitle;
        public string ItemName 
        {
            get { return _txtTitle.text; }
            set { _txtTitle.text = value; }
        }

    
    #region interface implementatinos
        public void OnPointerEnter(PointerEventData data) 
        {
            _txtTitle.fontStyle = FontStyles.Bold;
        }

        public void OnPointerExit(PointerEventData data) 
        {
            _txtTitle.fontStyle = FontStyles.Normal;
        }
    #endregion  // interface implementations
    }
}