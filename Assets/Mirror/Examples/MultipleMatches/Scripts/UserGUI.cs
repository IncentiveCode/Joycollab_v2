using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.MultipleMatch
{
    // Custom class
    public class UserGUI : MonoBehaviour
    {
        [Header("GUI Elements")]
        public Text playerName;

        [Header("Diagnostics - Do Not Modify")]
        public CanvasController canvasController;

        public void Awake()
        {
            canvasController = FindObjectOfType<CanvasController>();
        }

        [ClientCallback]
        public void SetPlayerInfo(PlayerInfo info)
        {
            playerName.text = $"Player {info.playerIndex}";
        }
    }
}
