/// <summary>
/// Room pointer data 를 위한 scriptable object class 
/// @author         : HJ Lee
/// @last update    : 2023. 10. 25
/// @version        : 0.1
/// @update
///     v0.1 (2023. 10. 25) : 최초 생성
/// </summary>

using System.Collections.Generic;
using UnityEngine;

namespace Joycollab.v2
{
    [CreateAssetMenu(fileName="Room Pointer Data", menuName="ScriptableObject/Room Pointer Data", order=int.MaxValue)]
    public class RoomPointerData : ScriptableObject
    {
        private const string TAG = "RoomPointerData";

        [Header("Move Parameter")] 
        [SerializeField] private float _moveLimit = 5f;
        [SerializeField] private float _speed = 2f;
        
        [Header("target Parameter")] 
        [SerializeField] private int _roomNo;
        [SerializeField] private Vector3 _target;


    #region public functions

        public float MoveLimit => _moveLimit;
        public float Speed => _speed;
        public int RoomNo => _roomNo;
        public Vector3 Target => _target;

    #endregion  // public functions
    }
}