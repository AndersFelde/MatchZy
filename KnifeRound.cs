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

        private LiveMatch LiveMatch { get; set; }

        public KnifeRound(LiveMatch liveMatch)
        {
            this.LiveMatch = liveMatch;
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
            KnifeActive = false;
            LiveMatch.EndKnifeRound();

        }
        public void HandleKnifeRoundChat(CCSPlayerController player, List<string> commandArgs, bool stay)
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
                        ChatMessage.SendAllChatMessage($"{winner} chose to swap");
                        LiveMatch.SwapTeams();
                    }
                    KnifeActive = false;
                }
                else
                {
                    ChatMessage.SendPlayerChatMessage(player, "You can't vote");

                }


            }
        }
    }
}