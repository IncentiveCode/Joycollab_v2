/// <summary>
/// loading 창을 출력하는 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 12. 18
/// @version        : 0.1
/// @update
///     v0.1 (2023. 12. 18) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Joycollab.v2
{
    public class LoadingProgress : MonoBehaviour
    {
        private const string TAG = "LoadingProgress";

        [Header("UI")]
        [SerializeField] private Image _imgProgress;

        // local variable
        private CanvasGroup canvasGroup;

        private const float minimumDuration = 1.0f;
        private const float fadeInTime = 0.1f;
        private const float fadeOutTime = 1.0f;
        private float loadTime, loadRatio;


    #region Unity functions

        private void Awake() 
        {
            if (! TryGetComponent<CanvasGroup>(out canvasGroup)) 
            {
                Debug.Log($"{TAG} | canvas group 이 설정되어 있지 않음");
            }
        }

        private void Start() 
        {
            canvasGroup.DOFade(1f, fadeInTime);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            JustLoading(minimumDuration).Forget();
        }

        private void Update() 
        {
            _imgProgress.fillAmount = Mathf.MoveTowards(_imgProgress.fillAmount, loadRatio, 3 * Time.deltaTime);
        }

    #endregion  // Unity functions


    #region just loading

        private async UniTaskVoid JustLoading(float time)
        {
            loadTime = loadRatio = 0f;
            _imgProgress.fillAmount = 0f;

            while (true)
            {
                // dummy load timer
                loadTime += Time.deltaTime;
                loadRatio = loadTime / time;
                if (loadRatio >= 1.0f) break;

                await UniTask.Yield();
            }

            canvasGroup.DOFade(0f, fadeOutTime);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            await UniTask.Delay((int)(fadeOutTime * 1000));

            Destroy(this.gameObject);
        }

    #endregion  // just loading
    }
}