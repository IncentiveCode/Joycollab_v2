/// <summary>
/// SquareScene, 천장 같은 object 아래에 아바타가 들어오면 반투명하게 출력하는 역할을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 03. 13 
/// @version        : 0.3
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성, mirror-test 에서 작업한 항목 migration.
///     v0.2 (2023. 03. 10) : alpha value 와 SpriteRenderer targer 을 inspector 에서 조절 가능하도록 수정.
///     v0.3 (2023. 03. 13) : alpha value 뿐 아니라 색상으로 입력/조절 가능하도록 수정.
/// </summary>

using UnityEngine;

namespace Joycollab.v2
{
    public class ObjectAlphaTrigger : MonoBehaviour
    {
        [SerializeField] 
        private Color colorOnEnter;
        [SerializeField] 
        private Color colorOnExit;
        [SerializeField] 
        private SpriteRenderer[] arrRenderers;


        private void Awake() 
        {
            if (arrRenderers.Length == 0) 
            {
                SpriteRenderer spriteRenderer;
                if (TryGetComponent<SpriteRenderer>(out spriteRenderer))
                {
                    arrRenderers[0] = spriteRenderer;
                }
                Debug.Log("WorldObjectTrigger | SpriteRenderer 를 먼저 inspector 에서 설정해주세요.");
            }
        }

        private void Start() 
        {
            foreach (SpriteRenderer spriteRenderer in arrRenderers)
                spriteRenderer.color = colorOnExit;
        }

        private void OnTriggerEnter2D(Collider2D other) 
        {
            if (arrRenderers.Length == 0) return;

            if (other.tag.Equals("Player")) 
            {
                var mover = other.GetComponent<WorldAvatar>();
                if (mover != null && mover.isOwned)
                {
                    foreach (SpriteRenderer spriteRenderer in arrRenderers)
                        spriteRenderer.color = colorOnEnter;
                }
            }
        }    

        private void OnTriggerExit2D(Collider2D other) 
        {
            if (arrRenderers.Length == 0) return;

            if (other.tag.Equals("Player"))
            {
                var mover = other.GetComponent<WorldAvatar>();
                if (mover != null && mover.isOwned)
                {
                    foreach (SpriteRenderer spriteRenderer in arrRenderers)
                        spriteRenderer.color = colorOnExit;
                }
            }
        }
    }
}