using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Joycollab.v2
{
    public class RoomPlayer : MonoBehaviour
    {
        private const string TAG = "RoomPlayer";     

        [SerializeField] private TMP_Text _txtName;
        private WorldPlayer player;

        public void SetPlayer(WorldPlayer player) 
        {
            this.player = player;
            _txtName.text = player.avatarName;
        }
    }
}