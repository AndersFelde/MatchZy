
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using System.Runtime.CompilerServices;
using System.Net;
using System.Timers;
namespace Get5
{
    public class Warmup
    {
        public LiveMatch LiveMatch;
        private CounterStrikeSharp.API.Modules.Timers.Timer? ReadyNotificationTimer;
        public Warmup(LiveMatch liveMatch)
        {
            LiveMatch = liveMatch;
        }

        public void Debug()
        {
            Console.WriteLine("Warmup DEBUG");
            Console.WriteLine($"LiveMatch {LiveMatch}");
            Console.WriteLine($"ReadyNotificationTimer {ReadyNotificationTimer}");
        }

        public void Start()
        {
            LiveMatch.Match.CT.UnReadyPlayers();
            LiveMatch.Match.Terrorists.UnReadyPlayers();
            ReadyNotificationTimer = Utils.CreateContinousChatUpdate(SendUnreadyPlayersMessage, LiveMatch.Get5);
            ChatMessage.SendAllChatMessage("Welcome to the server, we are just warming up");
            ChatMessage.SendAllChatMessage("Send '.ready' to ready");
            Server.ExecuteCommand("exec warmup");
        }

        public void HandleReadyChat(CCSPlayerController player)
        {
            // TODO: Print all ready players
            LiveMatch.Match.GetPlayer(player)?.Ready();
            if ((LiveMatch.Match.CT.ReadyPlayers() > LiveMatch.Match.MinPlayersToReady) && (LiveMatch.Match.CT.ReadyPlayers() > LiveMatch.Match.MinPlayersToReady))
            {
                LiveMatch.EndWarmup();
            }
        }


        public void HandleUnReadyChat(CCSPlayerController player)
        {
            // TODO: Print all unready players
            LiveMatch.Match.GetPlayer(player)?.UnReady();
        }

        public void End()
        {
            ReadyNotificationTimer?.Kill();
            ReadyNotificationTimer = null;
        }

        public void SendUnreadyPlayersMessage()
        {

            foreach (var player in LiveMatch.Match.CT.Players)
            {
                string readyMsg = "NOT READY";
                if (player.IsReady)
                {
                    readyMsg = "READY";

                }
                ChatMessage.SendAllChatMessage($"{player.Name} [{readyMsg}]");

            }

            foreach (var player in LiveMatch.Match.Terrorists.Players)
            {
                string readyMsg = "NOT READY";
                if (player.IsReady)
                {
                    readyMsg = "READY";

                }
                ChatMessage.SendAllChatMessage($"{player.Name} [{readyMsg}]");

            }

        }


    }

}