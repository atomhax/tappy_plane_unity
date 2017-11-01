using System;
using System.Collections.Generic;

namespace SpilGames.Unity.Base.SDK {
	public class ConflictedPlayerData {
		public WalletData wallet;
		public InventoryData inventory;
	}

	public class ConflictedGameState {
		public string access;
		public string data;
	}

	public class MetaData {
		public int clientTime;
		public int timezoneOffset;
		public int deviceModel;
		public int serverTime;
		public int appVersion;
	}

	public class MergeConflictData {
		public ConflictedPlayerData playerData;
		public MetaData metaData;
		public List<ConflictedGameState> gameStates;
	}
}