using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio 
{
    [CreateAssetMenu(menuName = "Audio/Multy Sound Preset")]
    public class MultySoundPreset : PlayableSoundSettings
    {
        [SerializeField] private bool Hide;
        [SerializeField] private string commonPrefix;
        [SerializeField] private List<SoundPreset> presets;


        private void OnValidate() 
        {
            if (presets == null || presets.Count == 0)
                return;
            var commonGroup = presets
                .Where(p => p != null)
                .Aggregate(new Dictionary<SoundGroup, int>(), (combine, preset) => {
                combine[preset.group] = combine.GetValueOrDefault(preset.group, 0) + 1;
                return combine;
            }).Select(p => (p.Key, p.Value)).OrderByDescending(p => p.Value).Select(p => p.Key).FirstOrDefault();

            if (commonGroup == null) 
                return;

            var initialCount = presets.Count;
            presets = presets.Where(p => p != null).Where(p => p.group == commonGroup).ToList();
            if (presets.Count != initialCount)
                Debug.LogError("all presets in RandomSound should have the same sound group");
        }

        private void OnEnable() {
            OnValidate();
        }

        public override SoundPreset GetPreset()
        {
            return presets[Random.Range(0, presets.Count)];
        }

        public override SoundGroup Group() => presets[0].group;

        public override string Preffix() => commonPrefix;

        public override bool IsReady() => !Hide && presets.Count > 0 && presets.All(p => p != null && p.IsCorrect);
    }
}
