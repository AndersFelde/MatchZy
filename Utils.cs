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
    class Globals
    {

        static public Dictionary<int, string> TeamNumLookup = new Dictionary<int, string>{
        {2, "T"},
        { 3, "CT" },

    };
    }
    class StringChoiceField
    {
        private string? _Value;
        public string? Value
        {
            get { return _Value; }
            set
            {
                if (value != null && !Choices.Contains(value))
                {
                    throw new ArgumentException($"Invalid value: {value}. Value must be one of the choices.");
                }
                _Value = value;
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
        static public bool PlayerIsTerrorist(CCSPlayerController player)
        {
            return Globals.TeamNumLookup[player.TeamNum] == "T";
        }
        static public bool PlayerIsCT(CCSPlayerController player)
        {
            return Globals.TeamNumLookup[player.TeamNum] == "CT";
        }
        public static void Log(string message)
        {
            Console.WriteLine("[MatchZy] " + message);
        }

    }
}