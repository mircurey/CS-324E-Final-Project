using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Final_Project_Pacman
{
    public class SoundManager
    {
        // One-shot sound effects (use .Play() for these)
        public SoundEffect Chomp { get; private set; }
        public SoundEffect EatFruit { get; private set; }
        public SoundEffect Death { get; private set; }
        public SoundEffect EatGhost { get; private set; }
        public SoundEffect ExtraPac { get; private set; }
        public SoundEffect Intermission { get; private set; }

        // Beginning music needs an instance so we can loop/stop it
        private SoundEffect _beginningEffect;
        private SoundEffectInstance _beginningInstance;

        public void LoadContent(ContentManager Content)
        {
            // adjust asset names/paths if yours are different
            _beginningEffect = Content.Load<SoundEffect>("pacman_beginning");
            Chomp         = Content.Load<SoundEffect>("pacman_chomp");
            Death         = Content.Load<SoundEffect>("pacman_death");
            EatFruit      = Content.Load<SoundEffect>("pacman_eatfruit");
            EatGhost      = Content.Load<SoundEffect>("pacman_eatghost");
            ExtraPac      = Content.Load<SoundEffect>("pacman_extrapac");
            Intermission  = Content.Load<SoundEffect>("pacman_intermission");

            // create instance for beginning music so we can loop/stop it
            if (_beginningEffect != null)
            {
                _beginningInstance = _beginningEffect.CreateInstance();
                _beginningInstance.IsLooped = true;
            }
        }

        // Play/Stop control for beginning music
        public void PlayBeginning()
        {
            // if already playing do nothing
            if (_beginningInstance == null) return;
            if (_beginningInstance.State != SoundState.Playing)
                _beginningInstance.Play();
        }

        public void StopBeginning()
        {
            if (_beginningInstance == null) return;
            if (_beginningInstance.State == SoundState.Playing)
                _beginningInstance.Stop(); // immediate stop
            // (optional) reset instance so it can be played again cleanly
            _beginningInstance.Dispose();
            _beginningInstance = _beginningEffect.CreateInstance();
            _beginningInstance.IsLooped = true;
        }

        // Convenience wrappers for one-shot effects
        public void PlayChomp()     => Chomp?.Play();
        public void PlayEatFruit()  => EatFruit?.Play();
        public void PlayDeath()     => Death?.Play();
        public void PlayEatGhost()  => EatGhost?.Play();
        public void PlayExtraPac()  => ExtraPac?.Play();
        public void PlayIntermission() => Intermission?.Play();
    }
}
