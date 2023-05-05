using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.ScreenEngine.RumorsNotes
{
    public class Rumor
    {
        private int id;
        private string header, fullHeader, body, resolvedBody, dismissedBody;
        private bool isNew, isCompleted, isSelected;
        private RumorState state;

        public enum RumorState { Resolved, Dismissed, Active }
        public RumorState State { get { return state; } set { state = value; } }

        private Rectangle selectRect;

        public int ID { get { return id; } }
        public string Header { get { return header; } }
        public string FullHeader { get { return fullHeader; } }
        public string Body { get { return body; } }
        public string ResolvedBody { get { return resolvedBody; } }
        public string DismissedBody { get { return dismissedBody; } }
        public bool IsNew { get { return isNew; } set { isNew = value; } }
        public bool IsCompleted { get { return (State == RumorState.Resolved || State == RumorState.Dismissed); } }
        public bool IsSelected { get { return isSelected; } set { isSelected = value; } }
        public Rectangle SelectRectangle { get { return selectRect; } set { selectRect = value; } }

        public const int maxWidth = 44;

        public Rumor(int ID, string Header, string Body, string ResolvedBody, string DismissedBody)
        {
            id = ID;
            header = Header;
            fullHeader = Header;
            body = Body;
            resolvedBody = ResolvedBody;
            dismissedBody = DismissedBody;

            if (header.Length > maxWidth)
                header = header.Substring(0, maxWidth - 4) + "...";

            state = RumorState.Active;
            isNew = true;
        }

        protected const string space = " ";
        public string SaveData()
        {
            return id.ToString() + space +
                   ((int)state).ToString() + space +
                   isNew.ToString();
        }
        public void LoadData(string data)
        {
            string[] words = data.Split(' ');

            state = (RumorState)int.Parse(words[1]);
            isNew = bool.Parse(words[2]);
        }
    }
}
