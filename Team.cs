
using CounterStrikeSharp.API.Core;
using System.Text.Json;
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
        public Player(string playerName, ulong playerSteamID, CCSPlayerController? playerController)
        {
            this.Name = playerName;
            this.SteamID = playerSteamID;
            this.PlayerController = playerController;

        }

        public void Ready()
        {
            ChatMessage.SendPlayerChatMessage(PlayerController, "You are now ready");
            this.IsReady = true;
            PlayerController.Clan = "[READY]";
        }

        public void UnReady()
        {
            ChatMessage.SendPlayerChatMessage(PlayerController, "You are not ready");
            this.IsReady = false;
            PlayerController.Clan = "[NOT READY]";
        }
    }

    public class Team
    {
        public string TeamName { get; set; }
        public string TeamFlag { get; set; }
        public string TeamTag { get; set; }
        public bool IsPaused = false;

        public int Score = 0;

        public List<Player> Players { get; set; } = new List<Player>();

        public Team(string teamName, string teamFlag, string teamTag)
        {
            this.TeamName = teamName;
            this.TeamFlag = teamFlag;
            this.TeamTag = teamTag;
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

        public static Team LoadFromJson(string teamName)
        {
            string json = Utils.ReadConfigFile($"{teamName}.json");
            return JsonSerializer.Deserialize<Team>(json) ?? throw new InvalidOperationException($"Failed to deserialize the team from {teamName}.json");
        }

        public void JoinPlayer(CCSPlayerController player)
        {
            GetPlayer(player.SteamID).PlayerController = player;
        }

        public void DisconnectPlayer(CCSPlayerController player)
        {
            var p = GetPlayer(player.SteamID);
            p.PlayerController = null;
            p.IsReady = false;
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