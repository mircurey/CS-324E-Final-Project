using System;
using Microsoft.Xna.Framework;

namespace Final_Project_Pacman
{
    public class GameTimer
    {
        private double _durationSeconds;
        private double _remainingSeconds;

        public bool IsRunning { get; private set; }

        public int RemainingSeconds => (int)Math.Ceiling(_remainingSeconds);

        public GameTimer(double durationSeconds)
        {
            _durationSeconds = durationSeconds;
            _remainingSeconds = durationSeconds;
            IsRunning = false;
        }

        public void Start()
        {
            if (_remainingSeconds <= 0) // reset if timer finished
                _remainingSeconds = _durationSeconds;

            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Reset()
        {
            _remainingSeconds = _durationSeconds;
            IsRunning = false;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsRunning)
                return;

            _remainingSeconds -= gameTime.ElapsedGameTime.TotalSeconds;

            if (_remainingSeconds <= 0)
            {
                _remainingSeconds = 0;
                IsRunning = false;
            }
        }
    }
}