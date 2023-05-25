/// <summary>
/// 고정 위치를 가지는 창의 속성을 관리하기 위한 추상 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 05. 25
/// @version        : 0.3
/// @update
///     v0.1 (2023. 03. 20) : 최초 생성
///     v0.2 (2023. 03. 21) : Show() 를 async 로 변경
///     v0.3 (2023. 05. 25) : DOTween test
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Joycollab.v2
{
    public abstract class FixedView : MonoBehaviour
    {
        protected int viewID;
        protected eVisibleState visibleState;
        protected CanvasGroup canvasGroup;
        // protected Canvas canvas;
        protected RectTransform viewRect;

        //
        protected float fadeTime = 1f;


        protected virtual void Init() 
        {
            viewID = 0;
            visibleState = eVisibleState.Disappeared;

            if (! TryGetComponent<CanvasGroup>(out canvasGroup)) 
            {
                Debug.Log("CanvasGroup component 확인 요망");
                return;
            }

            /**
            if (! TryGetComponent<Canvas>(out canvas)) 
            {
                Debug.Log("Canvas component 확인 요망");
                return;
            }
             */

            if (! TryGetComponent<RectTransform>(out viewRect)) 
            {
                Debug.Log("RectTransform component 확인 요망");
                return;
            }
        }

        protected virtual void Reset()
        {
            viewRect.anchoredPosition = Vector2.zero;

            Hide();
        }

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

            // dotween test
            canvasGroup.DOFade(0f, fadeTime);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public async virtual UniTaskVoid Show() 
        {
            visibleState = eVisibleState.Appearing;

            // TODO. add 'Mobile Progress'

            await UniTask.Yield();
        }

        public async virtual UniTaskVoid Show(int opt) 
        {
            visibleState = eVisibleState.Appearing;

            // TODO. add 'Mobile Progress'

            await UniTask.Yield();
        }

        public async virtual UniTaskVoid Show(string opt="") 
        {
            visibleState = eVisibleState.Appearing;

            // TODO. add 'Mobile Progress'

            await UniTask.Yield();
        }

        // protected async virtual UniTaskVoid Appearing() 
        protected virtual void Appearing() 
        {
            if (canvasGroup == null) return;
            if (visibleState != eVisibleState.Appearing) 
                visibleState = eVisibleState.Appearing;

            /**
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0f;
            canvas.enabled = true;

            float minimimDuration = 0.5f;
            float loadTime = 0f;
            float loadRatio = 0f;

            while (canvasGroup.alpha < 1f) 
            {
                loadTime += Time.deltaTime;
                loadRatio = loadTime / minimimDuration;
                if (loadRatio >= 1f) break;

                canvasGroup.alpha = loadRatio;
                await UniTask.Yield();
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            visibleState = eVisibleState.Appeared;
             */

            // dotween test
            canvasGroup.DOFade(1f, fadeTime);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            visibleState = eVisibleState.Appeared;
        }
    }
}