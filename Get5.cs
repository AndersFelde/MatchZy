using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Core.Attributes;

namespace Get5
{
    [MinimumApiVersion(30)]
    public partial class Get5 : BasePlugin
    {
        public override string ModuleName => "Get5";
        public override string ModuleVersion => "0.1";

        public override string ModuleAuthor => "WD- (https://github.com/shobhit-pathak/)";

        public override string ModuleDescription => "A plugin for running and managing CS2 practice/pugs/scrims/matches!";

        public Dictionary<int, CCSPlayerController> playerData = new();
        public static string chatPrefix = "[Get5] ";

        public static string chatCommandPrefix = ".";

        public LiveMatch? LiveMatch { get; set; }

        public void Debug()
        {
            Console.WriteLine("LiveMatch DEBUG");
            Console.WriteLine($"LiveMatch {LiveMatch}");
            Console.WriteLine($"chatPrefix {chatPrefix}");
            Console.WriteLine($"chatCommandPrefix {chatCommandPrefix}");
            LiveMatch?.Debug();
        }

    }

}