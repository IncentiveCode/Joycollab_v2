/// <summary>
/// 윈도우 형태의 창의 속성을 관리하기 위한 추상 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 09. 12
/// @version        : 0.1
/// @update
///     v0.1 (2023. 09. 12) : 최초 생성
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Joycollab.v2
{
    public class WindowView : MonoBehaviour
    {
        // for scene
        [TagSelector]
        [SerializeField] protected string viewTag;
        protected bool isOffice, isWorld;

        // for view
        protected int viewID;
        protected eVisibleState visibleState;
        protected CanvasGroup canvasGroup;
        protected RectTransform viewRect;
        protected float fadeTime = 0.5f;

        // for view state
        protected float posX, posY; 
        [SerializeField] protected float width;
        [SerializeField] protected float height;
        public Vector2 CurrentPosition 
        {
            get { return new Vector2(posX, posY); }
        } 
        public float CurrentWidth 
        {
            get { return width; }
        }
        public float CurrentHeight 
        {
            get { return height; }
        }


    #region WindowView functions

        protected virtual void Init() 
        {
            viewID = 0;
            visibleState = eVisibleState.Disappeared;

            if (! TryGetComponent<CanvasGroup>(out canvasGroup)) 
            {
                Debug.Log("CanvasGroup component 확인 요망");
                return;
            }

            if (! TryGetComponent<RectTransform>(out viewRect)) 
            {
                Debug.Log("RectTransform component 확인 요망");
                return;
            }

            isOffice = viewTag.Equals(S.SignInScene_ViewTag); 
            isWorld = viewTag.Equals(S.WorldScene_ViewTag);
        } 

        protected virtual void Reset() 
        {
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

            canvasGroup.DOFade(0f, fadeTime);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public async virtual UniTaskVoid Show() 
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

    #endregion  // WindowView functions
    }
}