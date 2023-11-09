/// <summary>
/// Plan data 를 위한 scriptable object class 
/// @author         : HJ Lee
/// @last update    : 2023. 11. 09
/// @version        : 0.2
/// @update
///     v0.1 (2023. 07. 31) : 최초 생성
///     v0.2 (2023. 11. 09) : 각 언어별 가격 정보 추가.
/// </summary>

using System.Collections.Generic;
using UnityEngine;

namespace Joycollab.v2
{
    [CreateAssetMenu(fileName="Plan Data", menuName="ScriptableObject/Plan Data", order=int.MaxValue)]
    public class PlanData : ScriptableObject
    {
        private const string TAG = "PlanData";

        [SerializeField] private List<string> _listMonthlyFee;
        [SerializeField] private List<string> _listYearlyFee;
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

        public string GetFee(bool isYearly) 
        {
            int index = R.singleton.Region switch {
                S.REGION_KOREAN => 0,
                S.REGION_JAPANESE => 2,
                _ => 1
            };

            return isYearly ? _listYearlyFee[index] : _listMonthlyFee[index];
        }

    #endregion  // public functions
    }
}