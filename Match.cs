
using CounterStrikeSharp.API.Core;
using System.Text.Json;
using CounterStrikeSharp.API.Modules.Utils;


namespace Get5
{

    public class Match
    {
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }

        private Team? _CT;
        public Team CT
        {
            get { return _CT; }
            set
            {
                _CT = value;
                _CT.CSTeam = CsTeam.CounterTerrorist;
            }
        }

        private Team? _Terrorists;
        public Team Terrorists
        {
            get { return _Terrorists; }
            set
            {
                _Terrorists = value;
                _Terrorists.CSTeam = CsTeam.Terrorist;
            }
        }
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
        private StringChoiceField _MapSides = new StringChoiceField(new List<string> { "team1_ct", "team2_ct", "knife" });
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
        private StringChoiceField _VoteMode = new StringChoiceField(new List<string> { "ban", "pick" });
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

        public void Debug()
        {
            ChatMessage.SendConsoleMessage("Match DEBUG");
            ChatMessage.SendConsoleMessage($"MatchTitle: {MatchTitle}");
            ChatMessage.SendConsoleMessage($"NumMaps: {NumMaps}");
            ChatMessage.SendConsoleMessage($"MinPlayersToReady: {MinPlayersToReady}");
            ChatMessage.SendConsoleMessage($"VoteFirst: {VoteFirst}");
            ChatMessage.SendConsoleMessage($"MapSides: {MapSides}");
            ChatMessage.SendConsoleMessage($"VoteMode: {VoteMode}");
            Team1.Debug();
            Team2.Debug();
            MapList.Debug();
        }
        public Match(string teamName1, string teamName2, int numMaps, int minPlayersToReady, string voteFirst, string mapSides, string voteMode, MapList mapList, string? matchTitle = null)
        {
            this.Team1 = Team.LoadFromJson(teamName1);
            this.Team2 = Team.LoadFromJson(teamName2);
            this.MatchTitle = matchTitle ?? $"{teamName1} vs {teamName2}";
            this.NumMaps = numMaps;
            this.MinPlayersToReady = minPlayersToReady;
            this.VoteFirst = voteFirst;
            this.MapSides = mapSides;
            this.VoteMode = voteMode;
            this.MapList = mapList;

            // WARN: team1 is terrorist and team2 is CT ensures that MapVote goes in the correct order
            if (mapSides == "knife" || mapSides == "team2_ct")
            {
                this.Terrorists = this.Team1;
                this.CT = this.Team2;
            }
            else
            {
                this.CT = this.Team1;
                this.Terrorists = this.Team2;
            }

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

            int minPlayersToReady = jsonEl.TryGetProperty("minPlayersToReady", out JsonElement minPlayersToReadyElement)
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

            return new Match(teamName1: teamName1, teamName2: teamName2, matchTitle: matchTitle, numMaps: numMaps, minPlayersToReady: minPlayersToReady, voteFirst: voteFirst, mapSides: mapSides, voteMode: voteMode, mapList: mapList);
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