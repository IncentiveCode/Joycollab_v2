/// <summary>
/// Square 에서 사용할 카메라 제어 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 12. 08 
/// @version        : 0.5
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성, mirror-test 에서 작업한 항목 migration.
///     v0.2 (2023. 10. 24) : Teleport 시 fade out / fade in 추가.
///     v0.3 (2023. 10. 26) : WebGLSupport 의 ResizeEvent 추가.
///     v0.4 (2023. 11. 01) : fade out / fade in 수치 조정.
///     v0.5 (2023. 12. 08) : 마우스 휠 이벤트 추가.
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using WebGLSupport;

namespace Joycollab.v2
{
    public class SquareCamera : MonoBehaviour
    {
        // const value
        private const string TAG = "SquareCamera";
        private const float fMinSize = 3f;
        private const float fMaxSize = 8f;

        public static SquareCamera singleton { get; private set; }

        // 카메라, 크기 관련 변수
        private Camera mainCamera;
        private Vector3 v3CameraPos;
        private float fSize, fLastSize, fZ;
        private float cameraMoveSpeed;
        private float fWidth;

        // 카메라가 바라보는 아바타 관련 변수
        private Vector3 distance;
        private Transform playerTransform;

        // 임시 변수
        private bool isSet;
        
        // for map info
        private Vector2 v2SquareSize;
        private int floorNo;
        [SerializeField] private Vector3[] arrMapPos;

        // for room info
        private Vector2 v2RoomSize;
        private int roomNo;
        [SerializeField] private Vector3[] arrRoomPos;
        
        // for blocker
        private const float TIME = 0.8f;
        private const int DELAY_TIME = 500;
        private bool isMove;
        [SerializeField] private CanvasGroup blocker;


    #region Unity functions

        private void Awake() 
        {
            singleton = this;

            mainCamera = GetComponent<Camera>();
            cameraMoveSpeed = 4f;
            isSet = isMove = false;

            v2SquareSize = new Vector2(30.72f, 17.28f);
            floorNo = 1;

            v2RoomSize = new Vector2(14.4f, 8.1f);
            roomNo = 0;
            
            blocker.DOFade(0f, TIME);
            blocker.interactable = false;
            blocker.blocksRaycasts = false;

            WebGLWindow.OnResizeEvent += OnResize;
        }

        private void FixedUpdate()
        {
            if (! isSet) return;

            HandleCameraPosition(); 
        }

        private void OnDestroy() 
        {
            isSet = false;
            floorNo = roomNo = 0;
            mainCamera = null;

            WebGLWindow.OnResizeEvent -= OnResize;
        }

    #endregion  // Unity functions


    #region Public functions

        public void UpdateCameraInfo(Transform player, float size, float padding=0f) 
        {
            distance = new Vector3(0f, padding, -10f);
            playerTransform = player;

            float prefSize = PlayerPrefs.GetFloat(Key.ORTHOGRAPHIC_SIZE_IN_SQUARE, size);
            prefSize = Mathf.Clamp(prefSize, fMinSize, fMaxSize);
            mainCamera.orthographicSize = fLastSize = fSize = prefSize;

            v3CameraPos = new Vector3(0f, 0f, -10f);
            fZ = v3CameraPos.z;

            fWidth = size * Screen.width / Screen.height;
            isSet = true;
        }

        public async UniTaskVoid Teleport(int no)
        {
            if (isMove) return;
            if (floorNo == no) return;

            isMove = true;
            blocker.DOFade(1f, TIME);
            blocker.interactable = true;
            blocker.blocksRaycasts = true;
            
            await UniTask.Delay(DELAY_TIME);
            playerTransform.position = arrMapPos[no - 1];
            floorNo = no;
            roomNo = 0;
            await UniTask.Delay(DELAY_TIME);

            blocker.DOFade(0f, TIME);
            blocker.interactable = false;
            blocker.blocksRaycasts = false;
            isMove = false;
        }

        public async UniTaskVoid TeleportForRoom(int no)
        {
            if (isMove) return;
            if (roomNo == no) return;

            isMove = true;
            blocker.DOFade(1f, TIME);
            blocker.interactable = true;
            blocker.blocksRaycasts = true;
            
            await UniTask.Delay(DELAY_TIME);
            playerTransform.position = no == 0 ? arrMapPos[floorNo - 1] : arrRoomPos[no - 1];
            roomNo = no;
            await UniTask.Delay(DELAY_TIME);
            
            blocker.DOFade(0f, TIME);
            blocker.interactable = false;
            blocker.blocksRaycasts = false;
            isMove = false;
        }

        public async UniTaskVoid TeleportForRoom(int no, Vector3 pos)
        {
            if (isMove) return;
            if (roomNo == no) return;
            
            isMove = true;
            blocker.DOFade(1f, TIME);
            blocker.interactable = true;
            blocker.blocksRaycasts = true;

            await UniTask.Delay(DELAY_TIME);
            playerTransform.position = pos;
            roomNo = no;
            await UniTask.Delay(DELAY_TIME);
            
            blocker.DOFade(0f, TIME);
            blocker.interactable = false;
            blocker.blocksRaycasts = false;
            isMove = false; 
        }

    #endregion  // public functions


    #region Private functions

        private void HandleCameraPosition() 
        {
            if (mainCamera == null) return;
            if (mainCamera.transform == null) return;

            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position,
                playerTransform.position + distance,
                Time.deltaTime * cameraMoveSpeed);

            float lx, clampX, ly, clampY;

            if (roomNo == 0)
            {
                lx = v2SquareSize.x - fWidth; 
                ly = v2SquareSize.y - fSize;

                clampY = Mathf.Clamp(mainCamera.transform.position.y, 
                    -ly + ((floorNo == 1) ? 0 : 45), 
                    ly + ((floorNo == 1) ? 0 : 45)); 
                v3CameraPos.y = clampY;
            }
            else 
            {
                lx = v2RoomSize.x - fWidth;
                ly = v2RoomSize.y - fSize;

                float ty = ((roomNo - 1) * 30) + 80;
                clampY = Mathf.Clamp(mainCamera.transform.position.y, 
                    -ly + ty, 
                    ly + ty); 
                v3CameraPos.y = clampY;
            }

            clampX = Mathf.Clamp(mainCamera.transform.position.x, -lx, lx);
            v3CameraPos.x = clampX;
            v3CameraPos.z = fZ;

            mainCamera.transform.position = v3CameraPos;
        }

    #endregion  // private functions


    #region call by WorldPlayer

        public void HandleWheelEvent(float value) 
        {
            if (value == 0f) return; 

            fSize += (value > 0) ? -0.2f : 0.2f;
            fSize = Mathf.Clamp(fSize, fMinSize, fMaxSize);

            if (fSize != fLastSize) 
            {
                mainCamera.orthographicSize = fLastSize = fSize; 
                fWidth = fSize * Screen.width / Screen.height;
            }
        }

    #endregion  // call by WorldPlayer


    #region webgl support callback

        private void OnResize() 
        {
            Debug.Log($"{TAG} | OnResize(), width : {Screen.width}, height : {Screen.height}");
            fWidth = fSize * Screen.width / Screen.height;
        }

    #endregion  // webgl support callback
    }
}