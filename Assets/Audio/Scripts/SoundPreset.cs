using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio 
{
    [CreateAssetMenu(menuName = "Audio/Sound Preset")]
    public class SoundPreset : PlayableSoundSettings
    {
        public bool Hide = false;
        public string prefix = "";
        
        public AudioClip clip;

        public SoundGroup group;
        
        public SoundPlaySettings defaultSettings;

        public bool IsCorrect => clip != null;

        public override SoundPreset GetPreset() => this;

        public override SoundGroup Group() => group;

        public override bool IsReady() => !Hide && IsCorrect;

        public override string Preffix() => prefix;
    }
}
