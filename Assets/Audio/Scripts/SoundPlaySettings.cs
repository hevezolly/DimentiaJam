using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio 
{
    [System.Serializable]
    public class SoundPlaySettings
    {
        [Range(0, 1)]
        public float volume = 1f;

        public PitchType pitchType;

        [HideInInspector] public Vector2 minMaxPitch = new Vector2(1, 1);
        [HideInInspector] public float constantPitch = 1f;

        public SoundRepeatType repeat = SoundRepeatType.Once;

        [HideInInspector]
        [Min(1)]
        public int numberOfRepeats = 1;

        public SoundPlaySettings Copy() 
        {
            return new SoundPlaySettings() 
            {
                volume = volume,
                repeat = repeat,
                constantPitch = constantPitch,
                minMaxPitch = minMaxPitch,
                numberOfRepeats = numberOfRepeats
            };
        }

        public float GetPitch() 
        {
            if (pitchType == PitchType.Constant)
                return constantPitch;
            return Random.Range(minMaxPitch.x, minMaxPitch.y);
        }
    }

    public enum PitchType 
    {
        Constant,
        RandomBetweenTwoValues,
    }

    public enum SoundRepeatType 
    {
        Once,
        Loop,
        Number
    }
}
