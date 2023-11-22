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
    public partial class Get5
    {

        public override void Load(bool HotReload)
        {
            if (LiveMatch == null)
            {
                return;
            }

            RegisterEventHandler<EventPlayerConnectFull>((@event, info) =>
            {
                Utils.Log($"[FULL CONNECT] Player ID: {@event.Userid.UserId}, Name: {@event.Userid.PlayerName} has connected!");
                var player = @event.Userid;

                // Handling whitelisted players
                if (!player.IsBot)
                {
                    ChatMessage.SendPlayerChatMessage(player, "Welcome to the server!");
                    if (@event.Userid.UserId.HasValue)
                    {

                        playerData[@event.Userid.UserId.Value] = @event.Userid;
                    }
                }
                LiveMatch?.PlayerConnectHook(@event);
                return HookResult.Continue;
            });

            RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
            {
                Utils.Log($"[FULL CONNECT] Player ID: {@event.Userid.UserId}, Name: {@event.Userid.PlayerName} has connected!");
                var player = @event.Userid;

                // Handling whitelisted players
                if (!player.IsBot)
                {
                    ChatMessage.SendPlayerChatMessage(player, "Welcome to the server!");
                    if (@event.Userid.UserId.HasValue)
                    {

                        playerData[@event.Userid.UserId.Value] = @event.Userid;
                    }
                }
                LiveMatch?.PlayerDisconnectHook(@event);
                return HookResult.Continue;
            });

            RegisterEventHandler<EventGameEnd>((@event, info) =>
            {
                LiveMatch?.GameEndHook(@event);
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

            RegisterEventHandler<EventCsWinPanelRound>((@event, info) =>
            {
                LiveMatch?.RoundEndHook(@event);
                return HookResult.Continue;
            }, HookMode.Pre);

            RegisterEventHandler<EventPlayerChat>((@event, info) =>
                {
                    string message = @event.Text.Trim().ToLower();
                    if (!message.StartsWith(this.chatCommandPrefix))
                    {
                        return HookResult.Continue;
                    }

                    int index = @event.Userid;
                    // From APIVersion 50 and above, EventPlayerChat userid property will be a "slot", rather than an entity index 
                    // Player index is slot + 1
                    var playerUserId = NativeAPI.GetUseridFromIndex(index);
                    Utils.Log($"[EventPlayerChat] UserId(Index): {index} playerUserId: {playerUserId} Message: {@event.Text}");


                    List<string> commandArgs = @event.Text.Trim().ToLower().Replace(this.chatCommandPrefix, "").Split(" ").ToList();

                    CCSPlayerController player = playerData[playerUserId];

                    // Handling player commands
                    if (ChatCommands.CommandActions.ContainsKey(commandArgs[0]) && this.LiveMatch != null)
                    {
                        ChatCommands.CommandActions[commandArgs[0]](player, commandArgs, this.LiveMatch);
                    }

                    return HookResult.Continue;
                });

            RegisterEventHandler<EventPlayerHurt>((@event, info) =>
            {
                CCSPlayerController attacker = @event.Attacker;

                if (!attacker.IsValid || attacker.IsBot && !(@event.DmgHealth > 0 || @event.DmgArmor > 0))
                    return HookResult.Continue;
                LiveMatch.PlayerHurtHook(@event);

                return HookResult.Continue;
            });
        }
    }

}