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
    public class LiveMatch
    {

        private bool IsLive = false;
        private bool IsPaused = false;
        private bool IsKnifeRound = false;
        private bool IsWarmup = true;

        private bool IsMapVote = true;


        public Match Match { get; set; }

        private ReverseTeam? ReverseTeam { get; set; }

        public MapVote MapVote { get; set; }
        public KnifeRound KnifeRound { get; set; }

        public LiveMatch(Match match)
        {
            this.Match = match;
            this.MapVote = new MapVote(mapList: this.Match.MapList, liveMatch: this);
            this.KnifeRound = new KnifeRound(this);
        }
        public (int alivePlayers, int totalHealth) GetAlivePlayers(bool terrorists = false, bool CT = false)
        {
            int count = 0;
            int totalHealth = 0;
            List<Player> players = new();
            if (CT)
            {
                players = Match.CT.Players;
            }
            else if (terrorists)
            {
                players = Match.Terrorists.Players;

            }

            foreach (var player in players)
            {
                //makes health 0 if PlayerController is not defined, which will not enter if statement
                if ((player.PlayerController?.PlayerPawn.Value.Health ?? 0) > 0)
                {
                    totalHealth += player.PlayerController.PlayerPawn.Value.Health;
                    count += 0;
                }
            }
            return (count, totalHealth);
        }

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
            }
            else if (IsKnifeRound)
            {
                KnifeRound.HandleKnifeRoundEnd(@event);
            }


        }

        public void PlayerConnectHook(EventPlayerConnectFull @event)
        {
            if (Match.CT.HasPlayer(@event.Userid.SteamID)){
                
            }

        }

        public void StartWarmup()
        {
            IsWarmup = true;
            Server.ExecuteCommand("exec warmup");
        }

        public void EndWarmup()
        {
            IsWarmup = false;
            StartKnifeRound();
        }

        public void StartMapVote()
        {
            IsMapVote = true;
            MapVote.VoteActive = true;
            Server.ExecuteCommand("sv_pausable 1");
            Server.ExecuteCommand("pause");
        }

        public void EndMapVote()
        {
            IsMapVote = false;
            MapVote.VoteActive = false;
            Server.ExecuteCommand("unpause");
            Server.ExecuteCommand("sv_pausable 0");
            StartLive();
        }

        public void StartKnifeRound()
        {
            IsKnifeRound = true;
            KnifeRound.KnifeActive = true;
            Server.ExecuteCommand("exec knife");
        }

        public void EndKnifeRound()
        {
            IsKnifeRound = false;
            KnifeRound.KnifeActive = false;
            StartMapVote();
        }

        public void SwapTeams()
        {
            Server.ExecuteCommand("mp_swapteams");
            (Match.Terrorists, Match.CT) = (Match.CT, Match.Terrorists);
        }

        public void StartLive()
        {
            IsLive = true;
            Server.ExecuteCommand("exec live");

        }
        public void StopLive()
        {
            IsLive = false;
            Server.ExecuteCommand("exec live");

        }

    }
}