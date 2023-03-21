using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidBridge : MonoBehaviour
{
    const string pluginName = "kr.co.pitch_solution.jcollab_bridge.Bridge";

    class AlertViewCallback : AndroidJavaProxy 
    {
        private System.Action<int> alertHandler;

        public AlertViewCallback(System.Action<int> _alertHandler) : base (pluginName + "$AlertViewCallback") 
        {
            alertHandler = _alertHandler;
        }
        public void onButtonTapped(int index) 
        {
            Debug.Log("Button tapped : "+ index);

        }
    }

    static AndroidJavaClass _pluginClass;
    static AndroidJavaObject _pluginInstance;


    public static AndroidJavaClass PluginClass {
        get { 
            if (_pluginClass == null) 
            {
                _pluginClass = new AndroidJavaClass(pluginName);
            }

            return _pluginClass;
        }
    }

    public static AndroidJavaObject PluginInstance {
        get {
            if (_pluginInstance == null) 
            {
                _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance");
                // AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
                // _pluginActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }

            return _pluginInstance;
        }
    }

    /*
    public static AndroidJavaObject PluginActivity {
        get {
            if (_pluginActivity == null) 
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
                _pluginActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }

            return _pluginActivity;
        }
    }
     */


    private void Start() 
    {
        Debug.Log("Elapsed Time : "+ GetElapsedTime());
    }

    float time = 0f;
    private void Update() 
    {
        time += Time.deltaTime;
        if (time >= 5f) 
        {
            time = 0f;
            Debug.Log("Elapsed Time : "+ GetElapsedTime());
        }
    }


    float GetElapsedTime() 
    {
        if (Application.platform == RuntimePlatform.Android) 
            return PluginInstance.Call<float>("getElapsedTime");
        else 
        {
            Debug.LogWarning("Wrong Platform.");
            return 0f;
        }
    }


    public static void ShowAlertDialog(string[] param, System.Action<int> handler = null) 
    {
        if (param.Length < 3) 
        {
            Debug.LogError("AlertView requires at least 3 strings.");
            return;
        }

        if (Application.platform == RuntimePlatform.Android) 
            Debug.Log("... hm...");
            // PluginInstance.Call("showAlertView", PluginActivity, new object[] { param, new AlertViewCallback(handler) });
        else 
            Debug.LogWarning("Wrong Platform.");
    }
}
