using System;
using System.Collections.Generic;

namespace SpilGames.Unity.Base.SDK {
	public class ConflictedPlayerData {
		public WalletData wallet;
		public InventoryData inventory;
	}

	public class ConflictedGameState {
		public List<ConflictedGameStateObject> gameStates;
	}
	
	public class ConflictedGameStateObject {
		public string access;
		public string data;
	}

	public class MetaData {
		public long clientTime;
		public int timezoneOffset;
		public string deviceModel;
		public long serverTime;
		public string appVersion;
	}

	public class MergeConflictData {
		public ConflictedPlayerData playerData;
		public ConflictedGameState gameState;
		public MetaData metaData;
	}

	public class MergeConflict {
		public MergeConflictData localData;
		public MergeConflictData remoteData;
	}
}