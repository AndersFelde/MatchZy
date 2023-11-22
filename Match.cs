
using CounterStrikeSharp.API.Core;
using System.Text.Json;


namespace Get5
{

    public class Match
    {
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }

        public Team CT;
        public Team Terrorists;
        public string MatchTitle { get; set; }
        public int NumMaps { get; set; }
        public int MinPlayersToReady { get; set; }


        private StringChoiceField _VoteFirst = new(new List<string> { "team1", "team2", "random" });

        public string VoteFirst
        {
            get
            {
                return _VoteFirst.Value;

            }
            set
            {
                _VoteFirst.Value = value;
            }
        }
        private StringChoiceField _MapSides = new StringChoiceField(new List<string> { "team1_ct", "team2_t", "knife" });
        public string MapSides
        {
            get
            {
                return _MapSides.Value;

            }
            set
            {
                _MapSides.Value = value;
            }
        }
        private StringChoiceField _VoteMode = new StringChoiceField(new List<string> { "team1_ban", "team2_ban", "team1_pick", "team2_pick" });
        public string VoteMode
        {
            get
            {
                return _VoteMode.Value;

            }
            set
            {
                _VoteMode.Value = value;
            }
        }

        public MapList MapList { get; set; }
        public Match(string teamName1, string teamName2, string? matchTitle = null, int numMaps = 3, int MinPlayersToReady = 5, string voteFirst = "random", string mapSides = "knife", string voteMode = "team1_ban", MapList? mapList = null)
        {
            this.Terrorists = this.Team1 = Team.LoadFromJson(teamName1);
            this.CT = this.Team2 = Team.LoadFromJson(teamName2);
            this.MatchTitle = matchTitle ?? $"{teamName1} vs {teamName2}";
            this.NumMaps = numMaps;
            this.MinPlayersToReady = MinPlayersToReady;
            this.VoteFirst = voteFirst;
            this.MapSides = mapSides;
            this.VoteMode = voteMode;
            this.MapList = mapList ?? new MapList();
        }
        public static Match LoadFromJson(string match_name)
        {
            string json = Utils.ReadConfigFile(match_name);
            JsonElement jsonEl = JsonDocument.Parse(json).RootElement;
            string teamName1 = jsonEl.GetProperty("teamName1").ToString();

            string teamName2 = jsonEl.GetProperty("teamName2").ToString();

            string? matchTitle = jsonEl.TryGetProperty("matchTitle", out JsonElement matchTitleElement)
                ? matchTitleElement.ToString()
                : null;

            int numMaps = jsonEl.TryGetProperty("numMaps", out JsonElement numMapsElement)
                ? numMapsElement.GetInt32()
                : 3;

            int minPlayersToReady = jsonEl.TryGetProperty("MinPlayersToReady", out JsonElement minPlayersToReadyElement)
                ? minPlayersToReadyElement.GetInt32()
                : 5;

            string voteFirst = jsonEl.TryGetProperty("voteFirst", out JsonElement voteFirstElement)
                ? voteFirstElement.ToString()
                : "random";

            string mapSides = jsonEl.TryGetProperty("mapSides", out JsonElement mapSidesElement)
                ? mapSidesElement.ToString()
                : "knife";

            string voteMode = jsonEl.TryGetProperty("voteMode", out JsonElement voteModeElement)
                ? voteModeElement.ToString()
                : "team1_ban";

            List<string> maps = jsonEl.GetProperty("mapList").EnumerateArray().Select(element => element.ToString()).ToList();
            MapList mapList = new(maps);

            return new Match(teamName1, teamName2, matchTitle, numMaps, minPlayersToReady, voteFirst, mapSides, voteMode, mapList);
        }

        public List<Player> GetAllPlayers()
        {
            List<Player> players = new();
            players.AddRange(this.Team1.Players);
            players.AddRange(this.Team2.Players);
            return players;
        }
        public List<Player> GetAllActivePlayers()
        {
            return GetAllPlayers().Where(player => player.PlayerController != null).ToList();
        }

        public Player? GetPlayer(CCSPlayerController player)
        {
            if (this.Team1.HasPlayer(player))
            {
                return this.Team1.GetPlayer(player);
            }
            else if (this.Team2.HasPlayer(player))
            {
                return this.Team2.GetPlayer(player);
            }
            return null;
        }



        public Team? GetTeam(int teamNum)
        {

            if (Globals.TeamNumLookup[teamNum] == "T")
            {
                return this.Terrorists;
            }
            else if (Globals.TeamNumLookup[teamNum] == "CT")
            {
                return this.CT;
            }
            return null;
        }

        public Team? GetTeam(CCSPlayerController player)
        {

            int teamNum = player.TeamNum;
            return GetTeam(teamNum);
        }



    }

}