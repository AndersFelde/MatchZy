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
        public static void Debug()
        {
            ChatMessage.SendConsoleMessage("ChatCommands DEBUG");
            ChatMessage.SendConsoleMessage($"CommandActions {string.Join(", ", CommandActions.Keys)}");
        }

        static public Dictionary<string, Action<CCSPlayerController, List<string>, LiveMatch>> CommandActions = new()
        {
                { "ban", (player, commandArgs, liveMatch) => liveMatch.MapVote.HandleMapVoteChat(player, commandArgs, ban: true) },
                { "pick", (player, commandArgs, liveMatch) => liveMatch.MapVote.HandleMapVoteChat(player, commandArgs, ban: false) },
                { "stay", (player, commandArgs, liveMatch) => liveMatch.KnifeRound.HandleKnifeRoundChat(player, stay: true) },
                { "switch", (player, commandArgs, liveMatch) => liveMatch.KnifeRound.HandleKnifeRoundChat(player, stay: false) },
                { "ready", (player, commandArgs, liveMatch) => liveMatch.Warmup.HandleReadyChat(player) },
                { "unready", (player, commandArgs, liveMatch) => liveMatch.Warmup.HandleUnReadyChat(player) },
                { "pause", (player, commandArgs, liveMatch) => liveMatch.HandlePauseCommand(player) },
                { "unpause", (player, commandArgs, liveMatch) => liveMatch.HandleUnPauseCommand(player) },
                { "maps", (player, commandArgs, liveMatch) => liveMatch.MapVote.HandleMapsCommand(player) },
                { "help", (player, commandArgs, liveMatch) => PrintHelp() },
            };

        public static void PrintHelp()
        {
            ChatMessage.SendAllChatMessage("Available commands:");
            foreach (var command in CommandActions)
            {
                ChatMessage.SendAllChatMessage($"{Get5.chatCommandPrefix}{command.Key}");
            }

        }

    }

}