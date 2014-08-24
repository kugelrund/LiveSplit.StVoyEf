using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Reflection;

namespace LiveSplit.StVoyEf
{
    public class Factory : IComponentFactory
    {
        public string ComponentName
        {
            get { return "Star Trek: Voyager - Elite Force Auto Splitter"; }
        }
        public ComponentCategory Category
        {
            get { return ComponentCategory.Control; }
        }
        public string Description
        {
            get { return "Automates splitting for Star Trek Voyager - Elite Force and allows to remove loadtimes."; }
        }
        public IComponent Create(LiveSplitState state)
        {
            return new Component(state);
        }
        public string UpdateName
        {
            get { return ComponentName; }
        }
        public string UpdateURL
        {
            get { return "https://raw.githubusercontent.com/kugelrund/LiveSplit.StVoyEf/master/"; }
        }
        public Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
        public string XMLURL
        {
            get { return UpdateURL + "Components/update.LiveSplit.StVoyEf.xml"; }
        }
    }
}