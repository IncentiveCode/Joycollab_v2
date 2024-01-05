/// <summary>
/// [mirror] square scene 에서 자동으로 서버/클라이언트 실행을 시켜주는 도구.
/// @author         : HJ Lee
/// @last update    : 2024. 01. 05.
/// @version        : 0.2
/// @update
///     v0.1 (2023. 12. 14) : 최초 생성
///     v0.2 (2024. 01. 05) : release mode 일 때는 GUI 를 끄고, test mode 일 때는 GUI 를 키도록 수정.
/// </summary>

using UnityEngine;
using Mirror;

namespace Joycollab.v2
{
    public class AutoConnecter : MonoBehaviour
    {
        private const string TAG = "AutoConnecter";

        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private Mirror.SimpleWeb.SimpleWebTransport transport;
        [SerializeField] private NetworkManagerHUD networkManagerHud;
        [SerializeField] private bool releaseMode;

        private void Start()
        {
            transport.clientUseWss = releaseMode;
            transport.sslEnabled = releaseMode;

            if (Application.isBatchMode) 
            {
                Debug.Log($"{TAG} | === Server Build ===");
                networkManager.networkAddress = "localhost";
                networkManager.StartServer();
            } 
            else 
            {
                Debug.Log($"{TAG} | === Client Build ===");
                networkManager.networkAddress = releaseMode ? "dev.jcollab.com" : "localhost";
                networkManager.StartClient();

                networkManagerHud.enabled = !releaseMode;

                // 약간의 로딩 시간을 추가
                var pop = Instantiate(SystemManager.singleton.pfLoadingProgress, Vector3.zero, Quaternion.identity);
                var transform = GameObject.Find(S.Canvas_Popup).GetComponent<Transform>();
                pop.transform.SetParent(transform, false);
            }
        }
    }
}