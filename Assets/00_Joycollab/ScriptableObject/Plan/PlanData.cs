/// <summary>
/// Plan data 를 위한 scriptable object class 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 31
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 31) : 최초 생성
/// </summary>

using System.Collections.Generic;
using UnityEngine;

namespace Joycollab.v2
{
    [CreateAssetMenu(fileName="Plan Data", menuName="ScriptableObject/Plan Data", order=int.MaxValue)]
    public class PlanData : ScriptableObject
    {
        private const string TAG = "PlanData";

        [SerializeField] private string _planName;
        [SerializeField] private List<string> _listFeature;


    #region public functions

        public string PlanName 
        {
            get { return _planName; }
        }

        public Color PlanColor 
        {
            get {
                Color c = Color.white;

                switch (_planName) 
                {
                    case S.PLAN_BASIC :
                        c = C.BASIC;
                        break;

                    case S.PLAN_STANDARD :
                        c = C.STANDARD;
                        break;

                    case S.PLAN_PREMIUM :
                        c = C.PREMIUM;
                        break;

                    case S.PLAN_FREE :
                    default :
                        c = C.FREE;
                        break;
                }

                return c;
            }
        }

        public int Count 
        {
            get {
                return _listFeature.Count;
            }
        }

        public string GetFeature(int index)
        {
            if (_listFeature.Count <= index) 
            {
                Debug.Log($"{TAG} | out of index. list size : {Count}, request index : {index}");
                return string.Empty;
            }

            return _listFeature[index];
        }

    #endregion  // public functions
    }
}