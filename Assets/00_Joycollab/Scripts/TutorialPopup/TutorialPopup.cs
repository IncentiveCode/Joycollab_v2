/// <summary>
/// 튜토리얼 / 월드 미니맵을 출력하기 위한 팝업 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 11. 17
/// @version        : 0.1
/// @update
///     v0.1 (2023. 11. 17) : v1 에서 가져와서 수정 후 적용
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2 
{
    public class TutorialPopup : MonoBehaviour
    {
        private const string TAG = "TutorialPopup";

        [Header("Button")]
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnPrev;
        [SerializeField] private Button _btnNext;

        [Header("Info")]
        [SerializeField] private eTutorialType type;
        [SerializeField] private int _nPageCount;
        [SerializeField] private RawImage _imgPage;

        // local variables
        private int nCurrentPage;
        private string sCurrentKey;
        private Color tempColor;

        // width, height
        private RectTransform parentRect;
        private RectTransform rect;
        private float width;
        private float height;


    #region Unity functions

        private void Awake() 
        {
            // set button listener
            _btnClose.onClick.AddListener(() => Destroy(gameObject));

            _btnPrev.onClick.AddListener(() => {
                nCurrentPage = Mathf.Clamp(--nCurrentPage, 0, (_nPageCount-1));
                Load(nCurrentPage);

                _btnPrev.gameObject.SetActive(nCurrentPage > 0);
                _btnNext.gameObject.SetActive(nCurrentPage < (_nPageCount-1));
            });

            _btnNext.onClick.AddListener(() => {
                nCurrentPage = Mathf.Clamp(++nCurrentPage, 0, (_nPageCount-1));
                Load(nCurrentPage);

                _btnPrev.gameObject.SetActive(nCurrentPage > 0);
                _btnNext.gameObject.SetActive(nCurrentPage < (_nPageCount-1));
            });


            // set local variables
            parentRect = GetComponent<RectTransform>();
            rect = _imgPage.GetComponent<RectTransform>();
        }

    #endregion  // Unity functions


    #region Addressable image load

        public void Init(eTutorialType type) 
        {
            // info organize 
            switch (type) 
            {
                case eTutorialType.QuickLearning :
                    string memberType = R.singleton.myMemberType;
                    bool isAdmin = memberType.Equals(S.ADMIN) || memberType.Equals(S.OWNER);
                    _nPageCount = isAdmin ? 6 : 4;
                    Debug.Log($"{TAG} | Awake(), Office 주요기능 튜토리얼 설정. is admin : {isAdmin}, page count : {_nPageCount}");
                    break;

                case eTutorialType.MiniMap :
                    _nPageCount = 2;
                    Debug.Log($"{TAG} | World MiniMap 설정. page count : {_nPageCount}");
                    break;
                
                case eTutorialType.None :
                default : 
                    Debug.Log($"{TAG} | 더 이상 진행할 수 없습니다.");
                    break;
            }

            _btnPrev.gameObject.SetActive(false);
            _btnNext.gameObject.SetActive(true);

            nCurrentPage = 0;
            Load(nCurrentPage);
        }

        private void Load(int page) 
        {
            tempColor = _imgPage.color;
            tempColor.a = 0;
            _imgPage.color = tempColor;

            width = parentRect.rect.width - 32f;
            height = parentRect.rect.height - 32f;
            Debug.Log("TutorialPopup | width : "+ width +", height : "+ height);

            if (type == eTutorialType.QuickLearning)
                sCurrentKey = string.Format("{0}_{1:00}", R.singleton.Region, page);
            else
                sCurrentKey = string.Format("MiniMap_{0}f", page);

            LoadSprite(sCurrentKey);
        }

        private void LoadSprite(string key) 
        {
            Addressables.LoadAssetAsync<Texture2D>(key).Completed += OnLoadDone;
        }

        private void OnLoadDone(AsyncOperationHandle<Texture2D> handle) 
        {
            switch (handle.Status) 
            {
                case AsyncOperationStatus.Succeeded :
                    // _imgPage.sprite = handle.Result;
                    _imgPage.texture = handle.Result;
                    Util.ResizeRawImage(rect, _imgPage, width, height);

                    Appearing().Forget();
                    break;

                case AsyncOperationStatus.Failed :
                    Debug.LogError($"TutorialPopup | Sprite load fail. name : {sCurrentKey}");
                    break;

                default : break;
            }
        }

        private async UniTaskVoid Appearing() 
        {
            float minimumDuration = 0.5f;
            float time = 0f;
            float ratio = 0f;

            tempColor = _imgPage.color;

            while (tempColor.a < 1f) 
            {
                time += Time.deltaTime;
                ratio = time / minimumDuration;
                if (ratio >= 1f) break;

                tempColor.a = ratio;
                _imgPage.color = tempColor;
                await UniTask.Yield();
            }

            tempColor.a = 1f;
            _imgPage.color = tempColor;
        }

    #endregion  // Addressable image load
    }
}