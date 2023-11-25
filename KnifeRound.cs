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
using System.ComponentModel;

namespace Get5
{
    public class KnifeRound
    {
        public bool KnifeActive = false;
        public bool T_won = false;
        public bool CT_won = false;

        private string winning_team = "";

        private LiveMatch LiveMatch { get; set; }

        public KnifeRound(LiveMatch liveMatch)
        {
            this.LiveMatch = liveMatch;
        }
        private CounterStrikeSharp.API.Modules.Timers.Timer? StaySwitchTimer;

        public void Start()
        {
            ChatMessage.SendAllChatMessage("Knife round is starting!");
            KnifeActive = true;
            Server.ExecuteCommand("exec comp");
            Server.ExecuteCommand("exec knife");
        }

        public void End()
        {
            KnifeActive = false;
            StaySwitchTimer?.Kill();
            StaySwitchTimer = null;
        }

        public void HandleKnifeRoundEnd(EventCsWinPanelRound @event)
        {
            (int tAlive, int tHealth) = LiveMatch.GetAlivePlayers(terrorists: true);
            (int ctAlive, int ctHealth) = LiveMatch.GetAlivePlayers(CT: true);
            int knifeWinner;
            if (ctAlive > tAlive)
            {
                CT_won = true;
            }
            else if (tAlive > ctAlive)
            {
                T_won = true;
            }
            else if (ctHealth > tHealth)
            {
                CT_won = true;
            }
            else if (tHealth > ctHealth)
            {
                T_won = true;
            }
            else
            {
                // Choosing a winner randomly
                Random random = new();
                knifeWinner = random.Next(2, 4);
            }

            // Below code is working partially (Winner audio plays correctly for knife winner team, but may display round winner incorrectly)
            // Hence we restart the game with StartAfterKnifeWarmup and allow the winning team to choose side

            @event.FunfactToken = "";

            // Commenting these assignments as they were crashing the server.
            // long empty = 0;
            // @event.FunfactPlayer = null;
            // @event.FunfactData1 = empty;
            // @event.FunfactData2 = empty;
            // @event.FunfactData3 = empty;
            int finalEvent = 10;
            if (CT_won)
            {
                finalEvent = 8;
            }
            else if (T_won)
            {
                finalEvent = 9;
            }
            @event.FinalEvent = finalEvent;
            if (T_won)
            {
                winning_team = "Terrorists";
            }
            else
            {
                winning_team = "CT";
            }
            StaySwitchTimer = Utils.CreateContinousChatUpdate(SendSwitchStayMessage, LiveMatch.Get5, 30);
            SendSwitchStayMessage();
        }

        public void SendSwitchStayMessage()
        {
            ChatMessage.SendAllChatMessage($"{winning_team} choose to stay or switch");
            ChatMessage.SendAllChatMessage($"Send .switch or .stay");
        }

        public void HandleKnifeRoundChat(CCSPlayerController player, bool stay)
        {
            if (KnifeActive)
            {
                if ((Utils.PlayerIsTerrorist(player) && T_won) || (Utils.PlayerIsCT(player) && CT_won))
                {
                    string winner;
                    if (T_won)
                    {
                        winner = "Terrorsits";
                    }
                    else
                    {

                        winner = "CT";
                    }
                    if (stay)
                    {
                        ChatMessage.SendAllChatMessage($"{winner} chose to stay");
                    }
                    else
                    {
                        ChatMessage.SendAllChatMessage($"{winner} chose to switch");
                        LiveMatch.SwapTeams();
                    }
                    KnifeActive = false;
                    LiveMatch.EndKnifeRound();
                }
                else
                {
                    ChatMessage.SendPlayerChatMessage(player, "You can't vote");

                }


            }
        }

        public void Debug()
        {
            ChatMessage.SendConsoleMessage("KnifeRound DEBUG");
            ChatMessage.SendConsoleMessage($"KnifeActive {KnifeActive}");
            ChatMessage.SendConsoleMessage($"T_won {T_won}");
            ChatMessage.SendConsoleMessage($"CT_won {CT_won}");
        }
    }
}