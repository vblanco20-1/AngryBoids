﻿using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;

using System.Linq;
using System.Collections.Generic;

namespace CryEngine
{
	public partial class GameRules
	{
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern internal static EntityId _RevivePlayer(EntityId playerId, Vec3 pos, Vec3 rot, int teamId, bool clearInventory);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern internal static EntityId _SpawnPlayer(int channelId, string name, string className, Vec3 pos, Vec3 angles);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern internal static EntityId _GetPlayer();

		public static void RevivePlayer(EntityId playerId, Vec3 pos, Vec3 rot, int teamId = 0, bool clearInventory = true)
		{
			_RevivePlayer(playerId, pos, rot, teamId, clearInventory);
		}

		/// <summary>
		/// Creates the player actor.
		/// </summary>
		/// <param name="channelId"></param>
		/// <param name="name"></param>
		/// <param name="pos"></param>
		/// <param name="angles"></param>
		public static T SpawnPlayer<T>(int channelId, string name, Vec3 pos, Vec3 angles) where T : Actor, new()
		{
			// just in case
			Actor.Remove(channelId);

			EntityId entityId = _SpawnPlayer(channelId, name, "Player", pos, angles);
			if(entityId == 0)
			{
				Debug.LogAlways("GameRules.SpawnPlayer failed; new entityId was invalid");
				return null;
			}

			int scriptId = ScriptCompiler.AddScriptInstance(new T());
			if(scriptId == -1)
			{
				Debug.LogAlways("GameRules.SpawnPlayer failed; new scriptId was invalid");
				return null;
			}

			var player = ScriptCompiler.GetScriptInstanceById(scriptId) as T;
			player.InternalSpawn(entityId, channelId);

			return player;
		}

		public static T GetLocalPlayer<T>() where T : Actor
		{
			return GetPlayer<T>(_GetPlayer());
		}

		public static Actor GetPlayer(EntityId playerId)
		{
			return Entity.GetEntity(playerId) as Actor;
		}

		public static T GetPlayer<T>(EntityId playerId) where T : Actor
		{
			return GetPlayer(playerId) as T;
		}
	}
}