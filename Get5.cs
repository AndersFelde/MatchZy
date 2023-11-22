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
    [MinimumApiVersion(30)]
    public partial class Get5 : BasePlugin
    {
        public override string ModuleName => "MatchZy";
        public override string ModuleVersion => "0.1";

        public override string ModuleAuthor => "WD- (https://github.com/shobhit-pathak/)";

        public override string ModuleDescription => "A plugin for running and managing CS2 practice/pugs/scrims/matches!";

        public Dictionary<int, CCSPlayerController> playerData = new Dictionary<int, CCSPlayerController>();
        public string chatPrefix = "[Get5] ";

        private LiveMatch? LiveMatch { get; set; }

        public void StartMatch()
        {
            // TODO: definere command og greier
        }
        public override void Load(bool HotReload)
        {
            RegisterEventHandler<EventPlayerConnectFull>((@event, info) =>
            {
                Utils.Log($"[FULL CONNECT] Player ID: {@event.Userid.UserId}, Name: {@event.Userid.PlayerName} has connected!");
                var player = @event.Userid;

                // Handling whitelisted players
                if (!player.IsBot)
                {
                    player.PrintToChat($"{chatPrefix} Welcome to the server!");
                    if (@event.Userid.UserId.HasValue)
                    {

                        playerData[@event.Userid.UserId.Value] = @event.Userid;
                    }
                }
                return HookResult.Continue;
            });

            RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
            {
                Utils.Log($"[EventPlayerDisconnect] Player ID: {@event.Userid.UserId}, Name: {@event.Userid.PlayerName} has disconnected!");
                if (@event.Userid.UserId.HasValue)
                {
                    playerData.Remove(@event.Userid.UserId.Value);
                }

                return HookResult.Continue;
            });
            RegisterEventHandler<EventPlayerChat>((@event, info) =>
                {

                    int index = @event.Userid;
                    // From APIVersion 50 and above, EventPlayerChat userid property will be a "slot", rather than an entity index 
                    // Player index is slot + 1
                    var playerUserId = NativeAPI.GetUseridFromIndex(index);
                    Utils.Log($"[EventPlayerChat] UserId(Index): {index} playerUserId: {playerUserId} Message: {@event.Text}");

                    var originalMessage = @event.Text.Trim();
                    List<string> commandArgs = @event.Text.Trim().ToLower().Split(" ").ToList();

                    CCSPlayerController player = playerData[playerUserId];

                    // Handling player commands
                    if (ChatCommands.CommandActions.ContainsKey(commandArgs[0]))
                    {
                        ChatCommands.CommandActions[commandArgs[0]](player, commandArgs);
                    }

                    return HookResult.Continue;
                });
        }
    }

}