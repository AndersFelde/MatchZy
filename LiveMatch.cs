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

        private bool IsLive = false;
        private bool IsPaused = false;
        private bool IsKnifeRound = false;
        private bool IsWarmup = true;

        private bool IsMapVote = true;



        private Get5 Get5 { get; set; }
        public Match Match { get; set; }

        public MapVote MapVote { get; set; }
        public KnifeRound KnifeRound { get; set; }

        public DamageInfo DamageInfo { get; set; }

        public LiveMatch(Match match, Get5 get5)
        {

            this.Match = match;
            this.Get5 = get5;
            this.MapVote = new MapVote(mapList: this.Match.MapList, liveMatch: this);
            this.KnifeRound = new KnifeRound(this);
            this.DamageInfo = new DamageInfo(this);
            StartWarmup();
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


        public void StartWarmup()
        {
            ChatMessage.SendAllChatMessage("Welcome to the server, we are just warming up");
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
            ChatMessage.SendAllChatMessage("Captains, prepare to vote for maps");
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
            ChatMessage.SendAllChatMessage("Knife round is starting!");
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
            DamageInfo.InitPlayerDamageInfo();
            Server.ExecuteCommand("exec live");
            ChatMessage.SendAllChatMessage("LIVE LIVE LIVE");
            ChatMessage.SendAllChatMessage("LIVE LIVE LIVE");
            ChatMessage.SendAllChatMessage("LIVE LIVE LIVE");

        }
        public void EndLive()
        {
            ChatMessage.SendAllChatMessage("Match ended");
            ChatMessage.SendConsoleMessage($"Match ended {Match.CT.TeamName}: {Match.CT.Score} - {Match.Terrorists.TeamName}: {Match.Terrorists.Score}");
            IsLive = false;
            Get5.LiveMatch = null;
        }

        public void Pause()
        {
            if (!IsPaused)
            {
                IsPaused = true;
                ChatMessage.SendAllChatMessage("Match will pause as soon as possible");
                Server.ExecuteCommand("mp_pause_match");
            }
        }

        public void UnPause()
        {
            if (!Match.CT.IsPaused && !Match.Terrorists.IsPaused)
            {
                IsPaused = false;
                ChatMessage.SendAllChatMessage("Match unpaused");
                Server.ExecuteCommand("mp_unpause_match");
            }
        }

    }
}