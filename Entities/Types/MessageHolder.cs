using Pilgrimage_Of_Embers.Entities.Factions;

namespace Pilgrimage_Of_Embers.Entities.Types
{
    public class MessageHolder
    {
        object sender, baggage;
        BaseFaction senderFaction;
        int timer, cooldown;
        string message, subMessage;

        public object Sender { get { return sender; } }
        public object Baggage { get { return baggage; } }
        public BaseFaction SenderFaction { get { return senderFaction; } }

        public int Timer { get { return timer; } set { timer = value; } }
        public int Cooldown { get { return cooldown; } set { cooldown = value; } }

        public string Message { get { return message; } }
        public string SubMessage { get { return subMessage; } }

        public MessageHolder(object Sender, object Baggage, BaseFaction SenderFaction, string Message, string SubMessage, int Time, int Cooldown)
        {
            sender = Sender;
            baggage = Baggage;
            senderFaction = SenderFaction;

            timer = Time;
            cooldown = Cooldown;

            message = Message.Trim();
            subMessage = SubMessage.Trim();

            message = message.Replace("!", "");
            message = message.Replace(".", "");
            message = message.Replace("?", "");
            message = message.Replace(",", "");
            message = message.Replace("'", "");
            message = message.Replace("\"", "");
            message = message.Replace("-", "");
            message = message.Replace("(", "");
            message = message.Replace(")", "");
        }

        public virtual MessageHolder Copy()
        {
            MessageHolder copy = (MessageHolder)this.MemberwiseClone();

            copy.sender = this.sender;
            copy.senderFaction = this.senderFaction;
            copy.message = this.message;
            copy.subMessage = this.subMessage;

            return copy;
        }
    }
}
