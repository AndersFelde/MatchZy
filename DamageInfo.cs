using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;


namespace Get5
{

	public class DamageInfo
	{
		private LiveMatch LiveMatch { get; set; }
		public DamageInfo(LiveMatch liveMatch)
		{
			LiveMatch = liveMatch;
		}

		public void InitPlayerDamageInfo()
		{
			var players = LiveMatch.Match.GetAllActivePlayers();
			for (int i = 0; i < players.Count; i++)
			{
				var player1 = players[i].PlayerController;
				if (player1 == null) continue;
				if (player1.IsBot) continue;
				int attackerId = player1.UserId.Value;
				for (int j = 0; j < players.Count; j++)
				{
					var player2 = players[j].PlayerController;
					if (player2 == null) continue;
					if (player1 == player2) continue;
					if (player2.IsBot) continue;
					if (player1.TeamNum == player2.TeamNum) continue;
					if (player1.TeamNum == 2)
					{
						if (player2.TeamNum != 3) continue;
						int targetId = player2.UserId.Value;
						if (!playerDamageInfo.TryGetValue(attackerId, out var attackerInfo))
							playerDamageInfo[attackerId] = attackerInfo = new Dictionary<int, DamagePlayerInfo>();

						if (!attackerInfo.TryGetValue(targetId, out var targetInfo))
							attackerInfo[targetId] = targetInfo = new DamagePlayerInfo();
					}
					else if (player1.TeamNum == 3)
					{
						if (player2.TeamNum != 2) continue;
						int targetId = player2.UserId.Value;
						if (!playerDamageInfo.TryGetValue(attackerId, out var attackerInfo))
							playerDamageInfo[attackerId] = attackerInfo = new Dictionary<int, DamagePlayerInfo>();

						if (!attackerInfo.TryGetValue(targetId, out var targetInfo))
							attackerInfo[targetId] = targetInfo = new DamagePlayerInfo();
					}
				}
			}
		}

		public Dictionary<int, Dictionary<int, DamagePlayerInfo>> playerDamageInfo = new Dictionary<int, Dictionary<int, DamagePlayerInfo>>();
		public void UpdatePlayerDamageInfo(EventPlayerHurt @event, int targetId)
		{
			int attackerId = (int)@event.Attacker.UserId!;
			if (!playerDamageInfo.TryGetValue(attackerId, out var attackerInfo))
				playerDamageInfo[attackerId] = attackerInfo = new Dictionary<int, DamagePlayerInfo>();

			if (!attackerInfo.TryGetValue(targetId, out var targetInfo))
				attackerInfo[targetId] = targetInfo = new DamagePlayerInfo();

			targetInfo.DamageHP += @event.DmgHealth;
			targetInfo.Hits++;
		}

		public void ShowDamageInfo()
		{
			HashSet<(int, int)> processedPairs = new HashSet<(int, int)>();

			foreach (var entry in playerDamageInfo)
			{
				int attackerId = entry.Key;
				foreach (var (targetId, targetEntry) in entry.Value)
				{
					if (processedPairs.Contains((attackerId, targetId)) || processedPairs.Contains((targetId, attackerId)))
						continue;

					// Access and use the damage information as needed.
					int damageGiven = targetEntry.DamageHP;
					int hitsGiven = targetEntry.Hits;
					int damageTaken = 0;
					int hitsTaken = 0;

					if (playerDamageInfo.TryGetValue(targetId, out var targetInfo) && targetInfo.TryGetValue(attackerId, out var takenInfo))
					{
						damageTaken = takenInfo.DamageHP;
						hitsTaken = takenInfo.Hits;
					}

					var attackerController = Utilities.GetPlayerFromUserid(attackerId);
					var targetController = Utilities.GetPlayerFromUserid(targetId);

					if (attackerController != null && targetController != null)
					{
						int attackerHP = attackerController.PlayerPawn.Value.Health < 0 ? 0 : attackerController.PlayerPawn.Value.Health;
						string attackerName = attackerController.PlayerName;

						int targetHP = targetController.PlayerPawn.Value.Health < 0 ? 0 : targetController.PlayerPawn.Value.Health;
						string targetName = targetController.PlayerName;

						ChatMessage.SendPlayerChatMessage(attackerController, $"To: [{damageGiven} / {hitsGiven} hits] From: [{damageTaken} / {hitsTaken} hits] - {targetName} - ({targetHP} hp)");
						ChatMessage.SendPlayerChatMessage(targetController, $"To: [{damageTaken} / {hitsTaken} hits] From: [{damageGiven} / {hitsGiven} hits] - {attackerName} - ({attackerHP} hp)");
					}

					// Mark this pair as processed to avoid duplicates.
					processedPairs.Add((attackerId, targetId));
				}
			}
			playerDamageInfo.Clear();
		}
	}

	public class DamagePlayerInfo
	{
		public int DamageHP { get; set; } = 0;
		public int Hits { get; set; } = 0;
	}
}
