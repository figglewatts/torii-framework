using System;
using UnityEngine;

namespace Torii.Audio
{
    [Serializable]
    public class SoundscapeSound
    {
        public AudioClip Sound;

        //[MinMaxSlider(0, 360)] [InfoBox("0 is directly in front, 180 is behind, etc.")]
        public Vector2 AngleRange = new Vector2(0, 360);

        public float MinAngle => AngleRange.x;
        public float MaxAngle => AngleRange.y;

        //[MinMaxSlider(0, 500)] 
        public Vector2 DistanceRange = new Vector2(5, 15);
        public float MinDistance => DistanceRange.x;
        public float MaxDistance => DistanceRange.y;

        public float Weighting;
    }
}
