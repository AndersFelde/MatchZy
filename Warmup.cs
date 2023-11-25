
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

        public bool IsWarmup = false;
        private CounterStrikeSharp.API.Modules.Timers.Timer? ReadyNotificationTimer;
        public Warmup(LiveMatch liveMatch)
        {
            LiveMatch = liveMatch;
        }

        public void Debug()
        {
            ChatMessage.SendConsoleMessage("Warmup DEBUG");
            ChatMessage.SendConsoleMessage($"LiveMatch {LiveMatch}");
            ChatMessage.SendConsoleMessage($"ReadyNotificationTimer {ReadyNotificationTimer}");
            ChatMessage.SendConsoleMessage($"CT ready players {LiveMatch.Match.CT.ReadyPlayers()}");
            ChatMessage.SendConsoleMessage($"T ready players {LiveMatch.Match.Terrorists.ReadyPlayers()}");
        }

        public void Start()
        {
            LiveMatch.Match.CT.UnReadyPlayers();
            LiveMatch.Match.Terrorists.UnReadyPlayers();
            ReadyNotificationTimer = Utils.CreateContinousChatUpdate(SendPlayersStatusMessage, LiveMatch.Get5, seconds: 30);
            IsWarmup = true;
            SendPlayersStatusMessage();
            ChatMessage.SendAllChatMessage("Welcome to the server, we are just warming up");
            ChatMessage.SendAllChatMessage("Send '.ready' to ready");
            Server.ExecuteCommand("exec prac");
        }

        public void HandleReadyChat(CCSPlayerController player)
        {
            if (!IsWarmup) return;
            LiveMatch.Match.GetPlayer(player)?.Ready();
            if ((LiveMatch.Match.CT.ReadyPlayers() >= LiveMatch.Match.MinPlayersToReady) && (LiveMatch.Match.Terrorists.ReadyPlayers() >= LiveMatch.Match.MinPlayersToReady))
            {
                LiveMatch.EndWarmup();
            }
            else
            {
                SendPlayersStatusMessage();
            }
        }


        public void HandleUnReadyChat(CCSPlayerController player)
        {
            if (!IsWarmup) return;
            LiveMatch.Match.GetPlayer(player)?.UnReady();
            SendPlayersStatusMessage();
        }

        public void End()
        {
            IsWarmup = false;
            ReadyNotificationTimer?.Kill();
            ReadyNotificationTimer = null;
        }

        public void SendPlayersStatusMessage()
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