
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
    class ChatMessage
    {

        public static void SendAllChatMessage(string message)
        {
            Server.PrintToChatAll($"{ChatColors.Green}{Get5.chatPrefix} {message}{ChatColors.Default}");
        }

        public static void SendPlayerChatMessage(CCSPlayerController player, string message)
        {
            player.PrintToChat($"{ChatColors.Green}{Get5.chatPrefix} {message}{ChatColors.Green}");
        }
        public static void SendConsoleMessage(string message)
        {
            Console.WriteLine($"{Get5.chatPrefix} {message}");
        }
    }

}