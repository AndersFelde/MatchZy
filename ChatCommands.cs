using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Core.Attributes;

namespace Get5
{
    class ChatCommands
    {

        static public Dictionary<string, Action<CCSPlayerController, List<string>, LiveMatch>> CommandActions = new()
        {
                { "ban", (player, commandArgs, liveMatch) => liveMatch.MapVote.HandleMapVoteChat(player, commandArgs, ban: true) },
                { "pick", (player, commandArgs, liveMatch) => liveMatch.MapVote.HandleMapVoteChat(player, commandArgs, pick: true) },
            };

    }

}