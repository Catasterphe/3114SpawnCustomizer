using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginAPI.Core.Attributes;
using PluginAPI.Core;
using PluginAPI.Events;
using PluginAPI.Roles;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles;
using Unity;
using Mirror;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.PlayableScps.Scp173;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp096;
using PlayerRoles.PlayableScps.Scp939;
using PlayerRoles.PlayableScps.Scp106;
using Always3114;

namespace Always3114
{
    public class Plugin
    {
        public static Plugin Singleton { get; private set; }
        public const string Version = "13.3.1";
        [PluginConfig] public Config Config;

        [PluginEntryPoint("3114 Spawn Customizer", Version, "Spawns a player as the skeleton if there is not one already selected.", "Aster")]
        public void LoadPlugin()
        {
            Singleton = this;
            EventManager.RegisterEvents(this);
            var handler = PluginHandler.Get(this);
            handler.LoadConfig(this, nameof(PluginConfig));
            Harmony harmony = new Harmony("aster.scpsl.spawn3114");
            harmony.PatchAll();

            Log.Info($"Plugin {handler.PluginName} loaded.");
        }
    }
}

[HarmonyPatch(typeof(Scp3114Spawner), nameof(Scp3114Spawner.OnPlayersSpawned))]
public class Patch
{
    static bool Prefix()
    {
        Random rand = new Random();
        Config config = Plugin.Singleton.Config;
        if (rand.NextDouble() <= config.ChanceToSpawn) {
            if (!NetworkServer.active)
            {
                return false;
            }

            Scp3114Spawner._ragdollsSpawned = false;
            Scp3114Spawner.SpawnCandidates.Clear();
            if (!config.AllowScps)
            {
                PlayerRolesUtils.ForEachRole<HumanRole>(Scp3114Spawner.SpawnCandidates.Add);
            }
            else if (config.AllowScps)
            {
                if (!config.OnlyScps)
                {
                    PlayerRolesUtils.ForEachRole<HumanRole>(Scp3114Spawner.SpawnCandidates.Add);
                }
                PlayerRolesUtils.ForEachRole<Scp049Role>(Scp3114Spawner.SpawnCandidates.Add);
                PlayerRolesUtils.ForEachRole<Scp173Role>(Scp3114Spawner.SpawnCandidates.Add);
                PlayerRolesUtils.ForEachRole<Scp079Role>(Scp3114Spawner.SpawnCandidates.Add);
                PlayerRolesUtils.ForEachRole<Scp096Role>(Scp3114Spawner.SpawnCandidates.Add);
                PlayerRolesUtils.ForEachRole<Scp106Role>(Scp3114Spawner.SpawnCandidates.Add);
                PlayerRolesUtils.ForEachRole<Scp939Role>(Scp3114Spawner.SpawnCandidates.Add);
            }
            else
            {
                // fallback to the normal code that makes it so only humans can become 3114
                PlayerRolesUtils.ForEachRole<HumanRole>(Scp3114Spawner.SpawnCandidates.Add);
            }

            if (Scp3114Spawner.SpawnCandidates.Count >= config.MinimumNumberOfPlayers)
            {
                Scp3114Spawner.SpawnCandidates.RandomItem().roleManager.ServerSetRole(RoleTypeId.Scp3114, RoleChangeReason.RoundStart);
            }
            return false;
        }
        return false;
    }
}