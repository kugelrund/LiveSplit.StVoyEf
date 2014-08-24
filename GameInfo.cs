using System;
using System.Diagnostics;

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
        private static readonly MemoryAddress gameStateAddress = new MemoryAddress(0x4B9E78);

        private static readonly MemoryAddress mapAddress = new MemoryAddress(0x20284A98);

        private static readonly MemoryAddress vorsothHealthAddress = new MemoryAddress(0x2028C2B4);

        // might implement if desired
        //private static readonly DeepPointer gameTimer = new DeepPointer("stvoy.exe", 0x641C14, 0x510);


        // longest map name is forgeboss
        private const int MAX_MAP_LENGTH = 9;

        private Process gameProcess;

        public int PrevGameState { get; private set; }
        public int CurrGameState { get; private set; }
        public string PrevMap { get; private set; }
        public string CurrMap { get; private set; }
        public int CurrVorsothHealth { get { return vorsothHealthAddress.Deref(gameProcess); } }
        public bool MapChanged { get; private set; }
        public bool InGame { get; private set; }


        public GameInfo(Process gameProcess)
        {
            this.gameProcess = gameProcess;
        }

        private void UpdateMap()
        {
            string map = mapAddress.Deref(gameProcess, MAX_MAP_LENGTH);
            if (map.Length > 0 && map != CurrMap)
            {
                PrevMap = CurrMap;
                CurrMap = map;
                MapChanged = true;
            }
        }

        public void Update()
        {
            PrevGameState = CurrGameState;
            CurrGameState = gameStateAddress.Deref(gameProcess);

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
                    new LoadedMapEvent("borg1"),
                    new FinishedMapEvent("borg1"),
                    new LoadedMapEvent("borg2"),
                    new FinishedMapEvent("borg2"),
                    new LoadedMapEvent("holodeck"),
                    new FinishedMapEvent("holodeck"),
                    new LoadedMapEvent("voy1"),
                    new FinishedMapEvent("voy1"),
                    new LoadedMapEvent("voy2"),
                    new FinishedMapEvent("voy2"),
                    new LoadedMapEvent("voy3"),
                    new FinishedMapEvent("voy3"),
                    new LoadedMapEvent("voy4"),
                    new FinishedMapEvent("voy4"),
                    new LoadedMapEvent("voy5"),
                    new FinishedMapEvent("voy5"),
                    new LoadedMapEvent("stasis1"),
                    new FinishedMapEvent("stasis1"),
                    new LoadedMapEvent("stasis2"),
                    new FinishedMapEvent("stasis2"),
                    new LoadedMapEvent("stasis3"),
                    new FinishedMapEvent("stasis3"),
                    new LoadedMapEvent("voy6"),
                    new FinishedMapEvent("voy6"),
                    new LoadedMapEvent("voy7"),
                    new FinishedMapEvent("voy7"),
                    new LoadedMapEvent("voy8"),
                    new FinishedMapEvent("voy8"),
                    new LoadedMapEvent("scav1"),
                    new FinishedMapEvent("scav1"),
                    new LoadedMapEvent("scav2"),
                    new FinishedMapEvent("scav2"),
                    new LoadedMapEvent("scav3"),
                    new FinishedMapEvent("scav3"),
                    new LoadedMapEvent("scav3b"),
                    new FinishedMapEvent("scav3b"),
                    new LoadedMapEvent("scav4"),
                    new FinishedMapEvent("scav4"),
                    new LoadedMapEvent("scav5"),
                    new FinishedMapEvent("scav5"),
                    new LoadedMapEvent("scavboss"),
                    new FinishedMapEvent("scavboss"),
                    new LoadedMapEvent("voy9"),
                    new FinishedMapEvent("voy9"),
                    new LoadedMapEvent("borg3"),
                    new FinishedMapEvent("borg3"),
                    new LoadedMapEvent("borg4"),
                    new FinishedMapEvent("borg4"),
                    new LoadedMapEvent("borg5"),
                    new FinishedMapEvent("borg5"),
                    new LoadedMapEvent("borg6"),
                    new FinishedMapEvent("borg6"),
                    new LoadedMapEvent("voy13"),
                    new FinishedMapEvent("voy13"),
                    new LoadedMapEvent("voy14"),
                    new FinishedMapEvent("voy14"),
                    new LoadedMapEvent("voy15"),
                    new FinishedMapEvent("voy15"),
                    new LoadedMapEvent("dn1"),
                    new FinishedMapEvent("dn1"),
                    new LoadedMapEvent("dn2"),
                    new FinishedMapEvent("dn2"),
                    new LoadedMapEvent("dn3"),
                    new FinishedMapEvent("dn3"),
                    new LoadedMapEvent("dn4"),
                    new FinishedMapEvent("dn4"),
                    new LoadedMapEvent("dn5"),
                    new FinishedMapEvent("dn5"),
                    new LoadedMapEvent("train"),
                    new FinishedMapEvent("train"),
                    new LoadedMapEvent("dn6"),
                    new FinishedMapEvent("dn6"),
                    new LoadedMapEvent("dn7"),
                    new FinishedMapEvent("dn7"),
                    new LoadedMapEvent("dn8"),
                    new FinishedMapEvent("dn8"),
                    new LoadedMapEvent("voy16"),
                    new FinishedMapEvent("voy16"),
                    new LoadedMapEvent("voy17"),
                    new FinishedMapEvent("voy17"),
                    new LoadedMapEvent("forge1"),
                    new FinishedMapEvent("forge1"),
                    new LoadedMapEvent("forge2"),
                    new FinishedMapEvent("forge2"),
                    new LoadedMapEvent("forge3"),
                    new FinishedMapEvent("forge3"),
                    new LoadedMapEvent("forge4"),
                    new FinishedMapEvent("forge4"),
                    new LoadedMapEvent("forge5"),
                    new FinishedMapEvent("forge5"),
                    new LoadedMapEvent("forgeboss"),
                    new VorsothDeadEvent(),
                    new FinishedMapEvent("forgeboss"),
                    new LoadedMapEvent("voy20"),
                    new FinishedMapEvent("voy20")
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
        private readonly LoadedMapEvent mapForgebossEvent = new LoadedMapEvent("forgeboss");
        private bool mapIsForgeboss = false;
        private int prevVorsothHealth = -1;

        public override string Id { get { return "vorsoth_dead"; } }

        public override bool HasOccured(GameInfo info)
        {
            if (mapIsForgeboss && !info.MapChanged)
            {
                int vorsothHealth = info.CurrVorsothHealth;
                if (prevVorsothHealth == 1 && vorsothHealth == 0)
                {
                    mapIsForgeboss = false;
                    prevVorsothHealth = -1;
                    return true;
                }
                
                prevVorsothHealth = info.CurrVorsothHealth;
            }
            else
            {
                mapIsForgeboss = mapForgebossEvent.HasOccured(info);
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
}
