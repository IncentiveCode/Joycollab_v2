using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Joycollab.v2
{
    public class TurnManager : NetworkBehaviour
    {
        private List<WorldPlayer> players = new List<WorldPlayer>();

        public void AddPlayer(WorldPlayer _player) 
        {
            players.Add(_player);
        } 
        public int GetPlayerCount => players.Count;
    }
}