using System;
using Torii.Event;
using UnityEngine;

namespace Torii.Systems
{
    [CreateAssetMenu(menuName = "System/Pause")]
    public class PauseSystem : ScriptableObject
    {
        public bool Paused => Math.Abs(Time.timeScale) < float.Epsilon;
        [NonSerialized] public bool CanPause = true;

        public ToriiEvent OnGamePause;
        public ToriiEvent OnGameUnpause;

        public void TogglePause()
        {
            if (Paused) Unpause();
            else Pause();
        }

        public void Pause()
        {
            if (!CanPause) return;

            Time.timeScale = 0;
            OnGamePause.Raise();
        }

        public void Unpause()
        {
            Time.timeScale = 1;
            OnGameUnpause.Raise();
        }
    }
}
