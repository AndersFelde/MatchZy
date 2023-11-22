
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

        public void ScoreUpdateHook(EventTeamScore @event)
        {
            if (IsLive)
            {
                Match.GetTeam(@event.Teamid)?.UpdateScore(@event.Score);
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

        public void HandleReadyChat(CCSPlayerController player)
        {
            Match.GetPlayer(player)?.Ready();
            if ((Match.CT.ReadyPlayers() > Match.MinPlayersToReady) && (Match.CT.ReadyPlayers() > Match.MinPlayersToReady))
            {
                EndWarmup();
            }
        }


        public void HandleUnReadyChat(CCSPlayerController player)
        {
            Match.GetPlayer(player)?.UnReady();
        }

        public void PlayerConnectHook(EventPlayerConnectFull @event)
        {
            var player = @event.Userid;
            if (Match.CT.HasPlayer(player))
            {
                Match.CT.JoinPlayer(player);
            }
            else if (Match.Terrorists.HasPlayer(player))
            {
                Match.Terrorists.JoinPlayer(player);
            }
            else
            {
                Server.ExecuteCommand($"kickid {player.UserId}");
            }

        }
        public void PlayerDisconnectHook(EventPlayerDisconnect @event)
        {
            var player = @event.Userid;
            if (Match.CT.HasPlayer(player))
            {
                Match.CT.DisconnectPlayer(player);
            }
            else if (Match.Terrorists.HasPlayer(player))
            {
                Match.Terrorists.DisconnectPlayer(player);
            }
        }
        public void GameEndHook(EventGameEnd @event)
        {
            EndLive();
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