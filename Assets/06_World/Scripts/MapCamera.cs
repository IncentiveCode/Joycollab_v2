/// <summary>
/// Map 에서 사용할 카메라 제어 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 10. 25 
/// @version        : 0.3
/// @update
///     v0.1 (2023. 08. 31) : v1 에서 사용하던 항목 수정 후 적용.
///     v0.2 (2023. 09. 21) : class name 변경. (WorldCamera -> MapCamera)
///     v0.3 (2023. 10. 25) : WebGLSupport 의 ResizeEvent 추가.
/// </summary>

using UnityEngine;
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

        // const value
        private const float fMinSize = 3f;
        private const float fMaxSize = 10.4f;

        // for map info
        private Vector2 v2MapSize;

        // for mouse event
        private float fSize, fLastSize;
        private float fWidth;
        private Vector3 v3MouseOrigin, v3Drag;


    #region Unity functions

        private void Awake() 
        {
            singleton = this;

            mainCamera = GetComponent<Camera>();
            v2MapSize = new Vector2(20.48f, 13.08f);

            WebGLWindow.OnResizeEvent += OnResize;
        }

        private void OnEnable() 
        {
            float prefSize = PlayerPrefs.GetFloat(Key.ORTHOGRAPHIC_SIZE_W, 8f);
            prefSize = Mathf.Clamp(prefSize, fMinSize, fMaxSize);
            mainCamera.orthographicSize = fLastSize = fSize = prefSize;
            mainCamera.transform.position = new Vector3(1.477197f, 1.103448f, -10f);

            v3CameraPos = mainCamera.transform.position;
            fZ = v3CameraPos.z;

            fWidth = fSize * Screen.width / Screen.height;
            SetCameraLimit();
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
                fWidth = fSize * Screen.width / Screen.height;

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


    #region camera limit

        private void SetCameraLimit() 
        {
            float lx = v2MapSize.x - fWidth;
            float clampX = Mathf.Clamp(mainCamera.transform.position.x, -lx, lx);
            v3CameraPos.x = clampX;

            float ly = v2MapSize.y - fLastSize;
            float clampY = Mathf.Clamp(mainCamera.transform.position.y, -ly, ly);
            v3CameraPos.y = clampY;

            v3CameraPos.z = fZ;

            mainCamera.transform.position = v3CameraPos;
        }

    #endregion


    #region webgl support callback

        private void OnResize() 
        {
            Debug.Log($"{TAG} | OnResize(), width : {Screen.width}, height : {Screen.height}");
            fWidth = fSize * Screen.width / Screen.height;
            SetCameraLimit();
        }

    #endregion  // webgl support callback
    }
}