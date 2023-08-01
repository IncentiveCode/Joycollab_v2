/// <summary>
/// 시스템 플랜 상세 내용을 출력하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 08. 01
/// @version        : 0.2
/// @update
///     v0.1 (2023. 07. 31) : 최초 생성
///     v0.2 (2023. 08. 01) : Init() 할 때, localize string key 를 전달하는 형태로 변경.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using TMPro;

namespace Joycollab.v2 
{
    [RequireComponent(typeof(TMP_Text))]
    public class PlanContent : MonoBehaviour
    {
        private const string TAG = "PlanContent";

        private Image icon;
        private LocalizeStringEvent localizeString;

        public void Init(Color c, string key) 
        {
            this.icon = GetComponentInChildren<Image>();
            this.icon.color = c;

            this.localizeString = GetComponent<LocalizeStringEvent>();
            this.localizeString.StringReference.SetReference("Sentences", key);
        }
    }
}