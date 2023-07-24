/// <summary>
/// tiny theme data 를 사용하기 위한 scriptable object class 
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
    [CreateAssetMenu(fileName="Tiny Theme Data", menuName="ScriptableObject/Tiny Theme Data", order=int.MaxValue)]
    public class TinyThemeData : ScriptableObject
    {
        [SerializeField] private string _themeName;
        [SerializeField] private List<Sprite> _themeList;


    #region public functions

        public string ThemeName 
        {
            get { return _themeName; }
        }

        public Sprite GetTinyThemeImage(string name)
        {
            Sprite spr = null;

            foreach (Sprite s in _themeList) 
            {
                if (s.name.Equals(name)) 
                {
                    spr = s;
                    break;
                }
            }

            return spr;
        }

    #endregion  // public functions
    }
}