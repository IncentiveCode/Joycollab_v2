/// <summary>
/// SquareScene, 모임방 입구 문 처리를 위한 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 12. 07 
/// @version        : 0.2
/// @update
///     v0.1 (2023. 10. 24) : 최초 생성
///     v0.2 (2023. 12. 07) : OnTriggerEnter2D(), OnTriggerExit2D() 에 WorldPlayer 관련 코드 추가.
/// </summary>

using UnityEngine;
using DG.Tweening;

namespace Joycollab.v2
{
    public class RoomDoor : MonoBehaviour
    {
        private const string TAG = "RoomDoor";
        private const float TRANSITION_TIME = 0.5f;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Vector2 v2Close;
        [SerializeField] private Vector2 v2Open;
        
        
    #region Unity functions

        private void Awake()
        {
            if (spriteRenderer != null) return;
            
            Debug.Log($"{TAG} | SpriteRenderer 를 먼저 inspector 에서 설정해주세요.");
            return;
        }

        private void Start()
        {
            spriteRenderer.transform.DOLocalMove(v2Close, TRANSITION_TIME);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.tag.Equals("Player")) return;
            
            if (other.TryGetComponent<WorldAvatar>(out var mover))
            {
                if (mover.isOwned)
                {
                    spriteRenderer.transform.DOLocalMove(v2Open, TRANSITION_TIME).SetEase(Ease.OutQuart);
                }
            }
            else if (other.TryGetComponent<WorldPlayer>(out var player))
            {
                if (player.isOwned)
                {
                    spriteRenderer.transform.DOLocalMove(v2Open, TRANSITION_TIME).SetEase(Ease.OutQuart);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.tag.Equals("Player")) return;
                        
            if (other.TryGetComponent<WorldAvatar>(out var mover))
            {
                if (mover.isOwned)
                {
                    spriteRenderer.transform.DOLocalMove(v2Close, TRANSITION_TIME);
                }
            }
            else if (other.TryGetComponent<WorldPlayer>(out var player))
            {
                if (player.isOwned)
                {
                    spriteRenderer.transform.DOLocalMove(v2Close, TRANSITION_TIME);
                }
            }
        }

        #endregion  // Unity functions
    }
}