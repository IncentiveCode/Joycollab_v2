/// <summary>
/// 시스템 플랜 상세 내용을 출력하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 07. 31
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 31) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Joycollab.v2 
{
    [RequireComponent(typeof(TMP_Text))]
    public class PlanContent : MonoBehaviour
    {
        private const string TAG = "PlanContent";

        private Image icon;
        private TMP_Text text;

        public void Init(Color c, string s) 
        {
            this.icon = GetComponentInChildren<Image>();
            this.icon.color = c;

            this.text = GetComponent<TMP_Text>();
            text.text = s;
        }
    }
}