using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Audio 
{
    public class SoundGroup : ScriptableObject
    {
        [SerializeField] private SoundGroup masterGroup;
        private List<SoundGroup> childGroups = new();

        public SoundGroup MasterGroup => masterGroup;
        public void SetChildGroups(List<SoundGroup> newChildGroups) 
        {
            if (childGroups != null)
            {
                foreach (var c in childGroups) 
                    c.masterGroup = null;
            }

            childGroups = newChildGroups ?? new List<SoundGroup>();

            foreach (var c in childGroups) 
                c.masterGroup = this;

        }

        [HideInInspector]
        public int order;


        [Range(0, 1)]
        [SerializeField]
        private float volume = 1;

        [HideInInspector]
        public UnityEvent<float> ValueChangeEvent; 

        private bool initiated = false;

        private string VolKey => name + ".Volume";
        private float GetVolume() 
        {
            if (!initiated) 
            {
                SetVolume(PlayerPrefs.GetFloat(VolKey, volume));
                initiated = true;
            }
            return volume;
        }

        private void SetVolume(float value) 
        {
            var clamped = Mathf.Clamp01(value);
            if (clamped != volume) 
            {
                volume = clamped;
                PlayerPrefs.SetFloat(VolKey, volume);
                ValueChangeEvent?.Invoke(Volume);
                if (childGroups != null)
                foreach (var c in childGroups)
                    c.TriggerUpdate();
            }
        }

        public void TriggerUpdate() 
        {
            ValueChangeEvent?.Invoke(Volume);
        }

        public float Volume {
            get {
                if (masterGroup != null)
                    return BaseVolume * masterGroup.Volume;
                return BaseVolume;
            }
        }

        public float BaseVolume 
        {
            get => GetVolume();
            set => SetVolume(value);
        }

        public void CopyFrom(SoundGroup other) 
        {
            volume = other.volume;
            masterGroup = other.masterGroup;
            OnValidate();
        }

        private void OnValidate() {
            BaseVolume = volume;
        }
    }
}
