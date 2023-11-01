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
        public const string Version = "0.0.1";
        [PluginAPI.Core.Attributes.PluginConfig] public PluginConfig PluginConfig;

        [PluginEntryPoint("Always Spawn 3114", Version, "Spawns a player as the skeleton if there is not one already selected.", "Aster")]
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
        if (!NetworkServer.active)
        {
            return false;
        }

        Scp3114Spawner._ragdollsSpawned = false;
        Scp3114Spawner.SpawnCandidates.Clear();
        if (!Plugin.Singleton.PluginConfig.AllowScps)
        {
            PlayerRolesUtils.ForEachRole<HumanRole>(Scp3114Spawner.SpawnCandidates.Add);
        }
        else if (Plugin.Singleton.PluginConfig.AllowScps && !Plugin.Singleton.PluginConfig.OnlyScps)
        {
            PlayerRolesUtils.ForEachRole<HumanRole>(Scp3114Spawner.SpawnCandidates.Add);
            PlayerRolesUtils.ForEachRole<Scp049Role>(Scp3114Spawner.SpawnCandidates.Add);
            PlayerRolesUtils.ForEachRole<Scp173Role>(Scp3114Spawner.SpawnCandidates.Add);
            PlayerRolesUtils.ForEachRole<Scp079Role>(Scp3114Spawner.SpawnCandidates.Add);
            PlayerRolesUtils.ForEachRole<Scp096Role>(Scp3114Spawner.SpawnCandidates.Add);
            PlayerRolesUtils.ForEachRole<Scp106Role>(Scp3114Spawner.SpawnCandidates.Add);
            PlayerRolesUtils.ForEachRole<Scp939Role>(Scp3114Spawner.SpawnCandidates.Add);
        }
        else if (Plugin.Singleton.PluginConfig.OnlyScps && Plugin.Singleton.PluginConfig.AllowScps)
        {
            PlayerRolesUtils.ForEachRole<Scp049Role>(Scp3114Spawner.SpawnCandidates.Add);
            PlayerRolesUtils.ForEachRole<Scp173Role>(Scp3114Spawner.SpawnCandidates.Add);
            PlayerRolesUtils.ForEachRole<Scp079Role>(Scp3114Spawner.SpawnCandidates.Add);
            PlayerRolesUtils.ForEachRole<Scp096Role>(Scp3114Spawner.SpawnCandidates.Add);
            PlayerRolesUtils.ForEachRole<Scp106Role>(Scp3114Spawner.SpawnCandidates.Add);
            PlayerRolesUtils.ForEachRole<Scp939Role>(Scp3114Spawner.SpawnCandidates.Add);
        } else
        {
            // couldn't figure out spawn candidates from config file, just go with default humans.
            PlayerRolesUtils.ForEachRole<HumanRole>(Scp3114Spawner.SpawnCandidates.Add);
        }

        if (Scp3114Spawner.SpawnCandidates.Count >= Plugin.Singleton.PluginConfig.MinimumNumberOfPlayers)
        {
            Scp3114Spawner.SpawnCandidates.RandomItem().roleManager.ServerSetRole(RoleTypeId.Scp3114, RoleChangeReason.RoundStart);
        }

        return false;
    }
}