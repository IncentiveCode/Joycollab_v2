/// <summary>
/// tiny theme data 를 관리하기 위한 theme container class
/// @author         : HJ Lee
/// @last update    : 2023. 07. 24
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 24) : 이전 작업물 이관.
/// </summary>

using System.Collections.Generic;
using UnityEngine;

namespace Joycollab.v2
{
    public class TinyThemeContainer : MonoBehaviour
    {
        private const string TAG = "TinyThemeContainer";
        public static TinyThemeContainer singleton;
        
        [SerializeField] private List<TinyThemeData> _listThemeData;
    

    #region Unity functions

        private void Awake()
        {
            singleton = this; 
        }

        private void OnDestroy() 
        {
            if (singleton != null)
            {
                singleton = null;
            }
        }

    #endregion  // Unity functions


    #region public function

        public Sprite Get(string themeName, string fileName) 
        {
            Sprite s = null;
            foreach (TinyThemeData d in _listThemeData) 
            {
                if (d.ThemeName.Equals(themeName)) 
                {
                    s = d.GetTinyThemeImage(fileName);
                    break;
                }
            }

            if (s == null) Debug.Log($"{TAG} | Get(), sprite 획득 실패. theme : {themeName}, file : {fileName}");
            return s;
        }

    #endregion  // public function
    }
}