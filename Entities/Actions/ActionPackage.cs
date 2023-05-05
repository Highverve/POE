using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Performance;
using System.Text;

namespace Pilgrimage_Of_Embers.Entities.Actions
{
    public class ActionPackage
    {
        private StringBuilder currentAction = new StringBuilder();
        private CallLimiter actionLimit = new CallLimiter(250); //limited to 1/4th a second

        public ActionPackage() { }

        public void Update(GameTime gt)
        {
            if (actionLimit.IsCalling(gt) && currentAction.Length > 0)
                currentAction.Clear();
        }
        /// <summary>
        /// Actions are read and received by other classes, such as GameObject. Examples include "MINE", "CHOP", "DIG", etc.
        /// </summary>
        /// <param name="action"></param>
        public void SetAction(string action)
        {
            if (currentAction.Length == 0) //If the current action is none ...
                currentAction.Append(action); //set the currentAction to the specified action.
        }
        public StringBuilder CurrentAction { get { return currentAction; } }
    }
}
