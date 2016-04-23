using System;
using System.Diagnostics;
using LiveSplit.ComponentUtil;
using System.Windows.Forms;

namespace LiveSplit.StVoyEf
{
    class GameInfo
    {
        // 0 - when the game is closing
        // 1 - main menu
        // 2 - load state (unknown)
        // 3 - load state (only when loading first level)
        // 4 - load state (at the beginning of each loading apart from the first level)
        // 5 - main loading state (when the controls are blinking)
        // 6 - end loading state (directly after loading)
        // 7 - in game
        // 8 - videos when starting the game
        private DeepPointer gameStateAddress;

        private DeepPointer mapAddress;

        private DeepPointer vorsothHealthAddress;



        // longest map name is forgeboss
        private const int MAX_MAP_LENGTH = 13;

        private Process gameProcess;
        private GameVersion gameVersion;

        public int PrevGameState { get; private set; }
        public int CurrGameState { get; private set; }
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
        public bool InGame { get; private set; }


        public GameInfo(Process gameProcess)
        {
            this.gameProcess = gameProcess;

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
                    gameStateAddress = new DeepPointer(0xC52D8);
                    mapAddress = new DeepPointer(0x1C14FD);
                    vorsothHealthAddress = new DeepPointer(0x424B4, 0x3114);
                    break;
                case GameVersion.v12:
                    gameStateAddress = new DeepPointer(0xB9E78);
                    mapAddress = new DeepPointer(0x1B709D);
                    vorsothHealthAddress = new DeepPointer(0x641C28, 0x7A04);
                    break;
            }
        }

        private void UpdateMap()
        {
            string map = mapAddress.DerefString(gameProcess, MAX_MAP_LENGTH);
            if (map != null && map != CurrMap)
            {
                PrevMap = CurrMap;
                CurrMap = map;
                MapChanged = true;
            }
        }

