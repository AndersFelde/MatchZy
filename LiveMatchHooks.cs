
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

namespace Get5
{
    public partial class LiveMatch
    {

        private CounterStrikeSharp.API.Modules.Timers.Timer? NextMatchTimer;

        public void ScoreUpdateHook(EventTeamScore @event)
        {
            if (IsLive)
            {
                Match.GetTeam(@event.Teamid)?.UpdateScore(@event.Score);
            }
        }

        public void RoundEndHook(EventRoundEnd @event)
        {
            if (IsLive)
            {
                Match.GetTeam(@event.Winner).Score++;
                if (Utils.IsTeamSwapRequired())
                {
                    ChatMessage.SendAllChatMessage("Halftime");
                    ChatMessage.SendAllChatMessage("Halftime");
                    ChatMessage.SendAllChatMessage("Halftime");
                    ChatMessage.SendAllChatMessage("Halftime");
                    (Match.Terrorists, Match.CT) = (Match.CT, Match.Terrorists);
                }
            }
        }



        public void RoundEndHook(EventCsWinPanelRound @event)
        {
            if (IsLive)
            {
                DamageInfo.ShowDamageInfo();
            }
            else if (IsKnifeRound)
            {
                KnifeRound.HandleKnifeRoundEnd(@event);
            }


        }

        public void RoundStartHook(EventRoundStart @event)
        {
            if (IsLive)
            {

                DamageInfo.InitPlayerDamageInfo();
            }
        }

        public void PlayerHurtHook(EventPlayerHurt @event)
        {
            if (IsLive)
            {
                CCSPlayerController attacker = @event.Attacker;
                if (@event.Userid.TeamNum != attacker.TeamNum)
                {
                    int targetId = (int)@event.Userid.UserId!;

                    DamageInfo.UpdatePlayerDamageInfo(@event, targetId);
                }
            }
        }


        public void PlayerConnectHook(CCSPlayerController player)
        {
            if (Match.CT.HasPlayer(player))
            {
                Match.CT.JoinPlayer(player);
            }
            else if (Match.Terrorists.HasPlayer(player))
            {
                Match.Terrorists.JoinPlayer(player);
            }
            else if (Match.Spectators.HasPlayer(player))
            {
                Match.Spectators.JoinPlayer(player);
            }
            else
            {
                Server.ExecuteCommand($"kickid {player.UserId}");
            }

        }
        public void PlayerDisconnectHook(CCSPlayerController player)
        {
            if (Match.CT.HasPlayer(player))
            {
                Match.CT.DisconnectPlayer(player);
            }
            else if (Match.Terrorists.HasPlayer(player))
            {
                Match.Terrorists.DisconnectPlayer(player);
            }
        }
        public void GameEndHook()
        {
            if (IsLive)
            {
                if (Match.Team1.Score > Match.Team2.Score)
                {
                    Match.Team1.WonGames++;
                }
                else
                {
                    Match.Team2.WonGames++;
                }
                NextMatchTimer = Utils.CreateDelayedCommand(NextMap, Get5, seconds: 20);
            }
        }

        public void HandlePauseCommand(CCSPlayerController player)
        {
            Team? team = Match.GetTeam(player);
            team?.Pause();
            Pause();
        }

        public void HandleUnPauseCommand(CCSPlayerController player)
        {
            Team? team = Match.GetTeam(player);
            team?.UnPause();
            UnPause();
        }
    }
}