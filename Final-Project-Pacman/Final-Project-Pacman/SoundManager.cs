using Microsoft.Xna.Framework.Audio;

namespace Final_Project_Pacman
{
    public class SoundManager
    {
        public SoundEffect Beginning { get; private set; }
        public SoundEffect Chomp { get; private set; }
        public SoundEffect Death { get; private set; }
        public SoundEffect EatFruit { get; private set; }
        public SoundEffect EatGhost { get; private set; }
        public SoundEffect ExtraPac { get; private set; }
        public SoundEffect Intermission { get; private set; }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            // filenames based on your Content folder list; adjust keys if your pipeline paths differ
            Beginning = Content.Load<SoundEffect>("pacman_beginning");
            Chomp     = Content.Load<SoundEffect>("pacman_chomp");
            Death     = Content.Load<SoundEffect>("pacman_death");
            EatFruit  = Content.Load<SoundEffect>("pacman_eatfruit");
            EatGhost  = Content.Load<SoundEffect>("pacman_eatghost");
            ExtraPac  = Content.Load<SoundEffect>("pacman_extrapac");
            Intermission = Content.Load<SoundEffect>("pacman_intermission");
        }
    }
}
