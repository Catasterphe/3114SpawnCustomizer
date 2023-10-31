using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginAPI.Core.Attributes;
using PluginAPI.Core;
using PluginAPI.Events;
using PluginAPI.Roles;

namespace Always3114
{
    public class Plugin
    {
        public static Plugin Singleton { get; private set; }
        public const string Version = "0.0.1";
        static Random randomGen = new Random();
        [PluginConfig] public Config PluginConfig;

        [PluginEntryPoint("Always Spawn 3114", Version, "Spawns a player as the skeleton if there is not one already selected.", "Aster")]
        public void LoadPlugin()
        {
            Singleton = this;
            EventManager.RegisterEvents(this);
            var handler = PluginHandler.Get(this);
            handler.LoadConfig(this, nameof(PluginConfig));

            Log.Info($"Plugin {handler.PluginName} loaded.");
        }

        [PluginEvent]
        bool RoundStart(RoundStartEvent ev)
        {
            var EligiblePlayers = Player.GetPlayers().ToList();
            var playersToRemove = new List<Player>();

            if (EligiblePlayers.Count >= PluginConfig.MinimumNumberOfPlayers)
            {
                foreach (var player in EligiblePlayers)
                {
                    if (player.Role == PlayerRoles.RoleTypeId.Scp3114)
                    {
                        EligiblePlayers.Clear();
                        Log.Debug("3114 spawn; clear players");
                        return true;
                    }
                    if (!PluginConfig.AllowScps && player.IsSCP)
                    {
                        playersToRemove.Add(player);
                    } else if (PluginConfig.OnlyScps && player.IsHuman)
                    {
                        playersToRemove.Add(player);
                    }
                }

                // Remove players after figuring out who is not allowed based on the config.
                foreach (var player in playersToRemove)
                {
                    EligiblePlayers.Remove(player);
                }

                // get a random list element (player), 3 times by default
                for (int attempt = 0; attempt < PluginConfig.MaxAttempts; attempt++)
                {
                    // edge case if theres only one player on the server and they're not eligible due to being an scp
                    if (EligiblePlayers.Count > 0)
                    {
                        int randomNumber = randomGen.Next(EligiblePlayers.Count);
                        Player randomPlayer = EligiblePlayers[randomNumber];

                        // Check that the player hasn't left
                        if (randomPlayer != null)
                        {
                            Log.Info($"{randomPlayer.LogName} has been chosen to become SCP-3114");
                            randomPlayer.Role = PlayerRoles.RoleTypeId.Scp3114;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
