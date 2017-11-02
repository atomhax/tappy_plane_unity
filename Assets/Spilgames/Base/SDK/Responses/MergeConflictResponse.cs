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
		public long clientTime;
		public int timezoneOffset;
		public string deviceModel;
		public long serverTime;
		public string appVersion;
	}

	public class MergeConflictData {
		public ConflictedPlayerData playerData;
		public MetaData metaData;
		public List<ConflictedGameState> gameStates;
	}

	public class MergeConflict {
		public MergeConflictData localData;
		public MergeConflictData remoteData;
	}
}