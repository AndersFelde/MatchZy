
using CounterStrikeSharp.API.Core;
using System.Text.Json;
using CounterStrikeSharp.API.Modules.Utils;
using System.Xml;


namespace Get5
{
    public class Player
    {
        public CCSPlayerController? PlayerController { get; set; }

        public bool IsReady { get; set; } = false;
        public string Name { get; set; }
        private ulong _playerSteamID;
        public ulong SteamID
        {
            get { return _playerSteamID; }
            set
            {
                if (value.ToString().Length != 17)
                {
                    throw new System.ArgumentException("SteamID must be 17 characters long");
                }
                _playerSteamID = value;
            }
        }
        public Player(string name, ulong steamID, CCSPlayerController? playerController = null)
        {
            this.Name = name;
            this.SteamID = steamID;
            this.PlayerController = playerController;

        }

        public static Player LoadFromRawJson(string json)
        {

            JsonElement jsonEl = JsonDocument.Parse(json).RootElement;
            ulong steamID = jsonEl.GetProperty("teamFlag").GetUInt64();
            string name = jsonEl.GetProperty("teamTag").ToString();
            return new Player(name: name, steamID: steamID);
        }

        public void Ready()
        {
            this.IsReady = true;
            if (PlayerController != null)
            {
                ChatMessage.SendPlayerChatMessage(PlayerController, "You are now ready");
                PlayerController.Clan = "[READY]";
            }
        }

        public void UnReady()
        {
            this.IsReady = false;
            if (PlayerController != null)
            {
                ChatMessage.SendPlayerChatMessage(PlayerController, "You are not ready");
                PlayerController.Clan = "[NOT READY]";
            }
        }
    }

    public class Team
    {
        public string TeamName { get; set; }
        public string TeamFlag { get; set; }
        public string TeamTag { get; set; }
        public bool IsPaused = false;

        public CsTeam CSTeam { get; set; }


        public int Score = 0;

        public List<Player> Players { get; set; } = new List<Player>();

        public Team(string teamName, string teamFlag, string teamTag, CsTeam csTeam, List<Player> players)
        {
            this.TeamName = teamName;
            this.TeamFlag = teamFlag;
            this.TeamTag = teamTag;
            this.CSTeam = csTeam;
            this.Players = players;
        }

        public void UpdateScore(int score)
        {
            this.Score = score;
        }

        public bool HasPlayer(ulong steamID)
        {
            return this.Players.Any(player => player.SteamID == steamID);
        }

        public bool HasPlayer(CCSPlayerController player)
        {
            return this.Players.Any(player => player.SteamID == player.SteamID);
        }

        public Player GetPlayer(ulong steamID)
        {
            return this.Players.Find(player => player.SteamID == steamID) ?? throw new System.InvalidOperationException($"Player with SteamID {steamID} not found in team {this.TeamName}");
        }

        public Player GetPlayer(CCSPlayerController player)
        {
            return this.Players.Find(player => player.SteamID == player.SteamID) ?? throw new System.InvalidOperationException($"Player with SteamID {player.SteamID} not found in team {this.TeamName}");
        }

        public static Team LoadFromJson(string team_name, CsTeam csTeam)
        {
            string json = Utils.ReadConfigFile(team_name);
            JsonElement jsonEl = JsonDocument.Parse(json).RootElement;

            string teamFlag = jsonEl.GetProperty("teamFlag").ToString();

            string? teamTag = jsonEl.TryGetProperty("teamTag", out JsonElement teamTagElement)
                ? teamTagElement.ToString()
                : team_name[..2].ToUpper();


            List<Player> players = jsonEl.TryGetProperty("players", out JsonElement playerListElement)
                ? playerListElement.EnumerateArray().Select(p => Player.LoadFromRawJson(p.ToString())).ToList()
                : new List<Player>();

            return new Team(teamName: team_name,
                teamTag: teamTag,
                teamFlag: teamFlag,
                players: players,
                csTeam: csTeam);
            ;
            // string json = Utils.ReadConfigFile($"{teamFlag}.json");
            // return JsonSerializer.Deserialize<Team>(json) ?? throw new InvalidOperationException($"Failed to deserialize the team from {teamFlag}.json");
        }

        public void UnReadyPlayers()
        {
            foreach (var player in Players)
            {
                player.UnReady();
            }
        }

        public void JoinPlayer(CCSPlayerController player)
        {
            GetPlayer(player.SteamID).PlayerController = player;
            player.ChangeTeam(CSTeam);
        }

        public void DisconnectPlayer(CCSPlayerController player)
        {
            var p = GetPlayer(player.SteamID);
            p.PlayerController = null;
            p.IsReady = false;
        }

        public void GiveTeamTags()
        {
            foreach (var player in Players)
            {
                if (player.PlayerController != null)
                {
                    player.PlayerController.Clan = $"[{this.TeamTag}]";
                }
            }
        }

        public int JoinedPlayers()
        {
            int joined = 0;
            foreach (var player in Players)
            {
                if (player.PlayerController != null)
                {
                    joined++;
                }
            }
            return joined;
        }

        public int ReadyPlayers()
        {
            int ready = 0;
            foreach (var player in Players)
            {
                if (player.IsReady)
                {
                    ready++;
                }
            }
            return ready;

        }

        public void Pause()
        {
            ChatMessage.SendAllChatMessage($"{this.TeamName} has paused the game");
            this.IsPaused = true;
        }

        public void UnPause()
        {
            ChatMessage.SendAllChatMessage($"{this.TeamName} wants to unpause the game");
            this.IsPaused = false;
        }

    }
}