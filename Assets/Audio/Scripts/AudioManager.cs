using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio 
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance {get; private set;}

        public static bool CanPlay => Instance != null;

        public AllSoundGroups soundAssets;

        private Dictionary<SoundGroup, SoundGroup> trackedGroups = new();

        [SerializeField]
        private PoolInitiator AudioSourcePool;

        private void Awake() 
        {
            if (Instance != null) 
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);   
            trackedGroups.Clear();
            trackedGroups = soundAssets.groups.ToDictionary(g => g, g => Instantiate(g)); 

            var childs = new Dictionary<SoundGroup, List<SoundGroup>>();

            foreach (var group in trackedGroups.Keys) 
            {
                if (group.MasterGroup == null)
                    continue;
                if (!childs.ContainsKey(group.MasterGroup))
                    childs[group.MasterGroup] = new List<SoundGroup>();
                
                childs[group.MasterGroup].Add(group);
            }

            foreach (var c in childs) 
            {
                c.Key.SetChildGroups(c.Value);
            }

            AudioSourcePool.Init();
        }

        public SoundPlayer Play(AudioClip clip, SoundPlaySettings settings, SoundGroup group,
            Vector3 position = default, bool immediatelyPlay = true) 
        {
            var player = AudioSourcePool.Get<SoundPlayer>(position);
            player.Init(clip, settings, group);
            if (immediatelyPlay)
                player.Play();
            return player;
        }

#if UNITY_EDITOR
        private void OnApplicationQuit() {
            foreach (var group in trackedGroups) 
            {
                group.Key.CopyFrom(group.Value);
                Destroy(group.Value);
            }
        }
#endif
    }
}
