using Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameManagement
{
    public static class PlayerManager
    {
        private static List<PlayerController> players = new List<PlayerController>();
        public static void RegisterPlayer(PlayerController player) => players.Add(player);
        public static void UnregisterPlayer(PlayerController player) => players.Remove(player);

        public static int PlayerCount => players.Count;

        public static PlayerController GetPlayer(int index) => players[index];
    }
}
