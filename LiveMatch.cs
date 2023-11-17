using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
namespace Get5
{
    public class LiveMatch
    {

        private bool IsLive = false;
        private bool IsPaused = false;
        private bool IsKnifeRound = false;
        private bool IsWarmup = true;

        private int PlayedRounds = 0;

        private Match Match { get; set; }

        private ReverseTeam? ReverseTeam { get; set; }

        public LiveMatch(Match match)
        {
            this.Match = match;
        }

        public void StartMatch()
        {
            IsWarmup = false;
            IsLive = true;
            Server.ExecuteCommand("exec live");
        }

        public static void HandleMapVoteChat(CCSPlayerController? player, CommandInfo? command)
        {
            player.TeamNum;

        }

    }
}