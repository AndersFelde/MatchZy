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
    public class MapVote
    {
        private MapList AvailableMaps { get; set; }

        public MapList PickedMaps { get; set; } = new MapList();

        public MapList BannedMaps { get; set; } = new MapList();

        private CounterStrikeSharp.API.Modules.Timers.Timer? VoteUpdateTimer;

        private bool T_turn = true;

        private int vote_counter = 0;

        private bool Is_ban = true;

        public bool VoteActive = true;

        public bool VoteFinished = false;


        private LiveMatch LiveMatch { get; set; }

        public MapVote(MapList mapList, LiveMatch liveMatch)
        {
            this.AvailableMaps = mapList;
            this.LiveMatch = liveMatch;
            if (LiveMatch.Match.VoteFirst == "random")
            {

                Random random = new Random();
                int index = random.Next(0, 2);
                if (index == 0)
                {
                    T_turn = false;
                }
                else
                {
                    T_turn = true;
                }
            }
            else if (LiveMatch.Match.VoteFirst == "team1")
            {
                T_turn = false;
            }
            else if (LiveMatch.Match.VoteFirst == "team2")
            {
                T_turn = true;
            }

            if (LiveMatch.Match.VoteMode == "ban")
            {
                Is_ban = true;
            }
            else if (LiveMatch.Match.VoteMode == "pick")
            {
                Is_ban = false;
            }
        }

        public void Debug()
        {
            ChatMessage.SendConsoleMessage("MapVote DEBUG");
            ChatMessage.SendConsoleMessage($"T_turn {T_turn}");
            ChatMessage.SendConsoleMessage($"vote_counter {vote_counter}");
            ChatMessage.SendConsoleMessage($"Is_ban {Is_ban}");
            ChatMessage.SendConsoleMessage($"VoteActive {VoteActive}");
            ChatMessage.SendConsoleMessage($"VoteFinished {VoteFinished}");
            ChatMessage.SendConsoleMessage($"AvailableMaps");
            AvailableMaps.Debug();
            ChatMessage.SendConsoleMessage($"PickedMaps");
            PickedMaps.Debug();

        }

        public void Start()
        {
            VoteActive = true;
            ChatMessage.SendAllChatMessage("Map vote started!");
            PrintVoteStatus();
            VoteUpdateTimer = Utils.CreateContinousChatUpdate(PrintVoteStatus, LiveMatch.Get5, seconds: 30);
            Server.ExecuteCommand("sv_pausable 1");
            Server.ExecuteCommand("pause");
        }

        private void PrintVoteStatus()
        {
            ChatMessage.SendAllChatMessage($"Available maps: {AvailableMaps}");
            ChatMessage.SendAllChatMessage($"Picked maps: {PickedMaps}");
            ChatMessage.SendAllChatMessage($"Banned maps: {BannedMaps}");
            string teamTurnMsg = "CT";
            if (T_turn)
            {
                teamTurnMsg = "Terrorists";

            }
            string voteModeMsg = "pick";
            if (Is_ban)
            {
                voteModeMsg = "ban";
            }
            ChatMessage.SendAllChatMessage($"{teamTurnMsg} needs to {voteModeMsg} a map");
        }

        public void HandleMapVoteChat(CCSPlayerController player, List<string> commandArgs, bool ban)
        {
            if (!this.VoteFinished)
            {
                if ((Utils.PlayerIsTerrorist(player) && T_turn == true) || (Utils.PlayerIsCT(player) && T_turn == false))
                {
                    string map = commandArgs[1];
                    if (AvailableMaps.HasMap(map))
                    {
                        // hvis man bare spiller ett map, banner man til ett map er igjen
                        AvailableMaps.Remove(map);
                        if (ban || LiveMatch.Match.NumMaps == 1)
                        {
                            BannedMaps.Add(map);
                        }
                        else
                        {
                            PickedMaps.Add(map);
                        }
                        FlipVoteTurn();

                        vote_counter++;
                        if (vote_counter == 2)
                        {
                            FlipVoteMode();
                        }

                        if (AvailableMaps.Count() + PickedMaps.Count() == LiveMatch.Match.NumMaps)
                        {
                            PickedMaps.Append(AvailableMaps);
                        }

                        if (PickedMaps.Count() == LiveMatch.Match.NumMaps)
                        {
                            VoteFinished = true;
                            ChatMessage.SendAllChatMessage("Vote is finished!");
                            LiveMatch.EndMapVote();
                            return;
                        }

                        PrintVoteStatus();

                    }
                    else
                    {
                        ChatMessage.SendPlayerChatMessage(player, "Map not available!");
                    }

                }
                else
                {
                    ChatMessage.SendPlayerChatMessage(player, "Not your turn!");
                }
            }
            else
            {

                ChatMessage.SendPlayerChatMessage(player, "Vote is finished!");
            }

        }
        private void FlipVoteMode()
        {
            Is_ban = !Is_ban;
            vote_counter = 0;
        }

        private void FlipVoteTurn()
        {
            T_turn = !T_turn;
        }

        public void End()
        {
            VoteUpdateTimer?.Kill();
            VoteUpdateTimer = null;
            VoteActive = false;
            VoteFinished = true;
            Server.ExecuteCommand("unpause");
            Server.ExecuteCommand("sv_pausable 0");
        }
    }
}