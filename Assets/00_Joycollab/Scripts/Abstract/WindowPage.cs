/// <summary>
/// WindowView 내부에 탭으로 이루어진 페이지의 속성을 관리하기 위한 추상 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 09. 15
/// @version        : 0.1
/// @update
///     v0.1 (2023. 09. 15) : 최초 생성
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Joycollab.v2
{
    public class WindowPage : MonoBehaviour
    {
        // for view
        private eVisibleState visibleState;
        public bool isDisappeared => visibleState == eVisibleState.Disappeared;
        private CanvasGroup canvasGroup;
        private float fadeTime = 0.5f;


    #region WindowPage functions

        protected virtual void Init() 
        {
            visibleState = eVisibleState.Disappeared;

            if (! TryGetComponent<CanvasGroup>(out canvasGroup)) 
            {
                Debug.Log("CanvasGroup component 확인 요망");
                return;
            }
        }

        protected virtual void Reset() => Hide();

        public virtual void Hide() 
        {
            if (canvasGroup == null) return;

            if (visibleState == eVisibleState.Disappeared) 
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                return;
            }

            if (visibleState != eVisibleState.Disappeared) 
                visibleState = eVisibleState.Disappeared;

            canvasGroup.DOFade(0f, fadeTime);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public virtual void Block(bool isOn) 
        {
            if (canvasGroup == null) return;
            if (visibleState == eVisibleState.Disappeared) return;

            canvasGroup.interactable = !isOn;
            canvasGroup.blocksRaycasts = !isOn;
        }

        public virtual async UniTaskVoid Show() 
        {
            visibleState = eVisibleState.Appearing;
            await UniTask.Yield();
        }

        protected virtual void Appearing() 
        {
            if (canvasGroup == null) return;

            if (visibleState != eVisibleState.Appearing) 
                visibleState = eVisibleState.Appearing;

            canvasGroup.DOFade(1f, fadeTime);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            visibleState = eVisibleState.Appeared;
        }

    #endregion  // WindowPage functions
    }
}