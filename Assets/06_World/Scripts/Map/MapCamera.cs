/// <summary>
/// Map 에서 사용할 카메라 제어 클래스 
/// @author         : HJ Lee
/// @last update    : 2024. 01. 02 
/// @version        : 0.4
/// @update
///     v0.1 (2023. 08. 31) : v1 에서 사용하던 항목 수정 후 적용.
///     v0.2 (2023. 09. 21) : class name 변경. (WorldCamera -> MapCamera)
///     v0.3 (2023. 10. 25) : WebGLSupport 의 ResizeEvent 추가.
///     v0.4 (2024. 01. 02) : camera 설정 변경. (배경 외부가 보이지 않게끔 수정)
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using WebGLSupport;

namespace Joycollab.v2
{
    public class MapCamera : MonoBehaviour
    {
        private const string TAG = "MapCamera";

        public static MapCamera singleton { get; private set ; }

        // for camera, camera position
        private Camera mainCamera;
        private Vector3 v3CameraPos;
        private float fZ;

        // const value (fMaxSize 는 화면 비율에 따라 변경되게 수정)
        private const float fMinSize = 3f;
        private float fMaxSize;

        // for map info
        [SerializeField] private Image imgMap;
        private Vector2 v2MapSize;

        // ratio
        private float screenRatio, targetRatio;

        // for mouse event
        private float fSize, fLastSize;
        private float fWidth;
        private Vector3 v3MouseOrigin, v3Drag;


    #region Unity functions

        private void Awake() 
        {
            singleton = this;

            mainCamera = GetComponent<Camera>();
            // v2MapSize = new Vector2(20.48f, 13.08f);
            v2MapSize = new Vector2(
                imgMap.rectTransform.rect.size.x - 6,
                imgMap.rectTransform.rect.size.y - 6
            );
            v2MapSize *= 0.005f;
            Debug.Log($"{TAG} | map size : {v2MapSize}");

            // add resize event
            WebGLWindow.OnResizeEvent += OnResize;
        }

        private void OnEnable() 
        {
            // calculate ratio
            CalculateRatio();

            // load orthographic size
            float prefSize = PlayerPrefs.GetFloat(Key.ORTHOGRAPHIC_SIZE_IN_MAP, 8f);
            prefSize = Mathf.Clamp(prefSize, fMinSize, fMaxSize);
            mainCamera.orthographicSize = fLastSize = fSize = prefSize;
            mainCamera.transform.position = new Vector3(1.477197f, 1.103448f, -10f);

            // calculate width
            CalculateWidth();

            // camera setting
            v3CameraPos = mainCamera.transform.position;
            fZ = v3CameraPos.z;
            SetCameraLimit();
        }

        private void OnDisable() 
        {
            PlayerPrefs.SetFloat(Key.ORTHOGRAPHIC_SIZE_IN_MAP, mainCamera.orthographicSize); 
        }

        private void OnDestroy() 
        {
            WebGLWindow.OnResizeEvent -= OnResize;
        }

    #endregion


    #region call by WorldPointerEventHandler

        public void HandleWheelEvent(float value) 
        {
            if (value == 0f) return;

            fSize += (value > 0) ? -0.2f : 0.2f;
            fSize = Mathf.Clamp(fSize, fMinSize, fMaxSize);

            if (fSize != fLastSize) 
            {
                mainCamera.orthographicSize = fLastSize = fSize;
                fWidth = fSize * screenRatio;
                SetCameraLimit();
            }
        }

        public void HandleBeginDrag(Vector2 pos) 
        {
            v3MouseOrigin = mainCamera.ScreenToWorldPoint(pos);
        }

        public void HandleDrag(Vector2 pos, bool isEnd=false) 
        {
            v3Drag = mainCamera.ScreenToWorldPoint(pos);
            v3Drag = mainCamera.transform.position - (v3Drag - v3MouseOrigin);
            v3Drag.z = fZ;

            mainCamera.transform.position = v3Drag;
            SetCameraLimit();

            if (isEnd) 
            {
                // TODO. 마지막 위치 PlayerPrefs 에 기록.
            }
        }

    #endregion  // call by WorldPointerEventHandler


    #region camera option

        private void CalculateRatio() 
        {
            screenRatio = (float) Screen.width / (float) Screen.height;
            targetRatio = v2MapSize.x / v2MapSize.y;
            Debug.Log($"{TAG} | screen ratio : {screenRatio}, target ratio : {targetRatio}");
            if (screenRatio >= targetRatio)
            {
                fMaxSize = v2MapSize.y * 0.5f;
            }
            else
            {
                float differenceInSize = targetRatio / screenRatio;
                Debug.Log($"{TAG} | difference in size : {differenceInSize}");
                fMaxSize = v2MapSize.y * 0.5f * differenceInSize;
            }
            Debug.Log($"{TAG} | max size : {fMaxSize}");
        }

        private void CalculateWidth() 
        {
            float width = fMaxSize * screenRatio;
            Debug.Log($"{TAG} | max width ? {width}");
            if (width > v2MapSize.x) 
            {
                width = v2MapSize.x;
                float size = width / screenRatio;
                fMaxSize = size;
            }

            fWidth = fSize * screenRatio;
            Debug.Log($"{TAG} | current width ? {fWidth}");
            if (fWidth > v2MapSize.x) 
            {
                fWidth = v2MapSize.x;
                float size = fWidth / screenRatio;
                mainCamera.orthographicSize = fLastSize = fSize = size;
            }
        }

        private void SetCameraLimit() 
        {
            float lx = v2MapSize.x - fWidth;
            if (lx < 0) lx = 0f; 
            float clampX = Mathf.Clamp(mainCamera.transform.position.x, -lx, lx);
            v3CameraPos.x = clampX;

            float ly = v2MapSize.y - fLastSize;
            if (ly < 0) ly = 0f;
            float clampY = Mathf.Clamp(mainCamera.transform.position.y, -ly, ly);
            v3CameraPos.y = clampY;

            v3CameraPos.z = fZ;
            mainCamera.transform.position = v3CameraPos;
        }

    #endregion  // camera option


    #region webgl support callback

        private void OnResize() 
        {
            // calculate ratio
            CalculateRatio();

            // calculate width
            CalculateWidth();
            
            // camera setting
            SetCameraLimit();
        }

    #endregion  // webgl support callback
    }
}