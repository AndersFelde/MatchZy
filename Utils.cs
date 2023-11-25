using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using System.Net.NetworkInformation;
using CounterStrikeSharp.API.Modules.Cvars;

namespace Get5
{
    class Globals
    {

        static public Dictionary<int, CsTeam> TeamNumLookup = new Dictionary<int, CsTeam>{
        {0, CsTeam.None},
        {1, CsTeam.Spectator},
        {2, CsTeam.Terrorist},
        {3, CsTeam.CounterTerrorist },

    };
        public static string ConfigPath = Path.Join(Server.GameDirectory + "/csgo/cfg/get5/");
        public static void Debug()
        {
            ChatMessage.SendConsoleMessage("Globals DEBUG");
            ChatMessage.SendConsoleMessage($"TeamNumLookup {TeamNumLookup}");
            ChatMessage.SendConsoleMessage($"ConfigPath {ConfigPath}");
        }
    }

    class StringChoiceField
    {
        private string _Value = "";
        public string Value
        {
            get { return _Value; }
            set
            {
                if (value != null)
                {
                    if (Choices.Contains(value))
                    {
                        _Value = value;
                        return;
                    }
                }
                throw new ArgumentException($"Invalid value: {value}. Value must be one of the choices.");
            }
        }
        List<string> Choices { get; set; }

        public StringChoiceField(List<string> choices)
        {
            this.Choices = choices;
        }
    }
    class Utils
    {
        static public bool IsTerrorist(CCSPlayerController player)
        {
            return Globals.TeamNumLookup[player.TeamNum] == CsTeam.Terrorist;
        }
        static public bool IsTerrorist(int teamNum)
        {
            return Globals.TeamNumLookup[teamNum] == CsTeam.Terrorist;
        }
        static public bool IsCT(CCSPlayerController player)
        {
            return Globals.TeamNumLookup[player.TeamNum] == CsTeam.CounterTerrorist;
        }
        static public bool IsCT(int teamNum)
        {
            return Globals.TeamNumLookup[teamNum] == CsTeam.CounterTerrorist;
        }
        public static void Log(string message)
        {
            ChatMessage.SendConsoleMessage(message);
        }
        public static string ReadConfigFile(string filename, bool json = true)
        {
            filename += json ? ".json" : "";
            var path = Path.Join(Globals.ConfigPath, filename);
            return File.ReadAllText(path);
        }

        public static CounterStrikeSharp.API.Modules.Timers.Timer CreateContinousChatUpdate(Action callback, Get5 get5, int seconds = 30)
        {
            return get5.AddTimer(seconds, callback, TimerFlags.REPEAT);
        }

        public static CounterStrikeSharp.API.Modules.Timers.Timer CreateDelayedCommand(Action callback, Get5 get5, int seconds = 20)
        {
            return get5.AddTimer(seconds, callback, TimerFlags.REPEAT);
        }

        public static bool IsTeamSwapRequired()
        {
            // Handling OTs and side swaps (Referred from Get5)
            var gameRules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!;
            int roundsPlayed = gameRules.TotalRoundsPlayed;

            int roundsPerHalf = ConVar.Find("mp_maxrounds").GetPrimitiveValue<int>() / 2;
            int roundsPerOTHalf = ConVar.Find("mp_overtime_maxrounds").GetPrimitiveValue<int>() / 2;

            bool halftimeEnabled = ConVar.Find("mp_halftime").GetPrimitiveValue<bool>();

            if (halftimeEnabled)
            {
                if (roundsPlayed == roundsPerHalf)
                {
                    return true;
                }
                // Now in OT.
                if (roundsPlayed >= 2 * roundsPerHalf)
                {
                    int otround = roundsPlayed - 2 * roundsPerHalf;  // round 33 -> round 3, etc.
                    // Do side swaps at OT halves (rounds 3, 9, ...)
                    if ((otround + roundsPerOTHalf) % (2 * roundsPerOTHalf) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}