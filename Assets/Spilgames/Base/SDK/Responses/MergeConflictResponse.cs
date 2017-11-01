using System;
using System.Collections.Generic;

namespace SpilGames.Unity.Base.SDK {
	public class ConflictedPlayerData {
		public bool externalChange;
		public WalletData wallet;
		public InventoryData inventory;
	}

	public class ConflictedGameState {
		public string access;
		public string data;
	}

	public class MergeConflictData {
		public Dictionary<string, string> deviceVersions;
		public ConflictedPlayerData playerData;
		public List<ConflictedGameState> gameStates;
	}
}