        public void Update()
        {
            PrevGameState = CurrGameState;
            int currGameState;
            if (gameStateAddress.Deref(gameProcess, out currGameState))
            {
                CurrGameState = currGameState;
            }

            if (PrevGameState != CurrGameState)
            {
                UpdateMap();
                InGame = (CurrGameState == 7);
            }
            else
            {
                MapChanged = false;
            }
        }
    }

    abstract class GameEvent : IComparable<GameEvent>
    {
        private static GameEvent[] eventList = null;

        private int order = -1;
        public int Order { get { return order; } }
        public abstract string Id { get; }

        public static GameEvent[] GetEventList()
        {
            if (eventList == null)
            {
                eventList = new GameEvent[] { 
                    #region events
                    new StartedGameEvent(),
                    new LoadedMapEvent("borg1.bsp"),
                    new FinishedMapEvent("borg1.bsp"),
                    new LoadedMapEvent("borg2.bsp"),
                    new FinishedMapEvent("borg2.bsp"),
                    new LoadedMapEvent("holodeck.bsp"),
                    new FinishedMapEvent("holodeck.bsp"),
                    new LoadedMapEvent("voy1.bsp"),
                    new FinishedMapEvent("voy1.bsp"),
                    new LoadedMapEvent("voy2.bsp"),
                    new FinishedMapEvent("voy2.bsp"),
                    new LoadedMapEvent("voy3.bsp"),
                    new FinishedMapEvent("voy3.bsp"),
                    new LoadedMapEvent("voy4.bsp"),
                    new FinishedMapEvent("voy4.bsp"),
                    new LoadedMapEvent("voy5.bsp"),
                    new FinishedMapEvent("voy5.bsp"),
                    new LoadedMapEvent("stasis1.bsp"),
                    new FinishedMapEvent("stasis1.bsp"),
                    new LoadedMapEvent("stasis2.bsp"),
                    new FinishedMapEvent("stasis2.bsp"),
                    new LoadedMapEvent("stasis3.bsp"),
                    new FinishedMapEvent("stasis3.bsp"),
                    new LoadedMapEvent("voy6.bsp"),
                    new FinishedMapEvent("voy6.bsp"),
                    new LoadedMapEvent("voy7.bsp"),
                    new FinishedMapEvent("voy7.bsp"),
                    new LoadedMapEvent("voy8.bsp"),
                    new FinishedMapEvent("voy8.bsp"),
                    new LoadedMapEvent("scav1.bsp"),
                    new FinishedMapEvent("scav1.bsp"),
                    new LoadedMapEvent("scav2.bsp"),
                    new FinishedMapEvent("scav2.bsp"),
                    new LoadedMapEvent("scav3.bsp"),
                    new FinishedMapEvent("scav3.bsp"),
                    new LoadedMapEvent("scav3b.bsp"),
                    new FinishedMapEvent("scav3b.bsp"),
                    new LoadedMapEvent("scav4.bsp"),
                    new FinishedMapEvent("scav4.bsp"),
                    new LoadedMapEvent("scav5.bsp"),
                    new FinishedMapEvent("scav5.bsp"),
                    new LoadedMapEvent("scavboss.bsp"),
                    new FinishedMapEvent("scavboss.bsp"),
                    new LoadedMapEvent("voy9.bsp"),
                    new FinishedMapEvent("voy9.bsp"),
                    new LoadedMapEvent("borg3.bsp"),
                    new FinishedMapEvent("borg3.bsp"),
                    new LoadedMapEvent("borg4.bsp"),
                    new FinishedMapEvent("borg4.bsp"),
                    new LoadedMapEvent("borg5.bsp"),
                    new FinishedMapEvent("borg5.bsp"),
                    new LoadedMapEvent("borg6.bsp"),
                    new FinishedMapEvent("borg6.bsp"),
                    new LoadedMapEvent("voy13.bsp"),
                    new FinishedMapEvent("voy13.bsp"),
                    new LoadedMapEvent("voy14.bsp"),
                    new FinishedMapEvent("voy14.bsp"),
                    new LoadedMapEvent("voy15.bsp"),
                    new FinishedMapEvent("voy15.bsp"),
                    new LoadedMapEvent("dn1.bsp"),
                    new FinishedMapEvent("dn1.bsp"),
                    new LoadedMapEvent("dn2.bsp"),
                    new FinishedMapEvent("dn2.bsp"),
                    new LoadedMapEvent("dn3.bsp"),
                    new FinishedMapEvent("dn3.bsp"),
                    new LoadedMapEvent("dn4.bsp"),
                    new FinishedMapEvent("dn4.bsp"),
                    new LoadedMapEvent("dn5.bsp"),
                    new FinishedMapEvent("dn5.bsp"),
                    new LoadedMapEvent("train.bsp"),
                    new FinishedMapEvent("train.bsp"),
                    new LoadedMapEvent("dn6.bsp"),
                    new FinishedMapEvent("dn6.bsp"),
                    new LoadedMapEvent("dn8.bsp"),
                    new FinishedMapEvent("dn8.bsp"),
                    new LoadedMapEvent("voy16.bsp"),
                    new FinishedMapEvent("voy16.bsp"),
                    new LoadedMapEvent("voy17.bsp"),
                    new FinishedMapEvent("voy17.bsp"),
                    new LoadedMapEvent("forge1.bsp"),
                    new FinishedMapEvent("forge1.bsp"),
                    new LoadedMapEvent("forge2.bsp"),
                    new FinishedMapEvent("forge2.bsp"),
                    new LoadedMapEvent("forge3.bsp"),
                    new FinishedMapEvent("forge3.bsp"),
                    new LoadedMapEvent("forge4.bsp"),
                    new FinishedMapEvent("forge4.bsp"),
                    new LoadedMapEvent("forge5.bsp"),
                    new FinishedMapEvent("forge5.bsp"),
                    new LoadedMapEvent("forgeboss.bsp"),
                    new VorsothDeadEvent(),
                    new FinishedMapEvent("forgeboss.bsp"),
                    new LoadedMapEvent("voy20.bsp"),
                    new FinishedMapEvent("voy20.bsp")
                    #endregion
                };
                for (int i = 0; i < eventList.Length; ++i)
                {
                    eventList[i].order = i;
                }
            }

            return eventList;
        }

        public int CompareTo(GameEvent other)
        {
            if (order == -1 || other.order == -1)
            {
                throw new ArgumentException();
            }
            else
            {
                return order - other.order;
            }
        }

        public abstract bool HasOccured(GameInfo info);
    }

    class StartedGameEvent : GameEvent
    {
        public override string Id { get { return "new_game_started"; } }

        public override bool HasOccured(GameInfo info)
        {
            return info.CurrGameState == 3;
        }

        public override string ToString()
        {
            return "Started a new game";
        }
    }

    abstract class MapEvent : GameEvent
    {
        protected readonly string map;

        public MapEvent(string map)
        {
            this.map = map;
        }
    }

    class LoadedMapEvent : MapEvent
    {
        public override string Id { get { return "loaded_map_" + map; } }

        public LoadedMapEvent(string map) : base(map) { }

        public override bool HasOccured(GameInfo info)
        {
            return (info.PrevGameState != 7) && info.InGame && (info.CurrMap == map);
        }
                
        public override string ToString()
        {
            return "Loaded '" + map + "'";
        }
    }

    class FinishedMapEvent : MapEvent
    {
        public override string Id { get { return "finished_map_" + map; } }

        public FinishedMapEvent(string map) : base(map) { }

        public override bool HasOccured(GameInfo info)
        {
            return info.MapChanged && (info.CurrMap != map) && (info.PrevMap == map);
        }

        public override string ToString()
        {
            return "Finished '" + map + "'";
        }
    }

    class VorsothDeadEvent : GameEvent
    {
        private int prevVorsothHealth = -1;

        public override string Id { get { return "vorsoth_dead"; } }

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

    class EmptyEvent : GameEvent
    {
        public override string Id { get { return "empty"; } }

        public override bool HasOccured(GameInfo info)
        {
            return false;
        }
    }

    public enum GameVersion
    {
        v11, // patch 1.1
        v12  // patch 1.2
    }
}
