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

        private bool CT_turn = true;
        private bool T_turn = true;

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
        }

        public bool HandleMapVoteChat(CCSPlayerController player, List<string> commandArgs, bool ban)
        {
            if (!this.VoteFinished)
            {
                if ((Utils.PlayerIsTerrorist(player) && T_turn == true) || (Utils.PlayerIsCT(player) && CT_turn == true))
                {
                    string map = commandArgs[1];
                    if (AvailableMaps.HasMap(map))
                    {
                        if (ban)
                        {
                            AvailableMaps.Remove(map);
                            if (AvailableMaps.Count() == NumberOfMaps)
                            {
                                PickedMaps.Update(AvailableMaps);
                            }
                        }
                        else
                        {
                            PickedMaps.Add(map);
                        }
                        ChatMessage.SendAllChatMessage($"Available maps: {AvailableMaps}");
                        ChatMessage.SendAllChatMessage($"Picked maps: {PickedMaps}");
                        ChatMessage.SendAllChatMessage($"Banned maps: {PickedMaps}");

                        if (AvailableMaps.Count() == NumberOfMaps)
                        {
                            VoteFinished = true;
                            ChatMessage.SendAllChatMessage("Vote is finished!");
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
            return VoteFinished;

        }
    }
}