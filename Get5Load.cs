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

        private static bool PlayerIsValid(CCSPlayerController player)
        {
            if (!player.IsBot)
            {
                if (player.UserId.HasValue)
                {

                    return true;
                }
            }
            return false;

        }
        public override void Load(bool hotReload)
        {

            ChatMessage.SendConsoleMessage("-------------------- GET5 plugin live! ----------------------");
            // if (LiveMatch == null)
            // {
            //     return;
            // }

            RegisterEventHandler<EventPlayerConnectFull>((@event, info) =>
            {
                //To fix player not beeing passed in chat event
                if (PlayerIsValid(@event.Userid)) playerData[@event.Userid.UserId.Value] = @event.Userid;
                if (LiveMatch == null) return HookResult.Continue;
                LiveMatch?.PlayerConnectHook(@event.Userid);
                return HookResult.Continue;
            });

            RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
            {
                //To fix player not beeing passed in chat event
                if (PlayerIsValid(@event.Userid)) playerData.Remove(@event.Userid.UserId.Value);
                if (LiveMatch == null) return HookResult.Continue;
                LiveMatch?.PlayerDisconnectHook(@event.Userid);
                return HookResult.Continue;
            });

            RegisterEventHandler<EventGameEnd>((@event, info) =>
            {
                if (LiveMatch == null) return HookResult.Continue;
                LiveMatch?.GameEndHook(@event);
                return HookResult.Continue;
            });

            RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
            {
                if (LiveMatch == null) return HookResult.Continue;
                Utils.Log($"[EventPlayerDisconnect] Player ID: {@event.Userid.UserId}, Name: {@event.Userid.PlayerName} has disconnected!");
                if (@event.Userid.UserId.HasValue)
                {
                    playerData.Remove(@event.Userid.UserId.Value);
                }

                return HookResult.Continue;
            });

            RegisterEventHandler<EventCsWinPanelRound>((@event, info) =>
            {
                if (LiveMatch == null) return HookResult.Continue;
                LiveMatch?.RoundEndHook(@event);
                return HookResult.Continue;
            }, HookMode.Pre);

            RegisterEventHandler<EventPlayerChat>((@event, info) =>
                {
                    if (LiveMatch == null) return HookResult.Continue;
                    string message = @event.Text.Trim().ToLower();
                    if (!message.StartsWith(chatCommandPrefix))
                    {
                        return HookResult.Continue;
                    }

                    int currentVersion = Api.GetVersion();
                    int index = @event.Userid;
                    // From APIVersion 50 and above, EventPlayerChat userid property will be a "slot", rather than an entity index 
                    // Player index is slot + 1
                    if (currentVersion >= 50)
                    {
                        index += 1;
                    }
                    var playerUserId = NativeAPI.GetUseridFromIndex(index);
                    Utils.Log($"[EventPlayerChat] UserId(Index): {index} playerUserId: {playerUserId} Message: {@event.Text}");


                    List<string> commandArgs = @event.Text.Trim().ToLower().Replace(chatCommandPrefix, "").Split(" ").ToList();

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
                if (LiveMatch == null) return HookResult.Continue;
                CCSPlayerController attacker = @event.Attacker;

                if (!attacker.IsValid || attacker.IsBot && !(@event.DmgHealth > 0 || @event.DmgArmor > 0))
                    return HookResult.Continue;
                LiveMatch.PlayerHurtHook(@event);

                return HookResult.Continue;
            });
        }
    }

}