/// <summary>
/// [mobile]
/// Slide popup class
/// @author         : HJ Lee
/// @last update    : 2023. 03. 30
/// @version        : 0.2
/// @update
///     v0.1 (2022. 03. 07) : 최초 생성 및 기본 작업. 
///     v0.2 (2023. 03. 30) : UI 및 script 최적화. 
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
{
    public class SlidePopupM : MonoBehaviour
    {
        [Header("control")]
        private CanvasGroup group;
        [SerializeField] private Button _btnBackground;
        [SerializeField] private BackAndForth _panel;
        [SerializeField] private float _duration;

        [Header("info")]
        [SerializeField] private Transform _transformList;
        [SerializeField] private GameObject _goOption;
        [SerializeField] private Text _txtTitle;

        // local variables
        private float timer;
        private bool isFade;
        private bool isOpenDone;
        private bool isCloseDone;
        private bool reqDestroy;
        private float startAlpha;


    #region Unity functions
        private void Awake() 
        {
            timer = 0f;
            isFade = true;
            isOpenDone = false;
            isCloseDone = false;   
            reqDestroy = false;

            group = GetComponent<CanvasGroup>();
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
            startAlpha = group.alpha;

            _duration = (_duration == 0f) ? 0.1f : _duration;
            _panel.gameObject.SetActive(false);
        }

        private void Update() 
        {
            // Close -> Open
            if (isFade) 
            {
                if (! isOpenDone) 
                {
                    timer += Time.deltaTime;
                    if (timer < _duration)
                    {
                        // _group.alpha = Mathf.Lerp(startAlpha, 1.0f, timer / _duration);
                    }
                    else 
                    {
                        timer = 0f;
                        group.alpha = 1.0f;
                        group.interactable = true;
                        group.blocksRaycasts = true;

                        isOpenDone = true;
                        isFade = false;

                        OpenPopup();
                    }
                }
            }
            // Open -> Close
            else 
            {
                if (! isCloseDone) 
                {
                    timer += Time.deltaTime;
                    if (timer < _duration) 
                    {
                        // _group.alpha = Mathf.Lerp(startAlpha, 0.0f, timer / _duration);
                    }
                    else 
                    {
                        timer = 0f;
                        group.alpha = 0.0f;
                        group.interactable = false;
                        group.blocksRaycasts = false;

                        isCloseDone = true;
                        isFade = true;

                        reqDestroy = true;
                    }
                }
            }

            // destroy request
            if (reqDestroy) 
            {
                timer += Time.deltaTime;
                if (timer < _duration * 2) 
                {
                    Destroy(gameObject);
                }
            }
        }
    #endregion  // unity functions
        

    #region public functions
        public void InitPopup(string title, string[] param, bool cancelable = true) 
        {
            RectTransform currRect = _panel.GetComponent<RectTransform>();
            float width = currRect.rect.width;
            float height = 112 + (param.Length * 40) + 32;
            height = Mathf.Min(480f, height);
            currRect.sizeDelta = new Vector2(width, height);

            StartCoroutine(_panel.Move(false));

            _btnBackground.onClick.RemoveAllListeners();
            _btnBackground.interactable = cancelable;
            if (cancelable) _btnBackground.onClick.AddListener(() => RequestClose());

            // 정보 세팅.
            _txtTitle.text = title;
            ClearList();
            InstantiateOptions(param);

            // 출력.
            isOpenDone = false;
            isCloseDone = true;
        }

        public void RequestClose() 
        {
            StartCoroutine(_panel.Move(false));

            isOpenDone = true;
            isCloseDone = false;
        }
    #endregion  // public functions


    #region private functions
        private void OpenPopup() 
        {
            _panel.gameObject.SetActive(true);
            StartCoroutine(_panel.Move(true));
        }

        private void ClearList() 
        {
            Transform children = _transformList.GetComponentInChildren<Transform>();
            foreach (Transform child in children) 
            {
                if (child.name.Equals(_transformList.name)) continue;
                Destroy(child.gameObject);
            } 
        }

        private void InstantiateOptions(string[] param) 
        {
            for (int i = 0; i < param.Length; i++) 
            {
                int index = i;
                var item = Instantiate(_goOption, Vector3.zero, Quaternion.identity);
                item.GetComponent<Button>().onClick.AddListener(() => {
                    AndroidSelectCallback.SelectedIndex = index;
                    AndroidSelectCallback.isUpdated = true;

                    RequestClose();
                });
                item.GetComponentInChildren<Text>().text = param[i];
                item.transform.SetParent(_transformList, false);
            }
        }
    #endregion  // private functions
    }
}