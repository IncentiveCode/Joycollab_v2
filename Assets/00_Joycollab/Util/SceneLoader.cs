/// <summary>
/// Scene 전환 관리자
/// @author         : HJ Lee
/// @last update    : 2023. 11. 13
/// @version        : 0.3
/// @update
///     v0.1 (2023. 02. 22) : v1 에서 사용하던 클래스 정리.
///     v0.2 (2023. 10. 31) : world, square scene 을 사용하고 있는지 확인하는 함수 추가.
///     v0.3 (2023. 11. 13) : Map, Square, room scene 확인 함수 추가.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class SceneLoader : MonoBehaviour
    {
        // static variable, function
        public static eScenes nextScene;
        public static void Load(eScenes scene) 
        {
            nextScene = scene;
            SceneManager.LoadSceneAsync((int)eScenes.SceneLoader);
        }
        public static bool isWorld()
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            return (currentIndex == (int)eScenes.Map || currentIndex == (int)eScenes.Square || currentIndex == (int)eScenes.Room);
        }
        public static bool isMap() 
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            return currentIndex == (int)eScenes.Map;
        }
        public static bool isSquare()
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            return currentIndex == (int)eScenes.Square;
        }
        public static bool isRoom() 
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            return currentIndex == (int)eScenes.Room;
        }
        public static bool isGraphicUI() 
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            return currentIndex == (int)eScenes.GraphicUI;
        }

        [Header("UI")]
        [SerializeField] private Image _imgProgress;

        // local variable
        private const float minimumDuration = 1.0f;
        private float dummyLoadTime, dummyLoadRatio;
        private float loadRatio;


    #region Unity functions

        private void Start() 
        {
            LoadScene().Forget();
        }

        private void Update() 
        {
            _imgProgress.fillAmount = Mathf.MoveTowards(_imgProgress.fillAmount, loadRatio, 3 * Time.deltaTime);
        }

    #endregion  // Unity functions


    #region Scene loading

        private async UniTaskVoid LoadScene() 
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync((int) nextScene);
            operation.allowSceneActivation = false;

            dummyLoadTime = loadRatio = 0f;
            _imgProgress.fillAmount = 0f;

            while (! operation.isDone)
            {
                // dummy load timer
                dummyLoadTime += Time.deltaTime;
                dummyLoadRatio = dummyLoadTime / minimumDuration;

                // real load ratio
                loadRatio = Mathf.Min(operation.progress + 0.1f, dummyLoadRatio);
                // Debug.Log("Loading ... "+ (loadRatio * 100));

                if (loadRatio >= 1.0f) break;

                await UniTask.Yield();
            }

            // loading done. change scene.
            operation.allowSceneActivation = true;
        }

    #endregion  // Scene loading
    }
}