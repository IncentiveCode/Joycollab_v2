/// <summary>
/// scroll view 안에 있는 TMP_InputField 가 scroll event 를 받지 않도록 조치하는 Add on 스크립트 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 14
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 14) : JoyCollabClient 의 TmpInputScrollViewScroll.cs 수정 
/// </summary>

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class InputFieldOnDetailView : TMP_InputField
    {
        private ScrollRect scrollRect;


    #region Override functions

        protected override void Awake() 
        {
            base.Awake();

            GetParentScrollRect().Forget();
        }

        public override void OnScroll(PointerEventData eventData)
        {
            if (scrollRect != null) scrollRect.OnScroll(eventData);
        }

    #endregion  // Override functions


        private async UniTaskVoid GetParentScrollRect() 
        {
            await UniTask.DelayFrame(1);

            if (transform.parent.parent.parent.TryGetComponent<ScrollRect>(out scrollRect)) { }
        }
    }
}