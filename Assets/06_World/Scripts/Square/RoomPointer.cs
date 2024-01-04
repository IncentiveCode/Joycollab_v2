/// <summary>
/// Square, 모임방 예시를 위해 사용할 포인터 표시 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 12. 22 
/// @version        : 0.4
/// @update
///     v0.1 (2023. 10. 24) : 신규 생성
///     v0.2 (2023. 12. 07) : OnTriggerEnter2D() 에 WorldPlayer 관련 코드 추가.
///     v0.3 (2023. 12. 15) : data 가 없는 경우, 특정 모임방에서 커뮤니티 센터로 이동시킴.
///     v0.4 (2023. 12. 22) : 커뮤니티 센터로 이동하는 함수 추가.
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;

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
                    OnExitAsync().Forget();
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


    #region other function

        private async UniTaskVoid OnExitAsync() 
        {
            // TODO. 정보 확보
            PsResponse<ResWorkspaceList> res = await NetworkTask.RequestAsync<ResWorkspaceList>(URL.WORKSPACE_LIST, eMethodType.GET, string.Empty, R.singleton.token);
            if (! string.IsNullOrEmpty(res.message))
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return;
            }

            int nextSpaceSeq = 0;
            int nextMemberSeq = 0;
            string nextRoomType = string.Empty;
            foreach (var space in res.data.list) 
            {
                if (space.workspace.workspaceType.Equals(S.WORLD)) 
                {
                    nextSpaceSeq = space.workspace.seq;
                    nextMemberSeq = space.seq;
                    nextRoomType = space.workspace.clas.themes.id;
                    break;
                }
            }

            if (nextSpaceSeq == 0 && nextMemberSeq == 0) 
            {
                PopupBuilder.singleton.OpenAlert("커뮤니티 센터 정보를 확인할 수 없습니다.");
                return;
            }


            // TODO. 멤버 정보 확인
            string url = string.Format(URL.MEMBER_INFO, nextMemberSeq);
            PsResponse<ResMemberInfo> res2 = await NetworkTask.RequestAsync<ResMemberInfo>(url, eMethodType.GET, string.Empty, R.singleton.token);
            if (! string.IsNullOrEmpty(res2.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res2.message);
                return;
            }


            // TODO. 정보 반영
            R.singleton.MemberInfo = res2.data;
            switch (res2.data.lan.id) 
            {
                case S.REGION_KOREAN :
                    R.singleton.ChangeLocale(ID.LANGUAGE_KOREAN);
                    break;

                case S.REGION_JAPANESE :
                    R.singleton.ChangeLocale(ID.LANGUAGE_JAPANESE);
                    break;
                
                default :
                    R.singleton.ChangeLocale(ID.LANGUAGE_ENGLISH);
                    break;
            }    

            // Debug.Log($"{TAG} | current font size : {res.data.fontSize}");
            R.singleton.FontSizeOpt = res2.data.fontSize;
            R.singleton.ID = res2.data.user.id;

            WorldAvatarInfo info = new WorldAvatarInfo(res2.data);
            info.workspaceSeq = nextSpaceSeq;
            info.roomTypeId = nextRoomType;
            WorldPlayer.localPlayerInfo = info;


            // TODO. 이전 정보 처리
            SystemManager.singleton.BeforeMoveGroup(); 
            MultiSceneNetworkManager.singleton.StopClient();


            // TODO. 정보 저장
            JsLib.SetCookie(Key.WORKSPACE_SEQ, nextSpaceSeq.ToString());
            JsLib.SetCookie(Key.MEMBER_SEQ, nextMemberSeq.ToString());

            // Repo.Instance.workspaceSeq = nextSpaceSeq;
            // Repo.Instance.memberSeq = nextMemberSeq;
            R.singleton.workspaceSeq = nextSpaceSeq;
            R.singleton.memberSeq = nextMemberSeq;


            // TODO. 후속 조치
            SystemManager.singleton.AfterMoveGroup();
            SceneLoader.Load(eScenes.Square);
        }

    #endregion  // other function
    }
}