using System;
using System.Reflection;
using LiveSplit.ComponentAutosplitter;
using LiveSplit.Model;
using LiveSplit.UI.Components;

namespace LiveSplit.StVoyEf
{
    class Factory : IComponentFactory
    {
        private StVoyEfGame game = new StVoyEfGame();

        public string ComponentName => game.Name + " Auto Splitter";
        public string Description => "Automates splitting for " + game.Name + " and allows to remove loadtimes.";
        public ComponentCategory Category => ComponentCategory.Control;

        public string UpdateName => ComponentName;
        public string UpdateURL => "https://raw.githubusercontent.com/kugelrund/LiveSplit.StVoyEf/master/";
        public Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        public string XMLURL => UpdateURL + "Components/update.LiveSplit.StVoyEf.xml";

        public IComponent Create(LiveSplitState state)
        {
            return new Component(game, state);
        }
    }
}
