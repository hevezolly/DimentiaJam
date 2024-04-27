using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio 
{
    [CreateAssetMenu(menuName = "Audio/All Sound groups")]
    public class AllSoundGroups : ScriptableObject
    {
        public List<SoundGroup> groups;
    }
}
