using CryEngine;

using System.Linq;

/// <summary>
/// The campaign game mode is the base game mode
/// </summary>
namespace CryGameCode
{
    [DefaultGamemodeAttribute]
	public class SinglePlayer : GameRulesBase
	{
        public SinglePlayer()
        {
			ReceiveUpdates = true;
        }

		public override void OnUpdate()
		{
			// TODO: Register instantiated Players with CryScriptCompiler to utilize its update functionality.
			if (GameRules.Players == null)
				return;

			foreach (var player in GameRules.Players.Where(x => x.ReceiveUpdates))
				player.OnUpdate();
		}

        public override void OnClientConnect(int channelId, bool isReset = false, string playerName = "")
        {
			Console.LogAlways("OnClientConnect");

            Player player = GameRules.SpawnPlayer<Player>(channelId, "Player", new Vec3(0, 0, 0), new Vec3(0, 0, 0));

            if (player == null)
                Console.Log("OnClientConnect: Failed to spawn the player!");
        }

		public override void OnRevive(uint actorId, Vec3 pos, Vec3 rot, int teamId)
		{
			BasePlayer player = GameRules.GetPlayer(actorId);

			player.Position = new Vec3(541, 510, 146);
			player.Rotation = new Vec3(-90 * ((float)System.Math.PI / 180.0f), 0, 0);

			StaticEntity[] spawnPoints = EntitySystem.GetEntities("SpawnPoint");
			if (spawnPoints == null || spawnPoints.Length < 1)
			{
				Console.LogAlways("$1warning: No spawn points; using default spawn location!");
				return;
			}

			SpawnPoint spawnPoint = spawnPoints[0] as SpawnPoint;
			if (spawnPoint != null)
			{
				Console.LogAlways("Found spawnpoint", spawnPoint.Name);

				player.Position = spawnPoint.Position;
				player.Rotation = spawnPoint.Rotation;
			}
		}
	}
}