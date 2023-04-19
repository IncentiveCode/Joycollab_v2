using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PitchSolution
{
    public class Resizable : MonoBehaviour
    {
    #region resize info
        private enum eResizeDirection 
        {
            None = 0,
            Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left, TopLeft
        } 

        private readonly Vector2 v2Top = new Vector2(0.5f, 1f);
        private readonly Vector2 v2TopRight = new Vector2(1f, 1f);
        private readonly Vector2 v2Right = new Vector2(1f, 0.5f);
        private readonly Vector2 v2BottomRight = new Vector2(1f, 0f);
        private readonly Vector2 v2Bottom = new Vector2(0.5f, 0f);
        private readonly Vector2 v2BottomLeft = new Vector2(0f, 0f);
        private readonly Vector2 v2Left = new Vector2(0f, 0.5f);
        private readonly Vector2 v2TopLeft = new Vector2(0f, 1f);
    #endregion  // resize info


    // local variables


    #region Unity functions
        private void Awake() 
        {

        } 
    #endregion  // Unity functions


    #region Interface implementations

    #endregion  // Interface implementations
    }
}