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

    }
}