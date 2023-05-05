namespace Pilgrimage_Of_Embers.Entities.Captions
{
    /// <summary>
    /// What the entity says while idle.
    /// </summary>
    public class BaseCaption
    {
        string[] captions;
        public string[] Captions { get { return captions; } }

        public BaseCaption(params string[] Captions)
        {
            captions = Captions;
        }
    }
}
