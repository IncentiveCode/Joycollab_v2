/// <summary>
/// Avatar state data 를 사용하기 위한 scriptable object class 
/// @author         : HJ Lee
/// @last update    : 2023. 11. 03
/// @version        : 0.1
/// @update
///     v0.1 (2023. 11. 03) : 최초 생성
/// </summary>

using UnityEngine;

namespace Joycollab.v2
{
    [CreateAssetMenu(fileName="state Data", menuName="ScriptableObject/State Data", order=int.MaxValue)]
    public class StateData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _icon;


    #region public functions

        public string StateId => _id; 
        public Sprite StateIcon => _icon;

    #endregion  // public functions
    }
}