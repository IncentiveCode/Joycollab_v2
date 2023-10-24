/// <summary>
/// SquareScene, 모임방 입구 문 처리를 위한 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 10. 24 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 10. 24) : 최초 생성
/// </summary>

using UnityEngine;
using DG.Tweening;

namespace Joycollab.v2
{
    public class RoomDoor : MonoBehaviour
    {
        private const string TAG = "RoomDoor";
        private const float TRANSITION_TIME = 0.5f;

        [SerializeField] private SpriteRenderer renderer;
        [SerializeField] private Vector2 v2Close;
        [SerializeField] private Vector2 v2Open;
        
        
    #region Unity functions

        private void Awake()
        {
            if (renderer != null) return;
            
            Debug.Log($"{TAG} | SpriteRenderer 를 먼저 inspector 에서 설정해주세요.");
            return;
        }

        private void Start()
        {
            renderer.transform.DOLocalMove(v2Close, TRANSITION_TIME);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.tag.Equals("Player")) return;
            
            var mover = other.GetComponent<WorldAvatar>();
            if (mover != null && mover.isOwned)
            {
                renderer.transform.DOLocalMove(v2Open, TRANSITION_TIME).SetEase(Ease.OutQuart);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.tag.Equals("Player")) return;
                        
            var mover = other.GetComponent<WorldAvatar>();
            if (mover != null && mover.isOwned)
            {
                renderer.transform.DOLocalMove(v2Close, TRANSITION_TIME);
            }
        }

        #endregion  // Unity functions
    }
}