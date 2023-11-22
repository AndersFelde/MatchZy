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

        public bool VoteFinished = false;

        public int NumberOfMaps = 1;

        public MapVote(MapList mapList)
        {
            this.MapList = mapList;
            this.AvailableMaps = this.MapList;
        }
        public void HandleMapVoteChat(CCSPlayerController player, List<string> commandArgs, bool ban = false, bool pick = false)
        {
            if (this.VoteFinished)
            {
                player.PrintToChat("Vote is finished!");
                return;
            }
            if ((Utils.PlayerIsTerrorist(player) && this.T_turn == true) || (Utils.PlayerIsCT(player) && this.CT_turn == true))
            {
                string map = commandArgs[1];
                if (AvailableMaps.HasMap(map))
                {
                    if (ban)
                    {
                        AvailableMaps.Remove(map);
                        if (AvailableMaps.Count() == this.NumberOfMaps)
                        {
                            PickedMaps.Update(AvailableMaps);
                        }
                    }
                    else
                    {
                        PickedMaps.Add(map);
                    }
                    if (AvailableMaps.Count() == this.NumberOfMaps)
                    {
                        VoteFinished = true;
                    }
                    else
                    {
                        player.PrintToChat("Map not available!");
                        player.PrintToChat($"Available maps: {AvailableMaps}");
                    }


                }

            }
        }
    }
}