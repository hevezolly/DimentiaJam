using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio 
{
    [System.Serializable]
    public class Sound
    {
        public PlayableSoundSettings preset;

        public SoundPlayer Play(Vector3 position = default, bool immediatelyPlay = true) 
        {
            var p = preset.GetPreset();
            if (!AudioManager.CanPlay) {
                Debug.LogError($"Can't plaay audio clip {preset.name}, {nameof(AudioManager)} is not instantiated");
                return null;
            }
            return AudioManager.Instance.Play(p.clip, p.defaultSettings, p.group, position, immediatelyPlay);
        }
    }
}
