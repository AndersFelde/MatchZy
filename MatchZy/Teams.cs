using CounterStrikeSharp.API.Core;
using System.Text.Json;


namespace MatchZy
{
    public class Player
    {
        public CCSPlayerController? PlayerController { get; set; }
        public string PlayerName { get; set; }
        public string PlayerSteamID { get; set; }
        public Player(string playerName, string playerSteamID, CCSPlayerController? playerController = null)
        {
            this.PlayerName = playerName;
            this.PlayerSteamID = playerSteamID;
            this.PlayerController = playerController;

        }
    }

    public class Team
    {
        public required string teamName;
        public string teamFlag = "";
        public string teamTag = "";

        List<Player> Players { get; set; } = new List<Player>();

    }

    public partial class MatchZy
    {
        // Todo: Organize Teams code which can be later used for setting up matches
        public Team? Team1 { get; set; }
        public Team? Team2 { get; set; }
        private static Team LoadTeamFromJson(string teamName)
        {
            using (StreamReader r = new StreamReader($"{teamName}.json"))
            {
                string json = r.ReadToEnd();
                return JsonSerializer.Deserialize<Team>(json) ?? throw new InvalidOperationException($"Failed to deserialize the team from {teamName}.json");
            }


        }
        public void LoadTeams(string teamName1, string teamName2)
        {
            this.Team1 = LoadTeamFromJson(teamName1);
            this.Team2 = LoadTeamFromJson(teamName2);
        }

    }
}
