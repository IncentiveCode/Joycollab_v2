/// <summary>
/// Square 에서 사용할 카메라 제어 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 03. 07 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성, mirror-test 에서 작업한 항목 migration.
/// </summary>

using UnityEngine;

namespace Joycollab.v2
{
    public class SquareCamera : MonoBehaviour
    {
        public static SquareCamera singleton { get; private set; }

        // 카메라, 크기 관련 변수
        private Camera mainCam;
        private Vector3 v3CameraPos;
        private float fSize, fZ;
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


    #region Unity functions

        private void Awake() 
        {
            singleton = this;

            mainCam = Camera.main;
            cameraMoveSpeed = 4f;
            isSet = false;

            v2SquareSize = new Vector2(20.48f, 11.52f);
            floorNo = 1;

            v2RoomSize = new Vector2(9.6f, 5.4f);
            roomNo = 0;
        }

        private void FixedUpdate()
        {
            if (! isSet) return;

            HandleCameraPosition(); 
        }

        private void OnDestroy() 
        {
            isSet = false;
            mainCam = null;
        }

    #endregion  // Unity functions


    #region Public functions

        public void UpdateCameraInfo(Transform player, float size, float padding=0f) 
        {
            distance = new Vector3(0f, padding, -10f);
            playerTransform = player;

            mainCam.orthographicSize = fSize = size;
            v3CameraPos = new Vector3(0f, 0f, -10f);
            fZ = v3CameraPos.z;

            fWidth = size * Screen.width / Screen.height;
            isSet = true;
        }

        public void Teleport(int no) 
        {
            if (floorNo == no) return;

            playerTransform.position = arrMapPos[no - 1];
            floorNo = no;
        }

        public void TeleportForRoom(int no) 
        {
            if (roomNo == no) return;

            if (no == 0)
            {
                playerTransform.position = arrMapPos[floorNo - 1];
            }
            else
            {
                playerTransform.position = arrRoomPos[no - 1];
            }

            roomNo = no;
        }

    #endregion  // public functions


    #region Private functions

        private void HandleCameraPosition() 
        {
            if (mainCam == null) return;

            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position,
                playerTransform.position + distance,
                Time.deltaTime * cameraMoveSpeed);

            float lx, clampX, ly, clampY;

            if (roomNo == 0)
            {
                lx = v2SquareSize.x - fWidth; 
                ly = v2SquareSize.y - fSize;

                clampY = Mathf.Clamp(mainCam.transform.position.y, 
                    -ly + ((floorNo == 1) ? 0 : 35), 
                    ly + ((floorNo == 1) ? 0 : 35)); 
                v3CameraPos.y = clampY;
            }
            else 
            {
                lx = v2RoomSize.x - fWidth;
                ly = v2RoomSize.y - fSize;

                float ty = ((roomNo - 1) * 15) + 60;
                clampY = Mathf.Clamp(mainCam.transform.position.y, 
                    -ly + ty, 
                    ly + ty); 
                v3CameraPos.y = clampY;
            }

            clampX = Mathf.Clamp(mainCam.transform.position.x, -lx, lx);
            v3CameraPos.x = clampX;
            v3CameraPos.z = fZ;

            mainCam.transform.position = v3CameraPos;
        }

    #endregion  // private functions
    }
}