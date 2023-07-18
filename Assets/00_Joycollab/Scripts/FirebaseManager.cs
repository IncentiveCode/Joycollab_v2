/// <summary>
/// Firebase 관련 매니저 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 18
/// @version        : 0.2
/// @update
///     v0.1 (2023. 06. 30) : 최초 작성.
///     v0.2 (2023. 07. 18) : singleton 으로 변경.
/// </summary>

using UnityEngine;
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
using Firebase;
#endif

namespace Joycollab.v2
{
    public class FirebaseManager : MonoBehaviour
    {
        private const string TAG = "FirebaseManager";
        public static FirebaseManager singleton { get; private set; }


#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        private FirebaseApp app;

    #region Unity functions

        private void Awake() 
        {
            InitSingleton();
        }

        private void Start() 
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var res = task.Result;
                if (res == DependencyStatus.Available) 
                {
                    app = Firebase.FirebaseApp.DefaultInstance;
                    Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                    Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

                    // Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
                }
                else 
                {
                    Debug.LogError(string.Format("Could not resolve all Firebase dependencies: {0}", res));
                }
            });
        }

    #endregion  // Unity functions


    #region Initialize

        private void InitSingleton() 
        {
            if (singleton != null && singleton == this) return;
            if (singleton != null) 
            {
                Destroy(gameObject);
                return;
            }

            singleton = this;
            DontDestroyOnLoad(gameObject);
        }

    #endregion  // Initialize
        

    #region for FCM

        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) 
        {
            Debug.Log($"{TAG} | OnTokenReceived(), Registration Token : {token.Token}");
        }

        public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) 
        {
            Debug.Log($"{TAG} | OnMessageReceived(), Received message from : {e.Message.From}");
        }

    #endregion  // for FCM

#endif 
    }
}