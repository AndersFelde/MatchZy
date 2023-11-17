
using CounterStrikeSharp.API.Core;
using System.Text.Json;


namespace Get5
{
    public class Player
    {
        public CCSPlayerController? PlayerController { get; set; }
        public string PlayerName { get; set; }
        private string _playerSteamID;
        public string PlayerSteamID
        {
            get { return _playerSteamID; }
            set
            {
                if (value.Length != 17)
                {
                    throw new System.ArgumentException("SteamID must be 17 characters long");
                }
                _playerSteamID = value;
            }
        }
        public Player(string playerName, string playerSteamID, CCSPlayerController? playerController = null)
        {
            this.PlayerName = playerName;
            this.PlayerSteamID = playerSteamID;
            this.PlayerController = playerController;

        }
    }

    public class Team
    {
        public string TeamName { get; set; }
        public string TeamFlag { get; set; }
        public string TeamTag { get; set; }

        List<Player> Players { get; set; } = new List<Player>();

        public Team(string teamName, string teamFlag, string teamTag)
        {
            this.TeamName = teamName;
            this.TeamFlag = teamFlag;
            this.TeamTag = teamTag;
        }

    }

    public class ReverseTeam
    {
        public Team TERRORIST { get; set; }
        public Team CT { get; set; }

        public ReverseTeam(Team ct, Team terrorist)
        {
            this.TERRORIST = terrorist;
            this.CT = ct;
        }
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
