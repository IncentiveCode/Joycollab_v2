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
            } 
            else 
            {
                Debug.Log($"{TAG} | === Client Build ===");
                networkManager.StartClient();
            }
        }
    }
}