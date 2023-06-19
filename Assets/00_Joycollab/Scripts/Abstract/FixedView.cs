/// <summary>
/// 고정 위치를 가지는 창의 속성을 관리하기 위한 추상 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 12
/// @version        : 0.4
/// @update
///     v0.1 (2023. 03. 20) : 최초 생성
///     v0.2 (2023. 03. 21) : Show() 를 async 로 변경
///     v0.3 (2023. 05. 25) : DOTween test
///     v0.4 (2023. 06. 12) : softkeyboard 출력상태에서 back button 입력시 내용 사라지는 오류 수정.
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;

namespace Joycollab.v2
{
    public abstract class FixedView : MonoBehaviour
    {
        // for view
        protected int viewID;
        protected eVisibleState visibleState;
        protected CanvasGroup canvasGroup;
        protected RectTransform viewRect;
        protected float fadeTime = 1f;

        // for mobile keyboard
        protected bool keepOldTextInField;
        protected string oldText;
        protected string editText;


    #region FixedView functions

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

        public async virtual UniTaskVoid Show(string opt="") 
        {
            visibleState = eVisibleState.Appearing;

            // TODO. add 'Mobile Progress'

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

    #endregion  // FixedView functions


    #region Mobile inputfield 예외처리

        public virtual void SetInputFieldListener(TMP_InputField input) 
        {
            input.onValueChanged.AddListener(Editing);
            input.onSelect.AddListener(OnSelect);
            input.onDeselect.AddListener((value) => {
                if (keepOldTextInField) 
                {
                    input.text = oldText;
                    keepOldTextInField = false;
                }
            });
            input.onTouchScreenKeyboardStatusChanged.AddListener(ReportChangeStatus);
        }

        private void Editing(string currentText) 
        {
            oldText = editText;
            editText = currentText;
        }

        private void OnSelect(string currentText)
        {
            oldText = currentText;
        }

        private void ReportChangeStatus(TouchScreenKeyboard.Status status) 
        {
            if (status == TouchScreenKeyboard.Status.Canceled) 
            {
                keepOldTextInField = true;
            }
        }

    #endregion Mobile inputfield 예외처리
    }
}