/// <summary>
/// Floor data 를 위한 scriptable object class 
/// @author         : HJ Lee
/// @last update    : 2023. 10. 04
/// @version        : 0.1
/// @update
///     v0.1 (2023. 10. 04) : 최초 생성
/// </summary>

using UnityEngine;

namespace Joycollab.v2
{
    [CreateAssetMenu(fileName="Floor Data", menuName="ScriptableObject/Floor Data", order=int.MaxValue)]
    public class FloorData : ScriptableObject
    {
        private const string TAG = "FloorData";

        [SerializeField] private int _floorNo;
        [SerializeField] private string _floorName;


    #region public functions

        public int FloorNo 
        {
            get { return _floorNo; }
        }

        public string FloorString
        {
            get { return $"{_floorNo}F"; }
        }

        public string FloorName 
        {
            get { return _floorName; }
        }

    #endregion  // public functions
    }
}