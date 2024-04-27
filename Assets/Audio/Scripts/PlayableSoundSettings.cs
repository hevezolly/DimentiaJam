using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio 
{
    public abstract class PlayableSoundSettings: ScriptableObject 
    {
        public abstract string Preffix();
        public abstract SoundGroup Group();
        public abstract SoundPreset GetPreset();
        public abstract bool IsReady();
    }
}
