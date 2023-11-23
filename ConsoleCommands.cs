using System;
using System.IO;
using System.Linq;
using System.Text.Json;
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
using CounterStrikeSharp.API.Modules.Admin;
using System.Runtime.CompilerServices;
namespace Get5
{
    public partial class Get5
    {
        [RequiresPermissions("@css/generic")]
        [ConsoleCommand("get5", "This is an example command description")]
        public void get5Command(CCSPlayerController? player, CommandInfo command)
        {
            ChatMessage.SendConsoleMessage("Get5 version 1.0.0");
        }

        [RequiresPermissions("@css/generic")]
        [ConsoleCommand("get5_start", "Start a get5 match")]
        [CommandHelper(minArgs: 1, usage: "{match_name}", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        public void get5_startCommand(CCSPlayerController? player, CommandInfo command)
        {
            string match_name = command.ArgByIndex(1);
            LiveMatch = new LiveMatch(match: Match.LoadFromJson(match_name), get5: this);
        }

        [RequiresPermissions("@css/generic")]
        [ConsoleCommand("get5_stop", "Stop a get5 match")]
        [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        public void get5_stopCommand(CCSPlayerController? player, CommandInfo command)
        {

            LiveMatch = null;
        }

        [RequiresPermissions("@css/generic")]
        [ConsoleCommand("get5_help", "Get help for get5 commands")]
        [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        public void get5_help(CCSPlayerController? player, CommandInfo command)
        {
            ChatMessage.SendConsoleMessage("Available commands:");
            ChatMessage.SendConsoleMessage("get5");
            ChatMessage.SendConsoleMessage("get5_start {match_name}");
            ChatMessage.SendConsoleMessage("get5_stop");
            ChatMessage.SendConsoleMessage("get5_help");


        }


    }

}
