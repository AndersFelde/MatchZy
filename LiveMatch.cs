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
using System.Diagnostics;
namespace Get5
{
    public partial class LiveMatch
    {

        private bool IsLive = false;
        private bool IsPaused = false;
        private bool IsKnifeRound = false;
        private bool IsWarmup = true;

        private bool IsMapVote = true;



        public Get5 Get5 { get; set; }
        public Match Match { get; set; }

        public MapVote MapVote { get; set; }
        public KnifeRound KnifeRound { get; set; }

        public DamageInfo DamageInfo { get; set; }
        public Warmup Warmup { get; set; }

        public void Debug(){
            Console.WriteLine("LiveMatch DEBUG");
            Console.WriteLine($"IsLive {IsLive}");
            Console.WriteLine($"IsPaused {IsPaused}");
            Console.WriteLine($"IsKnifeRound {IsKnifeRound}");
            Console.WriteLine($"IsWarmup {IsWarmup}");
            Console.WriteLine($"IsMapVote {IsMapVote}");
            Match.Debug();
            MapVote.Debug();
            KnifeRound.Debug();
            Warmup.Debug();
        }

        public LiveMatch(Match match, Get5 get5)
        {

            this.Match = match;
            this.Get5 = get5;
            this.MapVote = new MapVote(mapList: this.Match.MapList, liveMatch: this);
            this.KnifeRound = new KnifeRound(this);
            this.DamageInfo = new DamageInfo(this);
            this.Warmup = new Warmup(this);
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
                if (player.PlayerController != null && player.PlayerController.PlayerPawn.Value.Health > 0)
                {
                    totalHealth += player.PlayerController.PlayerPawn.Value.Health;
                    count += 0;
                }
            }
            return (count, totalHealth);
        }


        public void StartWarmup()
        {
            Warmup.Start();
            IsWarmup = true;
        }

        public void EndWarmup()
        {
            Warmup.End();
            IsWarmup = false;
            if (MapVote.VoteFinished)
            {
                StartKnifeRound();
            }
            else
            {
                StartMapVote();
            }
        }

        public void StartMapVote()
        {
            MapVote.Start();
            IsMapVote = true;
        }
        private void ChangeToNextMap()
        {

            string map = MapVote.PickedMaps.maps[0];
            MapVote.PickedMaps.RemoveAt(0);
            Server.ExecuteCommand($"changelevel {map}");
        }

        public void EndMapVote()
        {
            MapVote.End();
            IsMapVote = false;
            ChangeToNextMap();
            StartKnifeRound();
        }

        public void StartKnifeRound()
        {
            if (Match.MapSides == "knife")
            {
                KnifeRound.Start();
                IsKnifeRound = true;

            }
            else
            {
                StartLive();
            }
        }

        public void EndKnifeRound()
        {
            IsKnifeRound = false;
            KnifeRound.End();
            StartLive();
        }


        public void StartLive()
        {
            IsLive = true;
            Server.ExecuteCommand("exec live");
            ChatMessage.SendAllChatMessage("LIVE LIVE LIVE");
            ChatMessage.SendAllChatMessage("LIVE LIVE LIVE");
            ChatMessage.SendAllChatMessage("LIVE LIVE LIVE");

        }

        public void NextMap()
        {
            if (MapVote.PickedMaps.Count() == 0)
            {
                EndLive();
                return;
            }

            ChangeToNextMap();
            StartWarmup();
        }
        public void EndLive()
        {
            ChatMessage.SendAllChatMessage("Match ended");
            ChatMessage.SendConsoleMessage($"Match ended {Match.CT.TeamName}: {Match.CT.Score} - {Match.Terrorists.TeamName}: {Match.Terrorists.Score}");
            IsLive = false;
            Get5.LiveMatch = null;
        }

        public void SwapTeams()
        {
            Server.ExecuteCommand("mp_swapteams");
            (Match.Terrorists, Match.CT) = (Match.CT, Match.Terrorists);
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