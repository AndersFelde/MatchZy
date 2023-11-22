
using CounterStrikeSharp.API.Core;
using System.Text.Json;


namespace Get5
{

    public class Match
    {
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
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
            this.Team1 = LoadTeamFromJson(teamName1);
            this.Team2 = LoadTeamFromJson(teamName2);
            this.MatchTitle = matchTitle ?? $"{teamName1} vs {teamName2}";
            this.NumMaps = numMaps;
            this.MinPlayersToReady = MinPlayersToReady;
            this.VoteFirst = voteFirst;
            this.MapSides = mapSides;
            this.VoteMode = voteMode;
            this.MapList = mapList ?? new MapList();
        }

        private static Team LoadTeamFromJson(string teamName)
        {
            using (StreamReader r = new StreamReader($"{teamName}.json"))
            {
                string json = r.ReadToEnd();
                return JsonSerializer.Deserialize<Team>(json) ?? throw new InvalidOperationException($"Failed to deserialize the team from {teamName}.json");
            }


        }
    }

}