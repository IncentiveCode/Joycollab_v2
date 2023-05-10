/// <summary>
/// 두 지점만 왔다갔다 하는 UI 를 위한 script
/// @author         : HJ Lee
/// @last update    : 2023. 02. 07
/// @version        : 0.1
/// @update
///     v0.1 (2023. 02. 07) : 기존의 PanelMovable 을 수정.
/// </summary>

using System.Collections;
using UnityEngine;

namespace Joycollab.v2
{
    public class BackAndForth : MonoBehaviour
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private Vector2 posBack;
        [SerializeField] private Vector2 posForth;
        [SerializeField] private bool StartForthPosition;
        [SerializeField] private float time;

        private readonly float step = 5f; 


    #region Unity functions

        private void Awake() 
        {
            if (rect == null) rect = GetComponent<RectTransform>();
            if (posBack == null) posBack = Vector2.zero;
            if (posForth == null) posForth = Vector2.zero;

            rect.anchoredPosition = StartForthPosition ? posForth : posBack;
            time = (time == 0f) ? 0.5f : time;
        }

    #endregion


    #region Public functions

        public IEnumerator Move(bool isForth) 
        {
            float elapsed = 0f;
            float distance = 0f;

            while (true) 
            {
                elapsed += Time.deltaTime;
                distance = Vector2.Distance(rect.anchoredPosition, isForth ? posForth : posBack);
                if (distance >= step) 
                {
                    rect.anchoredPosition = Vector2.Lerp(
                        rect.anchoredPosition,
                        isForth ? posForth : posBack,
                        elapsed 
                    );
                }
                else 
                {
                    elapsed = distance = 0f;
                    rect.anchoredPosition = isForth ? posForth : posBack;

                    break;
                }

                yield return null;
            }
        }

    #endregion
    }
}