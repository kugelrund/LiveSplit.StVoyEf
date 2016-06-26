using System;

namespace LiveSplit.StVoyEf
{
    using ComponentAutosplitter;

    class StVoyEfGame : Game
    {
        private static readonly Type[] eventTypes = new Type[] { typeof(LoadedMapEvent),
                                                                 typeof(FinishedMapEvent),
                                                                 typeof(FinishedHoloMapEvent),
                                                                 typeof(VorsothDeadEvent),
                                                                 typeof(StartedGameEvent) };
        public override Type[] EventTypes => eventTypes;

        public override string Name => "Star Trek: Voyager - Elite Force";
        public override string ProcessName => "stvoy";

        public override GameEvent ReadLegacyEvent(string id)
        {
            // fallback to read old autosplitter settings
            if (id == "new_game_started")
            {
                return new StartedGameEvent();
            }
            else if (id.StartsWith("loaded_map_"))
            {
                return new LoadedMapEvent(id.Replace("loaded_map_", ""));
            }
            else if (id.StartsWith("finished_map_"))
            {
                return new FinishedMapEvent(id.Replace("finished_map_", ""));
            }
            else if (id.StartsWith("finished_holomap_"))
            {
                return new FinishedHoloMapEvent(id.Replace("finished_holomap_", ""));
            }
            else if (id == "vorsoth_dead")
            {
                return new VorsothDeadEvent();
            }
            else if (id == "empty")
            {
                return new EmptyEvent();
            }
            else
            {
                return new EmptyEvent();
            }
        }
    }

    class StartedGameEvent : GameEvent
    {
        private static readonly string[] attributeNames = new string[] { };
        private static string[] attributeValues = new string[] { };

        public override string[] AttributeNames => attributeNames;
        public override string[] AttributeValues
        {
            get { return attributeValues; }
            protected set { attributeValues = value; }
        }
        public override string Description => "The single player mission got started.";

        public override bool HasOccured(GameInfo info)
        {
            return info.CurrGameState == StVoyEfState.LoadingSPStart;
        }

        public override string ToString()
        {
            return "Started a new game";
        }
    }

    class LoadedMapEvent : MapEvent
    {
        public override string Description => "A certain map was loaded.";

        public LoadedMapEvent() : base()
        {
        }

        public LoadedMapEvent(string map) : base(map)
        {
        }

        public override bool HasOccured(GameInfo info)
        {
            return (info.PrevGameState != StVoyEfState.InGame) && info.InGame &&
                   (info.CurrMap == map);
        }

        public override string ToString()
        {
            return "Map '" + map + "' was loaded";
        }
    }

    class FinishedMapEvent : MapEvent
    {
        public override string Description => "A certain map was finished.";

        public FinishedMapEvent() : base()
        {
        }

        public FinishedMapEvent(string map) : base(map)
        {
        }

        public override bool HasOccured(GameInfo info)
        {
            return info.MapChanged && (info.CurrMap != map) && (info.PrevMap == map);
        }

        public override string ToString()
        {
            return "Map '" + map + "' was finished";
        }
    }

    class FinishedHoloMapEvent : FinishedMapEvent
    {
        public override string Description => "A holodeck map was finished.";

        public FinishedHoloMapEvent() : base()
        {
        }

        public FinishedHoloMapEvent(string map) : base(map)
        {
        }

        public override bool HasOccured(GameInfo info)
        {
            return info.CurrMap == map && info.InMenu;
        }
    }

    class VorsothDeadEvent : GameEvent
    {
        private static readonly string[] attributeNames = new string[] { };
        private static string[] attributeValues = new string[] { };

        public override string[] AttributeNames => attributeNames;
        public override string[] AttributeValues
        {
            get { return attributeValues; }
            protected set { attributeValues = value; }
        }
        public override string Description => "Vorsoth was killed.";
        private int prevVorsothHealth = -1;

        public override bool HasOccured(GameInfo info)
        {
            if (info.CurrMap == "forgeboss.bsp")
            {
                int vorsothHealth = info.CurrVorsothHealth;
                if (prevVorsothHealth == 1 && vorsothHealth == 0)
                {
                    prevVorsothHealth = -1;
                    return true;
                }

                prevVorsothHealth = info.CurrVorsothHealth;
            }

            return false;
        }

        public override string ToString()
        {
            return "Vorsoth dead";
        }
    }

    public enum GameVersion
    {
        v11, // patch 1.1
        v12  // patch 1.2
    }

    public enum StVoyEfState
    {
        GameClosing = 0, MainMenu = 1, LoadingSPStart, Loading = 5, InGame = 7
    }
}

namespace LiveSplit.ComponentAutosplitter
{
    using System.Text;
    using System.Windows.Forms;
    using ComponentUtil;
    using StVoyEf;

    partial class GameInfo
    {
        private Int32 gameStateAddress;
        private Int32 mapAddress;
        private Int32 inMenuAddress;
        private DeepPointer vorsothHealthAddress;

        private GameVersion gameVersion;

        public StVoyEfState PrevGameState { get; private set; }
        public StVoyEfState CurrGameState { get; private set; }
        public bool InMenu { get; private set; }
        public string PrevMap { get; private set; }
        public string CurrMap { get; private set; }
        public int CurrVorsothHealth
        {
            get
            {
                int vorsothHealth;
                vorsothHealthAddress.Deref(gameProcess, out vorsothHealth);
                return vorsothHealth;
            }
        }
        public bool MapChanged { get; private set; }

        partial void GetVersion()
        {
            if (gameProcess.MainModuleWow64Safe().ModuleMemorySize == 6635520)
            {
                gameVersion = GameVersion.v11;
            }
            else if (gameProcess.MainModuleWow64Safe().ModuleMemorySize == 7524352)
            {
                gameVersion = GameVersion.v12;
            }
            else
            {
                MessageBox.Show("Unsupported game version", "LiveSplit.StVoyEf",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                gameVersion = GameVersion.v11;
            }

            switch (gameVersion)
            {
                case GameVersion.v11:
                    gameStateAddress = 0xC52D8;
                    mapAddress = 0x1C14FD;
                    vorsothHealthAddress = new DeepPointer(0x424B4, 0x3114);
                    inMenuAddress = 0x269570;
                    break;
                case GameVersion.v12:
                    gameStateAddress = 0xB9E78;
                    mapAddress = 0x1B709D;
                    vorsothHealthAddress = new DeepPointer(0x641C28, 0x7A04);
                    inMenuAddress = 0x269570;
                    break;
            }
        }

        partial void UpdateInfo()
        {
            int currGameState;
            if (gameProcess.ReadValue(baseAddress + gameStateAddress, out currGameState))
            {
                PrevGameState = CurrGameState;
                CurrGameState = (StVoyEfState)currGameState;
            }

            int currInMenu;
            if (gameProcess.ReadValue(baseAddress + inMenuAddress, out currInMenu))
            {
                InMenu = (currInMenu != 0);
            }

            if (PrevGameState != CurrGameState)
            {
                UpdateMap();
                InGame = (CurrGameState == StVoyEfState.InGame);
            }
            else
            {
                MapChanged = false;
            }
        }

        private void UpdateMap()
        {
            StringBuilder mapStringBuilder = new StringBuilder();
            if (gameProcess.ReadString(baseAddress + mapAddress, mapStringBuilder) &&
                mapStringBuilder.ToString() != CurrMap)
            {
                PrevMap = CurrMap;
                CurrMap = mapStringBuilder.ToString();
                MapChanged = true;
            }
        }
    }
}
