using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidThemeControl : MonoBehaviour
{
    public void Awake()
    {
        StatusBarControl(true);
        NavigationBarControl(true);
    }

    public void StatusBarControl(bool _isVisible)
    {
        ApplicationChrome.statusBarState = _isVisible ? ApplicationChrome.States.Visible : ApplicationChrome.States.Hidden;
    }

    public void StatusBarColorControl(uint _colorValue)
    {
        ApplicationChrome.statusBarColor = _colorValue;
    }

    public void NavigationBarControl(bool _isVisible)
    {
        ApplicationChrome.navigationBarState = _isVisible ? ApplicationChrome.States.Visible : ApplicationChrome.States.Hidden;
    }

    public void NavigationBarColorControl(uint _colorValue)
    {
        ApplicationChrome.navigationBarColor = _colorValue;
    }
}
