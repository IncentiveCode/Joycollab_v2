/// <summary>
/// [mirror] square scene 에서 자동으로 서버/클라이언트 실행을 시켜주는 도구.
/// @author         : HJ Lee
/// @last update    : 2023. 11. 02.
/// @version        : 0.1
/// @update
///     v0.1 : 최초 생성
/// </summary>

using UnityEngine;
using Mirror;

namespace Joycollab.v2
{
    public class AutoConnecter : MonoBehaviour
    {
        private const string TAG = "AutoConnecter";

        [SerializeField] private NetworkManager networkManager;

        private void Start()
        {
            if (Application.isBatchMode) 
            {
                Debug.Log($"{TAG} | === Server Build ===");
                networkManager.StartServer();
            } 
            else 
            {
                Debug.Log($"{TAG} | === Client Build ===");
                networkManager.StartClient();

                // 약간의 로딩 시간을 추가
                var pop = Instantiate(SystemManager.singleton.pfLoadingProgress, Vector3.zero, Quaternion.identity);
                var transform = GameObject.Find(S.Canvas_Popup).GetComponent<Transform>();
                pop.transform.SetParent(transform, false);
            }
        }
    }
}