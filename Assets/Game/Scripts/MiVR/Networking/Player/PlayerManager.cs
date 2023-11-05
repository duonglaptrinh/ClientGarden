using System;
using System.Collections.Generic;

namespace Game
{
    public static class PlayerManager
    {
        private static readonly HashSet<INetworkPlayer> players = new HashSet<INetworkPlayer>();

        public static IEnumerable<INetworkPlayer> Players => players;

        public  static event Action<INetworkPlayer> OnNewPlayerAdded;
        public  static event Action<INetworkPlayer> OnRemovePlayer;

        public static void Register(INetworkPlayer player)
        {
            if (players.Add(player))
            {
                OnNewPlayerAdded?.Invoke(player);
            }
        }

        public static void Unregister(INetworkPlayer player)
        {
            if (players.Remove(player))
            {
                OnRemovePlayer?.Invoke(player);
            }
        }
    }
}