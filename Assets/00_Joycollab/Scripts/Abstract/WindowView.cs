/// <summary>
/// 윈도우 형태의 창의 속성을 관리하기 위한 추상 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 11. 02
/// @version        : 0.3
/// @update
///     v0.1 (2023. 09. 12) : 최초 생성
///     v0.2 (2023. 11. 01) : minWidth, minHeight 추가. WindowViewData, Data Key 추가.
///     v0.3 (2023. 11. 02) : Show(int seq, Vector2 pos) 추가.
/// </summary>

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Joycollab.v2
{
    public class WindowView : MonoBehaviour, IPointerDownHandler
    {
        // for scene
        [TagSelector]
        [SerializeField] protected string viewTag;
        protected bool isOffice, isWorld, isMobile;

        // for view
        protected int viewID;
        protected WindowViewData viewData; 
        protected string viewDataKey;
        [SerializeField] protected bool isMultiple;
        public bool AllowMultiple 
        {
            get { return isMultiple; }
        }
        protected eVisibleState visibleState;
        public bool isDisappeared 
        {
            get { return visibleState == eVisibleState.Disappeared; }
        }

        protected CanvasGroup canvasGroup;
        protected RectTransform viewRect;
        protected float fadeTime = 0.5f;

        // for view state
        [SerializeField] protected float minWidth;
        [SerializeField] protected float minHeight;
        public float MinWidth => minWidth;
        public float MinHeight => minHeight;

        public Vector2 CurrentPosition 
        {
            get { return viewRect.anchoredPosition; }
        } 
        public float CurrentWidth 
        {
            get { return viewRect.rect.width; }
        }
        public float CurrentHeight 
        {
            get { return viewRect.rect.height; }
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
            isMobile = viewTag.Equals(S.MobileScene_ViewTag);
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

        public async virtual UniTaskVoid Show() 
        {
            visibleState = eVisibleState.Appearing;
            await UniTask.Yield();
        }
        
        public async virtual UniTaskVoid Show(int seq, Vector2 pos) 
        {
            visibleState = eVisibleState.Appearing;
            await UniTask.Yield();
        }

        public virtual void SetAsLastSibling() 
        {
            viewRect.SetAsLastSibling();
        }

        protected virtual void Appearing() 
        {
            if (canvasGroup == null) return;

            if (visibleState == eVisibleState.Appeared)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                return;
            }

            if (visibleState != eVisibleState.Appearing) 
                visibleState = eVisibleState.Appearing;

            canvasGroup.DOFade(1f, fadeTime);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            visibleState = eVisibleState.Appeared;
            viewRect.SetAsLastSibling();
        }

        protected virtual void SaveViewData(WindowViewData data)
        {
            if (viewData == null) return; 

            data.position = viewRect.anchoredPosition;
            data.size = viewRect.sizeDelta;
            PlayerPrefs.SetString(viewDataKey, data.ToJson());
        }

        protected virtual void LoadViewData() 
        {
            if (PlayerPrefs.HasKey(viewDataKey)) 
            {
                string temp = PlayerPrefs.GetString(viewDataKey, string.Empty);
                viewData.LoadFromJson(temp);
            }
            else 
            {
                viewData.Init(minWidth, minHeight);
            }

            // apply 'size & position'
            float width = Mathf.Clamp(viewData.size.x, minWidth, Screen.width);
            float height = Mathf.Clamp(viewData.size.y, minHeight, Screen.height);

            viewRect.sizeDelta = new Vector2(width, height);
            viewRect.anchoredPosition = new Vector2(
                Mathf.Clamp(viewData.position.x, (-width * 0.75f), Screen.width - width),
                Mathf.Clamp(viewData.position.y, (-height * 0.75f), Screen.height - height)
            );
        }

    #endregion  // WindowView functions


    #region Interface implementations

        public void OnPointerDown(PointerEventData data) 
        {
            viewRect.SetAsLastSibling();
        }

    #endregion  // Interface implementations
    }


    [Serializable]
    public class WindowViewData 
    {
        public Vector2 position;
        public Vector2 size;

        public WindowViewData()
        {
            position = size = Vector2.zero;
        }

        public void Init(float width, float height) 
        {
            position = new Vector2(
                Screen.width / 2 - width / 2,
                Screen.height / 2 - height / 2
            );
            size = new Vector2(width, height);

            string json = JsonUtility.ToJson(this);
            // Debug.Log($"WindowViewData | Init(), result : {json}");
        }

        public string ToJson() 
        {
            string json = JsonUtility.ToJson(this);
            // Debug.Log($"WindowViewData | ToJson(), result : {json}");
            return json; 
        }

        public void LoadFromJson(string json) 
        {
            // Debug.Log($"WindowViewData | LoadFromJson(), load data : {json}");
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }
}