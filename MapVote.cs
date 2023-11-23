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
        public MapList MapList { get; set; }
        private MapList AvailableMaps { get; set; }

        public MapList PickedMaps { get; set; } = new MapList();

        private bool T_turn = true;

        private int vote_counter = 0;

        private bool Is_ban = true;

        public bool VoteActive = true;

        public bool VoteFinished = false;

        public int NumberOfMaps = 1;

        private LiveMatch LiveMatch { get; set; }

        public MapVote(MapList mapList, LiveMatch liveMatch)
        {
            this.MapList = mapList;
            this.AvailableMaps = this.MapList;
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
                        if (ban || NumberOfMaps == 1)
                        {
                            AvailableMaps.Remove(map);
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


                        ChatMessage.SendAllChatMessage($"Available maps: {AvailableMaps}");
                        ChatMessage.SendAllChatMessage($"Picked maps: {PickedMaps}");
                        ChatMessage.SendAllChatMessage($"Banned maps: {PickedMaps}");

                        if (AvailableMaps.Count() + PickedMaps.Count() == NumberOfMaps)
                        {
                            PickedMaps.Append(AvailableMaps);
                        }

                        if (PickedMaps.Count() == NumberOfMaps)
                        {
                            VoteFinished = true;
                            ChatMessage.SendAllChatMessage("Vote is finished!");
                            LiveMatch.EndMapVote();
                        }


                    }
                    else
                    {
                        ChatMessage.SendPlayerChatMessage(player, "Map not available!");
                        ChatMessage.SendPlayerChatMessage(player, $"Available maps: {AvailableMaps}");
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
        public void PrintAvailableMaps()
        {
            ChatMessage.SendAllChatMessage($"Available maps: {AvailableMaps}");
        }
    }
}