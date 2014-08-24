using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.StVoyEf
{
    partial class Settings : UserControl
    {
        private readonly GameEvent[] eventList;
        private Dictionary<string, int> order;
        private List<GameEvent> availEvents;
        private List<GameEvent> usedEvents;

        public bool PauseGameTime { get; private set; }

        public Settings()
        {
            InitializeComponent();

            eventList = GameEvent.GetEventList();
            order = new Dictionary<string, int>();
            for (int i = 0; i < eventList.Length; ++i)
            {
                order.Add(eventList[i].Id, i);
            }

            availEvents = new List<GameEvent>(eventList);
            usedEvents = new List<GameEvent>();
            PauseGameTime = false;

            bindingUsedEvents.DataSource = usedEvents;
            bindingAvailEvents.DataSource = availEvents;
        }

        public GameEvent[] GetEventList()
        {
            List<GameEvent> eventList = new List<GameEvent>(usedEvents);
            eventList.Add(new EmptyEvent());
            return eventList.ToArray();
        }

        private void btnAddEvent_Click(object sender, EventArgs e)
        {
            foreach (Object item in lstAvailEvents.SelectedItems)
            {
                availEvents.Remove((GameEvent) item);
                usedEvents.Add((GameEvent) item);
            }
            usedEvents.Sort();
            availEvents.Sort();
            bindingUsedEvents.ResetBindings(true);
            bindingAvailEvents.ResetBindings(true);
        }

        private void btnRemoveEvent_Click(object sender, EventArgs e)
        {
            foreach (Object item in lstUsedEvents.SelectedItems)
            {
                usedEvents.Remove((GameEvent) item);
                availEvents.Add((GameEvent) item);
            }
            usedEvents.Sort();
            availEvents.Sort();
            bindingUsedEvents.ResetBindings(true);
            bindingAvailEvents.ResetBindings(true);
        }

        private void btnAllEvents_Click(object sender, EventArgs e)
        {
            usedEvents.Clear();
            usedEvents.AddRange(eventList);
            availEvents.Clear();
            bindingUsedEvents.ResetBindings(true);
            bindingAvailEvents.ResetBindings(true);
        }

        private void btnNoEvents_Click(object sender, EventArgs e)
        {
            availEvents.Clear();
            availEvents.AddRange(eventList);
            usedEvents.Clear();
            bindingUsedEvents.ResetBindings(true);
            bindingAvailEvents.ResetBindings(true);
        }

        private void chkPauseGameTime_CheckedChanged(object sender, EventArgs e)
        {
            PauseGameTime = chkPauseGameTime.Checked;
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            XmlElement settingsNode = document.CreateElement("settings");

            XmlElement usedEventsNode = document.CreateElement("usedEvents");
            XmlElement eventNode;
            foreach (GameEvent gameEvent in usedEvents)
            {
                eventNode = document.CreateElement("event");
                eventNode.InnerText = gameEvent.Id;
                usedEventsNode.AppendChild(eventNode);
            }
            settingsNode.AppendChild(usedEventsNode);

            XmlElement pauseGameTimeNode = document.CreateElement("pauseGameTime");
            pauseGameTimeNode.InnerText = PauseGameTime.ToString();
            settingsNode.AppendChild(pauseGameTimeNode);

            return settingsNode;
        }

        public void SetSettings(XmlNode settings)
        {
            if (settings["usedEvents"] != null)
            {
                usedEvents.Clear();
                foreach (XmlNode node in settings["usedEvents"].ChildNodes)
                {
                    int id;
                    if (order.TryGetValue(node.InnerText, out id))
                    {
                        usedEvents.Add(eventList[id]);
                        availEvents.Remove(eventList[id]);
                    }
                }
                bindingUsedEvents.ResetBindings(true);
                bindingAvailEvents.ResetBindings(true);
            }

            bool pauseGameTime;
            if (settings["pauseGameTime"] != null && Boolean.TryParse(settings["pauseGameTime"].InnerText, out pauseGameTime))
            {
                PauseGameTime = pauseGameTime;
                chkPauseGameTime.Checked = PauseGameTime;
            }
        }
    }
}
