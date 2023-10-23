/// <summary>
/// MenuBuilder 로 생성한 메뉴 항목 제어 스크립트
/// @author         : HJ Lee
/// @last update    : 2023. 10. 06 
/// @version        : 0.2
/// @update
///     v0.1 (2023. 03. 06) : 최초 생성
///     v0.2 (2023. 10. 06) : 기능 수정 및 테스트 진행
/// </summary>

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using TMPro;

namespace Joycollab.v2
{
    public class MenuItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private const string TAG = "MenuItem";
        private const string TABLE = "Menu";

        [SerializeField] private TMP_Text _txtTitle;
        [SerializeField] private LocalizeStringEvent _localeEvent; 

        public string ItemName => _txtTitle.text;

        public void Init(string key) 
        {
            _localeEvent.StringReference.SetReference(TABLE, key);
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