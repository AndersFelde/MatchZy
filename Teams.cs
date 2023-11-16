using CounterStrikeSharp.API.Core;
using System.Text.Json;


namespace MatchZy
{
    public class Player
    {
        readonly CCSPlayerController? player;
        public string PlayerName { get; set; }
        public string PlayerSteamID { get; set; }
        public Player(string playerName, string playerSteamID, CCSPlayerController? player)
        {
            this.PlayerName = playerName;
            this.PlayerSteamID = playerSteamID;
            this.player = player;

        }
    }

    public class Team
    {
        public required string teamName;
        public string teamFlag = "";
        public string teamTag = "";

        List<Player> players { get; set; } = new List<Player>();
    }

    public partial class MatchZy
    {
        // Todo: Organize Teams code which can be later used for setting up matches
        public static Team LoadTeamFromJson(string teamName)
        {
            using (StreamReader r = new StreamReader($"{teamName}.json"))
            {
                string json = r.ReadToEnd();
                return JsonSerializer.Deserialize<Team>(json);
            }


        }

    }
}
