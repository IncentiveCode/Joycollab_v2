/// <summary>
/// Square, 모임방 예시를 위해 사용할 포인터 표시 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 12. 15 
/// @version        : 0.3
/// @update
///     v0.1 (2023. 10. 24) : 신규 생성
///     v0.2 (2023. 12. 07) : OnTriggerEnter2D() 에 WorldPlayer 관련 코드 추가.
///     v0.3 (2023. 12. 15) : data 가 없는 경우, 특정 모임방에서 커뮤니티 센터로 이동시킴.
/// </summary>

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Joycollab.v2
{
    public class RoomPointer : MonoBehaviour
    {
        private const string TAG = "RoomPointer";

        [Header("Default")]
        [SerializeField] private eRendererType _rendererType;
        [SerializeField] private bool _isMovable;

        [Header("Parameters")]
        [SerializeField] private RoomPointerData data;

        private int _direction = 1;
        private bool run;

        // position variables
        private RectTransform rect; 
        private Vector2 pos;
        private Vector3 rotate;
        private float currentY, minY, maxY;


    #region Unity functions

        private void Awake() 
        {
            if (_rendererType == eRendererType.UI_Image)
            {
                rect = GetComponent<RectTransform>();
                pos = rect.anchoredPosition;
                rotate = rect.eulerAngles;
            }
            else if (_rendererType == eRendererType.SpriteRenderer) 
            {
                pos = transform.position;
                rotate = transform.eulerAngles;
            }

            currentY = pos.y;

            if (data == null)
            {
                minY = maxY = currentY;
            }
            else 
            {
                minY = currentY - data.MoveLimit;
                maxY = currentY + data.MoveLimit;
            }
        }

        private void Update() 
        {
            if (! run) return;

            if (_rendererType == eRendererType.UI_Image)
            {
                rect.anchoredPosition = pos;
                rect.eulerAngles = rotate;
            }
            else if (_rendererType == eRendererType.SpriteRenderer) 
            {
                transform.position = pos;
                transform.eulerAngles = rotate;
            }

            if (data == null) return;

            if (data.Speed == 0) return;
            rotate.y += Time.deltaTime * 90f;

            if (data.MoveLimit == 0) return;
            pos.y += Time.deltaTime * data.Speed * _direction;
            
            if (pos.y >= maxY) _direction = -1;
            if (pos.y <= minY) _direction = 1;
        }

        private void OnEnable() 
        {
            run = _isMovable;
        }

        private void OnDisable() 
        {
            run = false;     
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.tag.Equals("Player")) return;

            if (other.TryGetComponent<WorldPlayer>(out var player))
            {
                if (data == null) 
                {
                    Debug.Log($"{TAG} | 특정 모임방에서 커뮤니티 센터로 이동.");

                    MultiSceneNetworkManager.singleton.StopClient();

                    int.TryParse(JsLib.GetCookie(Key.CENTER_SEQ), out int seq);
                    WorldPlayer.localPlayerInfo.workspaceSeq = seq;
                    WorldPlayer.localPlayerInfo.roomTypeId = string.Empty;
                    SceneLoader.Load(eScenes.Square);
                }
                else
                {
                    if (player.isOwned)
                    {
                        SquareCamera.singleton.TeleportForRoom(data.RoomNo, data.Target).Forget();
                    }
                }
            }
        }

        #endregion
    }
}