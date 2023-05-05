using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Pilgrimage_Of_Embers.AudioEngine
{
    public class MusicDatabase
    {
        private static List<MusicTrack> tracks = new List<MusicTrack>();
        public static List<MusicTrack> Tracks { get { return tracks; } }

        public static void Load(ContentManager cm)
        {
            tracks.Add(new MusicTrack(1, cm.Load<SoundEffect>("Audio/Music/Sorrow's Twilight"), .4f));
            tracks.Add(new MusicTrack(2, cm.Load<SoundEffect>("Audio/Music/BossDefeated"), .5f));
        }
    }
}
