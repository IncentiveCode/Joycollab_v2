using System;
using UnityEngine;

public class AndroidClickListener : AndroidJavaProxy
{
    public readonly Action<int> Callback;
    public AndroidClickListener(Action<int> callback) : base("android.content.DialogInterface$OnClickListener") 
    { 
        this.Callback = callback;
    }
    public void onClick(AndroidJavaObject dialog, int id)
    {
        Callback.Invoke(id);
        dialog.Call("dismiss");
    }
}
