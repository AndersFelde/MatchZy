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

        public MapVote(MapList mapList)
        {
            this.MapList = mapList;
            this.AvailableMaps = this.MapList;
        }
        public void HandleMapVoteChat(CCSPlayerController? player, CommandInfo? command)
        {
            if (player == null || command == null)
            {
                return;
            }
            if ((Utils.PlayerIsTerrorist(player) && this.T_turn == true) || (Utils.PlayerIsCT(player) && this.CT_turn == true))
            {
                string voteAction = command.GetArg(0).ToLower();
                int mapIndex = int.Parse(command.GetArg(1));
                if (command.GetArg(1).ToLower() == "ban" && Is_ban)
                {

                }


            }

        }
    }
}