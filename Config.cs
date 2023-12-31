﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Always3114
{
    public class Config
    {
        // The Minimum number of players that have to be in the game for 3114 to spawn every round.
        public int MinimumNumberOfPlayers { get; set; } = 6;
        // Allow SCP's to becaaaome 3114?
        public bool AllowScps { get; set; } = true;
        // ONLY allow scsp to become 3114
        public bool OnlyScps { get; set; } = false;

        // declare a chance to spawn, 0 = 0%, 1 = 100%
        public float ChanceToSpawn { get; set; } = 1f;

    }
}
