/// <summary>
/// Firebase 관련 매니저 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 06. 30
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 30) : 최초 작성.
/// </summary>

using UnityEngine;
#if (UNITY_ANDROID || UNITY_IOS) && UNITY_EDITOR
using Firebase;
#endif

namespace Joycollab.v2
{
    public class FirebaseManager : MonoBehaviour
    {
#if (UNITY_ANDROID || UNITY_IOS) && UNITY_EDITOR
        private FirebaseApp app;

    #region Unity functions

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
        

    #region for FCM

        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) 
        {
            Debug.Log("Received Registration Token: " + token.Token);
        }

        public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) 
        {
            Debug.Log("Received a new message from: " + e.Message.From);
        }

    #endregion  // for FCM

#endif 
    }
}