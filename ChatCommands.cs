using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;

namespace Get5
{
    class ChatCommands
    {

        public Dictionary<string, Action<CCSPlayerController?, CommandInfo?>>? commandActions = new Dictionary<string, Action<CCSPlayerController?, CommandInfo?>> {
                { ".ban", (player, commandInfo) => LiveMatch.HandleMapVoteChat(player, commandInfo) },
                { ".pick", (player, commandInfo) => LiveMatch.HandleMapVoteChat(player, commandInfo) },
            };
    }

}