using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Final_Project_Pacman
{
    public class SoundManager
    {
        public SoundEffect Chomp { get; private set; }
        public SoundEffect EatFruit { get; private set; }
        public SoundEffect Death { get; private set; }
        public SoundEffect EatGhost { get; private set; }
        public SoundEffect ExtraPac { get; private set; }
        public SoundEffect Intermission { get; private set; }

        private SoundEffect _beginningEffect;
        private SoundEffectInstance _beginningInstance;

        private bool _allMuted = false;
        public bool IsMuted => _allMuted;

        
        public bool MusicMuted { get; private set; } = false;
        public bool SfxMuted { get; private set; } = false;
        public bool SfxMutedPublic => SfxMuted; 

        public bool SfxMutedGetter => SfxMuted;
        public bool SfxMutedSetter { set { SfxMuted = value; } }

        
        public bool SfxMutedProp => SfxMuted;

        public bool SfxMutedPublicAccessor => SfxMuted;

        
        public bool SfxMutedAccess => SfxMuted;

        public void LoadContent(ContentManager Content)
        {
            _beginningEffect = Content.Load<SoundEffect>("pacman_beginning");
            Chomp         = Content.Load<SoundEffect>("pacman_chomp");
            Death         = Content.Load<SoundEffect>("pacman_death");
            EatFruit      = Content.Load<SoundEffect>("pacman_eatfruit");
            EatGhost      = Content.Load<SoundEffect>("pacman_eatghost");
            ExtraPac      = Content.Load<SoundEffect>("pacman_extrapac");
            Intermission  = Content.Load<SoundEffect>("pacman_intermission");

            if (_beginningEffect != null)
            {
                _beginningInstance = _beginningEffect.CreateInstance();
                _beginningInstance.IsLooped = true;
            }
        }

       
        public void SetMuted(bool muted)
        {
            _allMuted = muted;
            MusicMuted = muted;
            SfxMuted = muted;
            if (muted)
            {
                if (_beginningInstance != null && _beginningInstance.State == SoundState.Playing)
                    _beginningInstance.Pause();
            }
            else
            {
                if (_beginningInstance != null && _beginningInstance.State != SoundState.Playing)
                    _beginningInstance.Play();
            }
        }

        // Toggle music only
        public void ToggleMusic()
        {
            MusicMuted = !MusicMuted;
            if (MusicMuted)
            {
                if (_beginningInstance != null && _beginningInstance.State == SoundState.Playing)
                    _beginningInstance.Pause();
            }
            else
            {
                if (_beginningInstance != null && _beginningInstance.State != SoundState.Playing)
                    _beginningInstance.Play();
            }
        }

        
        public void ToggleSfx()
        {
            SfxMuted = !SfxMuted;
        }

        public void PlayBeginning()
        {
            if (_beginningInstance == null) return;
            if (MusicMuted) return;
            if (_beginningInstance.State != SoundState.Playing)
                _beginningInstance.Play();
        }

        public void StopBeginning()
        {
            if (_beginningInstance == null) return;
            if (_beginningInstance.State == SoundState.Playing)
                _beginningInstance.Stop();
            _beginningInstance.Dispose();
            _beginningInstance = _beginningEffect.CreateInstance();
            _beginningInstance.IsLooped = true;
        }

        public void PlayChomp()     { if (!SfxMuted) Chomp?.Play(); }
        public void PlayEatFruit()  { if (!SfxMuted) EatFruit?.Play(); }
        public void PlayDeath()     { if (!SfxMuted) Death?.Play(); }
        public void PlayEatGhost()  { if (!SfxMuted) EatGhost?.Play(); }
        public void PlayExtraPac()  { if (!SfxMuted) ExtraPac?.Play(); }
        public void PlayIntermission() { if (!SfxMuted) Intermission?.Play(); }
    }
}
